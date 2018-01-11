using System;
using System.Xml.Serialization;

namespace XMLModloaderModV2.Deserializers.Version2.ModSub.BuildingsSub.ConfigurationSub.PropertiesSub
{
    [Serializable]
    public class Decay
    {
        [XmlAttribute(AttributeName = "Time")]
        public float Time { get; set; }

        [XmlAttribute(AttributeName = "RepairResource")]
        public string RepairResource { get; set; }
    }
}
