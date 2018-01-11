using System;
using System.Xml.Serialization;
using Planetbase;
using XMLModloaderModV2.Deserializers.Version2.ModSub.BuildingsSub.ConfigurationSub.PropertiesSub;

namespace XMLModloaderModV2.Deserializers.Version2.ModSub.BuildingsSub.ConfigurationSub
{
    [Serializable]
    public class Properties
    {
        [XmlElement(ElementName = "Flags")]
        public Flags Flags { get; set; }

        [XmlElement(ElementName = "MaxUsers")]
        public AttributeValue<int?> MaxUsers { get; set; }

        [XmlElement(ElementName = "InfoPanel")]
        public AttributeValue<string> Panel { get; set; }

        [XmlElement(ElementName = "Layout")]
        public AttributeValue<string> Layout { get; set; }

        [XmlElement(ElementName = "ExtraSizeTech")]
        public AttributeClassName<string> XmlExtraSizeTech { get; set; }

        [XmlElement(ElementName = "PowerStorageCapacity")]
        public AttributeValue<int> PowerStorageCapacity { get; set; }

        [XmlElement(ElementName = "WaterStorageCapacity")]
        public AttributeValue<int> WaterStorageCapacity { get; set; }

        [XmlElement(ElementName = "PowerGenerationRate")]
        public AttributeValue<int> PowerGenerationRate { get; set; }

        [XmlElement(ElementName = "WaterGenerationRate")]
        public AttributeValue<int> WaterGenerationRate { get; set; }

        [XmlElement(ElementName = "Exterior")]
        public AttributeValue<bool> IsExterior { get; set; }

        [XmlArray(ElementName = "Components")]
        [XmlArrayItem(ElementName = "Component")]
        public AttributeClassName<string>[] XmlComponents { get; set; }

        [XmlElement(ElementName = "ToolTip")]
        public AttributeValue<string> ToolTip { get; set; }

        [XmlElement(ElementName = "Decay")]
        public Decay Decay { get; set; }

        [XmlElement(ElementName = "Prestige")]
        public AttributeValue<int> Prestige { get; set; }
    }
}
