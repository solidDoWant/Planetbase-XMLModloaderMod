using Planetbase;
using PlanetbaseFramework;
using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using XMLModloaderModV2.Deserializers.Version2.ModSub;
using XMLModloaderModV2.Deserializers.Version2.ModSub.BuildingsSub.ConfigurationSub.PropertiesSub;
using XMLModloaderModV2.Deserializers.Version2.ModSub.BuildingsSub.ModelSub;

namespace XMLModloaderModV2.Deserializers.Version2
{
    [XmlRoot("Mod")]
    [ModLoaderIgnore]
    public class Version2Mod : XmlMod
    {
        public new static DeserializableTypeMapper TypeMapper => new DeserializableTypeMapper("2.0", "Version2", typeof(Version2Mod));

        [XmlAttribute("ModName")]
        public string DeserializedModName { get; set; }
        
        public override string ModName => DeserializedModName;

        [XmlAttribute("SchemaVersion")]
        public string SchemaVersion { get; set; }

        [XmlAttribute("ModVersion")]
        public string ModVersion { get; set; }

        [XmlAttribute("InternalName")]
        public string InternalName { get; set; }

        [XmlArray(ElementName = "Technologies")]
        [XmlArrayItem(ElementName = "Technology")]
        public Technology[] XmlTechnologies { get; set; }

        [XmlArray(ElementName = "Buildings")]
        [XmlArrayItem(ElementName = "Building")]
        public Building[] XmlBuildings { get; set; }

        [XmlIgnore]
        public List<Tech> Techs { get; private set; }

        [XmlIgnore]
        public List<ModuleType> Buildings { get; private set; }

        private static RuntimeObjectBuilder _builder { get; set; }

        [XmlIgnore]
        public RuntimeObjectBuilder Builder => _builder ?? (_builder = new RuntimeObjectBuilder(InternalName));

        public override void Init()
        {
            if (XmlTechnologies != null)
            {
                Techs = new List<Tech>(XmlTechnologies.Length);

                Debug.Log("Attempting to load " + XmlTechnologies.Length + " new technologies");

                foreach (Technology technology in XmlTechnologies)
                {
                    Tech newTech;

                    try
                    {
                        newTech = Builder.CreateTechObject(technology.ClassName);
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Error creating Tech with classname \"" + technology.ClassName + "\"");
                        Utils.LogException(e);
                        continue;
                    }

                    newTech.mName = technology.Name ?? technology.ClassName;

                    if (StringList.exists(newTech.mName))
                    {
                        newTech.mName = StringList.get(newTech.mName).Trim();
                    }

                    newTech.mValue = technology.Cost;

                    MerchantCategory category;

                    try
                    {
                        category = (MerchantCategory)Enum.Parse(typeof(MerchantCategory), technology.MerchantCategory);
                    }
                    catch (Exception e)
                    {
                        Debug.Log("\"" + technology.MerchantCategory + "\" category for \"" + technology.ClassName +
                                  "\" tech in mod \"" + ModName +
                                  "\" does not match any values for the MerchantCategory enum.");
                        Utils.LogException(e);
                        continue;
                    }

                    newTech.mMerchantCategory = category;

                    newTech.mDescription = technology.Description;

                    if (StringList.exists(newTech.mDescription))
                    {
                        newTech.mDescription = StringList.get(newTech.mDescription).Trim();
                    }

                    newTech.mIcon = technology.Icon == null ? Utils.ErrorTexture : ModTextures.FindTextureWithName(technology.Icon.FileName);

                    TypeList<Tech, TechList>.getInstance().add(newTech);

                    Techs.Add(newTech);

                    Debug.Log("Successfully loaded new technology \"" + newTech.getName() + "\"");
                }

                Debug.Log("Loaded " + Techs.Count + " new technologies successfully");
            }

            if (XmlBuildings != null)
            {
                Buildings = new List<ModuleType>(XmlBuildings.Length);

                Debug.Log("Attempting to load " + XmlBuildings.Length + " new buildings");

                foreach (Building xmlBuilding in XmlBuildings)
                {
                    try
                    {
                        BuildingModuleType newBuilding;

                        try
                        {
                            newBuilding = Builder.CreateBuildingModuleTypeObject(xmlBuilding.ClassName);
                        }
                        catch (Exception e)
                        {
                            {
                                Debug.Log("Error creating Building with classname \"" + xmlBuilding.ClassName + "\"");
                                Utils.LogException(e);
                                continue;
                            }
                        }

                        newBuilding.mName = xmlBuilding.DisplayName ?? xmlBuilding.ClassName;

                        if (StringList.exists(newBuilding.mName))
                        {
                            newBuilding.mName = StringList.get(newBuilding.mName).Trim();
                        }

                        newBuilding.ModuleObjects = new GameObject[xmlBuilding.Models.Length];

                        for (int i = 0; i < xmlBuilding.Models.Length; i++)
                        {
                            GameObject modelRootObject = new GameObject { name = newBuilding.mName + " root gameobject" };

                            foreach (Obj xmlGameObject in xmlBuilding.Models[i].GameObjects)
                            {
                                GameObject template = ModObjects.FindObjectByFilename(xmlGameObject.MeshFileName);

                                if (template != null)
                                {
                                    GameObject clone = GameObject.Instantiate(template);

                                    clone.tag = xmlGameObject.Tag == GameObjectTags.UNDEFINED ? 
                                        GameObjectTags.Untagged.ToString() : xmlGameObject.Tag.ToString();

                                    clone.transform.SetParent(modelRootObject.transform, false);
                                }
                                else
                                {
                                    Debug.Log("Couldn't find a file named \"" + xmlGameObject.MeshFileName + "\" in the mod's assets\\obj folder.");
                                }
                            }

                            modelRootObject.SetActive(true);

                            newBuilding.ModuleObjects[i] = modelRootObject;
                        }

                        newBuilding.mMaxSize = xmlBuilding.Models.Length - 1;

                        newBuilding.mDefaultSize = (int)Math.Floor((decimal)newBuilding.mMaxSize / 2);

                        newBuilding.mIcon = xmlBuilding.Icon == null ? Utils.ErrorTexture : ModTextures.FindTextureWithName(xmlBuilding.Icon.FileName);

                        if (xmlBuilding.Configuration != null)
                        {
                            if (xmlBuilding.Configuration.BaseResources != null)
                            {
                                newBuilding.BaseResourceCosts = new ResourceAmount[xmlBuilding.Configuration.BaseResources.Length];

                                for (int i = 0; i < xmlBuilding.Configuration.BaseResources.Length; i++)
                                {
                                    ResourceType type =
                                        TypeList<ResourceType, ResourceTypeList>.find(xmlBuilding.Configuration.BaseResources[i].XmlResourceType);

                                    newBuilding.BaseResourceCosts[i] = new ResourceAmount(type, xmlBuilding.Configuration.BaseResources[i].Amount);
                                }
                            }

                            if (xmlBuilding.Configuration.Properties != null)
                            {
                                if (xmlBuilding.Configuration.Properties.Flags != null)
                                {
                                    newBuilding.mFlags = xmlBuilding.Configuration.Properties.Flags.BitField;
                                }

                                if (xmlBuilding.Configuration.Properties.MaxUsers != null)
                                {
                                    newBuilding.mMaxUsers = (int)xmlBuilding.Configuration.Properties.MaxUsers.Value;
                                }

                                if (xmlBuilding.Configuration.Properties.Panel != null)
                                {
                                    try
                                    {
                                        ModuleType.Panel parsedEnumValue = (ModuleType.Panel)Enum.Parse(typeof(ModuleType.Panel),
                                            xmlBuilding.Configuration.Properties.Panel.Value);

                                        newBuilding.mRelatedPanel = parsedEnumValue;
                                    }
                                    catch (Exception e)
                                    {
                                        Debug.Log("Exception thrown while converting \"" + xmlBuilding.Configuration.Properties.Panel + "\" to enum of type \"ModuleType.Panel\"");
                                        Debug.Log(e);
                                    }
                                }

                                if (xmlBuilding.Configuration.Properties.Layout != null)
                                {
                                    try
                                    {
                                        ModuleType.LayoutType parsedEnumValue = (ModuleType.LayoutType)Enum.Parse(typeof(ModuleType.LayoutType),
                                            xmlBuilding.Configuration.Properties.Layout.Value);

                                        newBuilding.mLayoutType = parsedEnumValue;
                                    }
                                    catch (Exception e)
                                    {
                                        Debug.Log("Exception thrown while converting \"" + xmlBuilding.Configuration.Properties.Layout + "\" to enum of type \"ModuleType.LayoutType\"");
                                        Debug.Log(e);
                                    }
                                }


                                if (xmlBuilding.Configuration.Properties.XmlExtraSizeTech != null &&
                                    TypeList<Tech, TechList>.find(xmlBuilding.Configuration.Properties.XmlExtraSizeTech.Value) != null)
                                {
                                    newBuilding.mExtraSizeTech =
                                        TypeList<Tech, TechList>.find(xmlBuilding.Configuration.Properties.XmlExtraSizeTech.Value);

                                    newBuilding.mExtraSize = newBuilding.mMaxSize;
                                    newBuilding.mMaxSize--;
                                }

                                if (xmlBuilding.Configuration.Properties.PowerStorageCapacity != null)
                                {
                                    newBuilding.mPowerStorageCapacity = xmlBuilding.Configuration.Properties.PowerStorageCapacity.Value;
                                }

                                if (xmlBuilding.Configuration.Properties.WaterStorageCapacity != null)
                                {
                                    newBuilding.mWaterStorageCapacity = xmlBuilding.Configuration.Properties.WaterStorageCapacity.Value;
                                }

                                if (xmlBuilding.Configuration.Properties.PowerGenerationRate != null)
                                {
                                    newBuilding.mPowerGeneration = xmlBuilding.Configuration.Properties.PowerGenerationRate.Value;
                                }

                                if (xmlBuilding.Configuration.Properties.WaterGenerationRate != null)
                                {
                                    newBuilding.mWaterStorageCapacity = xmlBuilding.Configuration.Properties.WaterGenerationRate.Value;
                                }

                                if (xmlBuilding.Configuration.Properties.IsExterior != null)
                                {
                                    newBuilding.mExterior = xmlBuilding.Configuration.Properties.IsExterior.Value;
                                }

                                if (xmlBuilding.Configuration.Properties.XmlComponents != null)
                                {
                                    List<ComponentType> validComponents = new List<ComponentType>(xmlBuilding.Configuration.Properties.XmlComponents.Length);

                                    foreach (AttributeClassName<string> componentTypeName in xmlBuilding.Configuration.Properties.XmlComponents)
                                    {
                                        ComponentType foundType =
                                            TypeList<ComponentType, ComponentTypeList>.find(componentTypeName.Value);

                                        if (foundType != null)
                                        {
                                            validComponents.Add(foundType);
                                        }
                                    }

                                    newBuilding.mComponentTypes = validComponents.ToArray();
                                }

                                if (xmlBuilding.Configuration.Properties.ToolTip != null)
                                {
                                    newBuilding.mTooltip = xmlBuilding.Configuration.Properties.ToolTip.Value;

                                    if (StringList.exists(newBuilding.mTooltip))
                                    {
                                        newBuilding.mTooltip = StringList.get(newBuilding.mTooltip).Trim();
                                    }
                                }

                                if (xmlBuilding.Configuration.Properties.Decay != null)
                                {
                                    ResourceType foundResource =
                                        TypeList<ResourceType, ResourceTypeList>.find(xmlBuilding.Configuration.Properties.Decay
                                            .RepairResource);

                                    if (foundResource != null)
                                    {
                                        newBuilding.mCondicionDecayTime = xmlBuilding.Configuration.Properties.Decay.Time;
                                        newBuilding.mRepairResource = foundResource;
                                    }
                                    else
                                    {
                                        Debug.Log("Cound not find a loaded resource matching \"" + xmlBuilding.Configuration.Properties.Decay.RepairResource + "\"");
                                    }
                                }

                                if (xmlBuilding.Configuration.Properties.Prestige != null)
                                {
                                    newBuilding.mPrestige = xmlBuilding.Configuration.Properties.Prestige.Value;
                                }
                            }
                        }

                        TypeList<ModuleType, ModuleTypeList>.getInstance().add(newBuilding);

                        Buildings.Add(newBuilding);

                        Debug.Log("Successfully loaded new building \"" + newBuilding.getName() + "\"");
                    }
                    catch (Exception e)
                    {
                        Debug.Log("Failed to load new building. Exception thrown: ");
                        Utils.LogException(e);
                    }
                }

                Debug.Log("Successfully loaded " + Buildings.Count + " new buildings");
            }
        }
    }
}