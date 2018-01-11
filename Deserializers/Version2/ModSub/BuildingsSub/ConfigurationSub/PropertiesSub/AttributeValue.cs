using System;
using System.Xml.Serialization;

namespace XMLModloaderModV2.Deserializers.Version2.ModSub.BuildingsSub.ConfigurationSub.PropertiesSub
{
    [Serializable]
    public class AttributeValue<T>
    {
        [XmlAttribute(AttributeName = "Value")]
        public T Value { get; set; }
    }
}
