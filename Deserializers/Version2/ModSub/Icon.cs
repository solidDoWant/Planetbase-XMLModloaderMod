using System;
using System.Xml.Serialization;

namespace XMLModloaderModV2.Deserializers.Version2.ModSub
{
    [Serializable]
    public class Icon
    {
        [XmlAttribute(AttributeName = "FileName")]
        public string FileName { get; set; }
    }
}
