using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;
using PlanetbaseFramework;
using UnityEngine;

namespace XMLModloaderModV2
{
    public class XmlModloaderMod : ModBase
    {
        public override string ModName => "XML Modloader";

        public override void Init()
        {
            Debug.Log("Loading XML mods...");

            Debug.Log("Loading embedded schemas...");
            Dictionary<string, XmlSchema> embeddedSchemas = GetEmbeddedSchemas();
            uint loadedSchemaCount = 0;
            foreach (KeyValuePair<string, XmlSchema> embeddedSchema in embeddedSchemas)
            {
                try
                {
                    DeserializableTypeMapper.SchemaRegistry.Add(embeddedSchema.Key, embeddedSchema.Value);
                    loadedSchemaCount++;
                }
                catch (ArgumentException e)
                {
                    Debug.Log("Couldn't load add schema due to the following exception being thrown:");
                    Debug.LogException(e);
                }
            }
            Debug.Log("Loaded " + loadedSchemaCount + " schema(s)");

            Debug.Log("Loading deserializer classes...");

            uint loadedDeserializerCount = 0;
            foreach (Type mapperType in Assembly.GetExecutingAssembly().GetTypes())
            {
                if (typeof(XmlMod).IsAssignableFrom(mapperType) && !mapperType.IsAbstract)
                {
                    DeserializableTypeMapper mapper;

                    try
                    {
                        mapper =
                            mapperType.GetProperty(nameof(XmlMod.TypeMapper))
                                ?.GetValue(null, null) as DeserializableTypeMapper;
                    }
                    catch (NotImplementedException e)
                    {
                        Debug.Log("Deserializer class \"" + mapperType + "\" does not implement " + nameof(XmlMod.TypeMapper));
                        Utils.LogException(e);
                        continue;
                    }
                    catch(Exception e)
                    {
                        Debug.Log("Exception thrown while attempting to get the \"" + nameof(DeserializableTypeMapper) + "\" property from type \"" + mapperType + "\"");
                        Utils.LogException(e);
                        continue;
                    }

                    if (!DeserializableTypeMapper.SchemaRegistry.ContainsKey(mapper.SchemaName))
                    {
                        Debug.Log("No schema loaded with schema name \"" + mapper.SchemaName + "\". Aborting load of deserializer type \"" + mapperType.Name + "\"for deserializer version " + mapper.Version + ".");
                        continue;
                    }

                    DeserializerRegistry.Registry.Add(mapper);
                    loadedDeserializerCount++;

                    Debug.Log("Loaded deserializer class \"" + mapperType + "\" successfully");
                }
            }

            Debug.Log("Loaded " + loadedDeserializerCount + " deserializers successfully");

            if (Directory.Exists(ModPath))
            {
                string[] files = Directory.GetFiles(ModPath, "*.xml");
                Debug.Log("Found " + files.Length + " XML files in " + ModPath);

                foreach (string file in files)
                {
                    Debug.Log("Loading " + file);

                    string schemaVersion;
                    string modName;
                    string internalName;

                    using (XmlReader reader = XmlReader.Create(file,
                        new XmlReaderSettings {ValidationFlags = XmlSchemaValidationFlags.None}))
                    {
                        reader.ReadToFollowing("Mod");
                        schemaVersion = reader.GetAttribute("SchemaVersion");
                        modName = reader.GetAttribute("ModName");
                        internalName = reader.GetAttribute("InternalName");
                    }

                    if (schemaVersion == null)
                    {
                        Debug.Log(file + " not a valid mod file. Missing <Mod> tag or SchemaVersion attribute in mod tag.");
                        continue;
                    }

                    if (modName == null)
                    {
                        Debug.Log(file + " not a valid mod file. Missing <Mod> tag or ModName attribute in mod tag.");
                        continue;
                    }

                    if (internalName == null)
                    {
                        Debug.Log(file + " not a valid mod file. Missing <Mod> tag or InternalName attribute in mod tag.");
                        continue;
                    }

                    Debug.Log(modName + " uses version " + schemaVersion + " deserializer");

                    DeserializableTypeMapper
                        versionMapper = DeserializerRegistry.Registry.GetVersionTypeMapper(schemaVersion);

                    if (versionMapper == null)
                    {
                        Debug.Log("No schema loaded for schema version " + schemaVersion);
                        continue;
                    }

                    Type modType = RuntimeObjectBuilder.CreateXmlModType(modName, internalName, versionMapper.DeserializableType);

                    NameTable nameTable = new NameTable();

                    XmlNamespaceManager namespaceManager = new XmlNamespaceManager(nameTable);
                    namespaceManager.AddNamespace("", "");

                    XmlParserContext parserContext =
                        new XmlParserContext(nameTable, namespaceManager, "", XmlSpace.Default);

                    XmlSchemaSet schemaSet = new XmlSchemaSet(nameTable);
                    schemaSet.Add(versionMapper.Schema);

                    XmlReaderSettings readerSettings = new XmlReaderSettings
                    {
                        NameTable = nameTable,
                        ValidationType = ValidationType.Schema,
                        Schemas = schemaSet,
                        ValidationFlags = XmlSchemaValidationFlags.ReportValidationWarnings | XmlSchemaValidationFlags.ProcessIdentityConstraints
                    };

                    _xmlInvalid = false;
                    readerSettings.ValidationEventHandler += XmlValidationEventCallback;

                    _xsdInvalid = false;
                    schemaSet.ValidationEventHandler += XsdValidationEventCallback;

                    schemaSet.Compile();

                    XmlAttributeOverrides ignoreOverrides = new XmlAttributeOverrides();
                    XmlAttributes attributes = new XmlAttributes { XmlIgnore = true };
                    foreach (PropertyInfo property in typeof(ModBase).GetProperties(
                        BindingFlags.Public | BindingFlags.Instance))
                    {
                        ignoreOverrides.Add(typeof(ModBase), property.Name, attributes);
                        ignoreOverrides.Add(typeof(XmlMod), property.Name, attributes);
                        ignoreOverrides.Add(versionMapper.DeserializableType, property.Name, attributes);
                        ignoreOverrides.Add(modType, property.Name, attributes);
                    }

                    try
                    {
                        using (XmlReader reader = XmlReader.Create(file, readerSettings, parserContext))
                        {
                            XmlSerializer xmlDeserializer;
                            try
                            {
                                xmlDeserializer = new XmlSerializer(modType, ignoreOverrides);
                            }
                            catch (Exception e)
                            {
                                Debug.Log("Unable to create a deserializer for type \"" + modType + "\"");
                                Utils.LogException(e);
                                continue;
                            }

                            XmlMod mod;

                            try
                            {
                                mod = xmlDeserializer.Deserialize(reader) as XmlMod;
                            }
                            catch (Exception e)
                            {
                                Debug.Log("Unable to deserialize file \"" + file + "\"");
                                Utils.LogException(e);
                                continue;
                            }

                            if (_xmlInvalid)
                            {
                                Debug.Log("Failed to load \"" + file + "\" of type \"" + modType + "\"");
                                continue;
                            }

                            Debug.Log("Successfully loaded mod file into XML properties");

                            try
                            {
                                mod.Init();
                            }
                            catch (Exception e)
                            {
                                Debug.Log("Exception thrown while initializing mod");
                                Utils.LogException(e);
                                continue;
                            }

                            Modloader.ModList.Add(mod);
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Exception thrown while attempting to create a stream reader for " + file + ". Exception thrown: ");
                        Debug.Log(e);
                        continue;
                    }

                    Debug.Log("Successfully loaded " + file);
                }
            }
            else
            {
                Debug.Log("No mods found in " + ModPath);
            }

            Debug.Log("Done loading XML mods");
        }

        private static Dictionary<string, XmlSchema> GetEmbeddedSchemas()
        {
            Dictionary<string, XmlSchema> schemas = new Dictionary<string, XmlSchema>();

            foreach (string file in Utils.ListEmbeddedFiles())
            {
                string fileName = Utils.GetFileNameFromAssemblyResourceName(file);

                Match filenameMatch = Regex.Match(fileName, @"^\w+\.(?i)xsd$");   //Regex to check for valid filename from namespace
                if (filenameMatch.Success)
                {
                    _xsdInvalid = false;

                    XmlSchema loadedSchema = null;

                    //This callback on this method should catch any XSD syntax errors. However due to (what I think is) a bug in the .Net Framework
                    //(see https://referencesource.microsoft.com/#System.Xml/System/Xml/Schema/XmlSchema.cs,110), this will not catch certain XML based
                    //exceptions, like missing tags, or incomplete tags.
                    try
                    {
                        loadedSchema = XmlSchema.Read(Utils.LoadEmbeddedFile(file), XsdValidationEventCallback);
                    }
                    catch (XmlException e)
                    {
                        Debug.Log("Exception thrown while validating XSD file \"" + file + "\" on line " + e.LineNumber + ", position" + e.LinePosition + ": ");
                        Utils.LogException(e);
                        _xsdInvalid = true;
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Exception thrown while validating XSD file \"" + file + "\": ");
                        Utils.LogException(e);
                        _xsdInvalid = true;
                    }

                    if (loadedSchema != null)
                    {
                        //I'm doing this instead of loadedSchema.Compile() because loadedSchema.Compile() is deprecated and recommends this approach.
                        //Some validation issues aren't caught until the XSD is compiled.
                        XmlSchemaSet validationSet = new XmlSchemaSet();
                        validationSet.Add(loadedSchema);
                        validationSet.ValidationEventHandler += XsdValidationEventCallback;
                        validationSet.Compile();
                    }

                    if (_xsdInvalid)
                    {
                        Debug.Log("Schema \"" + file + "\" not loaded due to validation error, see above");
                        continue;
                    }
                    else
                    {
                        schemas.Add(Path.GetFileNameWithoutExtension(fileName), loadedSchema);
                    }

                    Debug.Log("Loaded schema file \"" + file + "\"");
                }
            }

            return schemas;
        }

        private static bool _xsdInvalid;
        private static void XsdValidationEventCallback(object sender, ValidationEventArgs args)
        {
            _xsdInvalid = true;
            Debug.Log("XSD validation " + args.Severity.ToString().ToLower() + " on line " + args.Exception.LineNumber +
                      ", position " + args.Exception.LinePosition + ": " + args.Message);
        }

        private static bool _xmlInvalid;
        private static void XmlValidationEventCallback(object sender, ValidationEventArgs args)
        {
            _xmlInvalid = true;
            Debug.Log("XML validation " + args.Severity.ToString().ToLower() + " on line " + args.Exception.LineNumber + 
                ", position " + args.Exception.LinePosition + ": " + args.Message);
        }
    }
}