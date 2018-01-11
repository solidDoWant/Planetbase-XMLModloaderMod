using System;
using System.Collections.Generic;
using System.Xml.Schema;

namespace XMLModloaderModV2
{
    public class DeserializableTypeMapper
    {
        public static Dictionary<string, XmlSchema> SchemaRegistry { get; } = new Dictionary<string, XmlSchema>();

        public string Version { get; }
        public string SchemaName { get; }

        public XmlSchema Schema => SchemaRegistry[SchemaName];

        public Type DeserializableType { get; }

        public DeserializableTypeMapper(string version, string schemaName, Type deserializableTypes)
        {
            Version = version;
            SchemaName = schemaName;
            DeserializableType = deserializableTypes;
        }
    }
}