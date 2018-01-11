using System;
using System.Xml.Serialization;
using XMLModloaderModV2.Deserializers.Version2.ModSub.BuildingsSub;

namespace XMLModloaderModV2.Deserializers.Version2.ModSub
{
    [Serializable]
    public class Building
    {
        [XmlAttribute(AttributeName = "ClassName")]
        public string ClassName { get; set; }

        [XmlAttribute(AttributeName = "DisplayName")]
        public string DisplayName { get; set; }

        [XmlArray(ElementName = "Models")]
        [XmlArrayItem(ElementName = "Model")]
        public Model[] Models { get; set; }

        [XmlElement(ElementName = "Icon")]
        public Icon Icon { get; set; }

        [XmlElement(ElementName = "Configuration")]
        public Configuration Configuration { get; set; }
    }
}
