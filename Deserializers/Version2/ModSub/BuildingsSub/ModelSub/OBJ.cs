using System;
using System.Xml.Serialization;

namespace XMLModloaderModV2.Deserializers.Version2.ModSub.BuildingsSub.ModelSub
{
    [Serializable]
    public class Obj
    {
        [XmlAttribute(AttributeName = "Tag")]
        public GameObjectTags Tag { get; set; }

        [XmlAttribute(AttributeName = "Mesh")]
        public string MeshFileName { get; set; }
    }
}
