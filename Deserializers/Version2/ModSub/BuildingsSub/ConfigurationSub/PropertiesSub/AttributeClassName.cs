using System;
using System.Xml.Serialization;

namespace XMLModloaderModV2.Deserializers.Version2.ModSub.BuildingsSub.ConfigurationSub.PropertiesSub
{
    [Serializable]
    public class AttributeClassName<T>
    {
        [XmlAttribute(AttributeName = "ClassName")]
        public T Value { get; set; }
    }
}
