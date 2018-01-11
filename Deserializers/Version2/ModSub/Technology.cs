using System;
using System.Xml.Serialization;

namespace XMLModloaderModV2.Deserializers.Version2.ModSub
{
    [Serializable]
    public class Technology
    {
        [XmlAttribute(AttributeName = "DisplayName")]
        public string Name { get; set; }

        [XmlAttribute(AttributeName = "ClassName")]
        public string ClassName { get; set; }

        [XmlAttribute(AttributeName = "Cost")]
        public int Cost { get; set; }

        [XmlAttribute(AttributeName = "MerchantCategory")]
        public string MerchantCategory { get; set; }

        [XmlAttribute(AttributeName = "Description")]
        public string Description { get; set; } = "Description not set";

        [XmlElement(ElementName = "Icon")]
        public Icon Icon { get; set; }
    }
}
