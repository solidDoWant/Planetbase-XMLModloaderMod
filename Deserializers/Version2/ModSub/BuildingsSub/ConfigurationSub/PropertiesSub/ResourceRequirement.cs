using System;
using System.Xml.Serialization;

namespace XMLModloaderModV2.Deserializers.Version2.ModSub.BuildingsSub.ConfigurationSub.PropertiesSub
{
    [Serializable]
    public class ResourceRequirement
    {
        [XmlAttribute(AttributeName = "Type")]
        public string XmlResourceType { get; set; }

        [XmlAttribute(AttributeName = "Cost")]
        public int Amount { get; set; }
    }
}
