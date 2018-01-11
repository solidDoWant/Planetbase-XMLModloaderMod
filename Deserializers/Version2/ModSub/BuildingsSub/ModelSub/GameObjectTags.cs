using System.Xml.Serialization;

namespace XMLModloaderModV2.Deserializers.Version2.ModSub.BuildingsSub.ModelSub
{
    public enum GameObjectTags
    {
        UNDEFINED,
        [XmlEnum]
        AnchorPoint,
        [XmlEnum]
        DomeOpaque,
        [XmlEnum]
        DomeTranslucent,
        [XmlEnum]
        DomeStaticTranslucent,
        [XmlEnum]
        DomeStatic,
        [XmlEnum]
        DomeMobile,
        [XmlEnum]
        DomeBase,
        [XmlEnum]
        DomeFloor,
        [XmlEnum]
        Untagged,
        [XmlEnum]
        SelectionObject,
        [XmlEnum]
        Sprite
    }
}
