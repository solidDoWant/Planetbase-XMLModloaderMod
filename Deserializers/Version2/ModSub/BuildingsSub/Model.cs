using System;
using System.Xml.Serialization;
using XMLModloaderModV2.Deserializers.Version2.ModSub.BuildingsSub.ModelSub;

namespace XMLModloaderModV2.Deserializers.Version2.ModSub.BuildingsSub
{
    [Serializable]
    public class Model
    {
        [XmlElement(ElementName = "GameObject")]
        public Obj[] GameObjects { get; set; }
    }
}
