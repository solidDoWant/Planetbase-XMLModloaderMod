using System;
using System.Xml.Serialization;
using XMLModloaderModV2.Deserializers.Version2.ModSub.BuildingsSub.ConfigurationSub;
using XMLModloaderModV2.Deserializers.Version2.ModSub.BuildingsSub.ConfigurationSub.PropertiesSub;

namespace XMLModloaderModV2.Deserializers.Version2.ModSub.BuildingsSub
{
    [Serializable]
    public class Configuration
    {
        [XmlArray(ElementName = "BaseCost")]
        [XmlArrayItem(ElementName = "ResourceRequirement")]
        public ResourceRequirement[] BaseResources { get; set; }

        [XmlElement(ElementName = "Properties")]
        public Properties Properties { get; set; }
    }
}