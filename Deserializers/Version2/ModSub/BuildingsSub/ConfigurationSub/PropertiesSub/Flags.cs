using System;
using System.Xml.Serialization;
using PlanetbaseFramework;
using UnityEngine;

namespace XMLModloaderModV2.Deserializers.Version2.ModSub.BuildingsSub.ConfigurationSub.PropertiesSub
{
    [Serializable]
    public class Flags
    {
        [XmlAttribute(AttributeName = "IsLandingPad")]
        public bool IsLandingPad { get; set; }

        [XmlAttribute(AttributeName = "IsMine")]
        public bool IsMine { get; set; }

        [XmlAttribute(AttributeName = "IsAirlock")]
        public bool IsAirlock { get; set; }

        [XmlAttribute(AttributeName = "IsStorage")]
        public bool IsStorage { get; set; }

        [XmlAttribute(AttributeName = "HasDome")]
        public bool HasDome { get; set; }

        [XmlAttribute(AttributeName = "HasLightAtNight")]
        public bool HasLightAtNight { get; set; }

        [XmlAttribute(AttributeName = "NeedsWind")]
        public bool NeedsWind { get; set; }

        [XmlAttribute(AttributeName = "NeedsLight")]
        public bool NeedsLight { get; set; }

        [XmlAttribute(AttributeName = "HasNoFoundations")]
        public bool HasNoFoundations { get; set; }

        [XmlAttribute(AttributeName = "IsDeadEnd")]
        public bool IsDeadEnd { get; set; }

        [XmlAttribute(AttributeName = "IsWalkable")]
        public bool IsWalkable { get; set; }

        [XmlAttribute(AttributeName = "HasBlinkingLights")]
        public bool HasBlinkingLights { get; set; }

        [XmlAttribute(AttributeName = "ShouldSnapComponents")]
        public bool ShouldSnapComponents { get; set; }

        [XmlAttribute(AttributeName = "IsStarport")]
        public bool IsStarport { get; set; }

        [XmlAttribute(AttributeName = "ShouldAutoRotate")]
        public bool ShouldAutoRotate { get; set; }

        [XmlAttribute(AttributeName = "IsAnimated")]
        public bool IsAnimated { get; set; }

        [XmlAttribute(AttributeName = "HasCylidricalBase")]
        public bool HasCylidricalBase { get; set; }

        [XmlAttribute(AttributeName = "IsRemotelyOperated")]
        public bool IsRemotelyOperated { get; set; }

        [XmlAttribute(AttributeName = "HasScanAnimation")]
        public bool HasScanAnimation { get; set; }

        [XmlAttribute(AttributeName = "HasPriorityControls")]
        public bool HasPriorityControls { get; set; }

        [XmlAttribute(AttributeName = "HasAntiMeteorLaser")]
        public bool HasAntiMeteorLaser { get; set; }

        [XmlAttribute(AttributeName = "HasLightningRod")]
        public bool HasLightningRod { get; set; }

        [XmlAttribute(AttributeName = "IsDisasterDetector")]
        public bool IsDisasterDetector { get; set; }

        [XmlIgnore]
        public int BitField {
            get
            {
                int field = 0;

                field |= IsLandingPad.ToInt() << 0;
                field |= IsMine.ToInt() << 1;
                field |= IsAirlock.ToInt() << 2;
                field |= IsStorage.ToInt() << 3;
                field |= HasDome.ToInt() << 4;
                field |= HasLightAtNight.ToInt() << 5;
                field |= NeedsWind.ToInt() << 6;
                field |= NeedsLight.ToInt() << 7;
                field |= HasNoFoundations.ToInt() << 8;
                field |= IsDeadEnd.ToInt() << 9;
                field |= IsWalkable.ToInt() << 10;
                field |= HasBlinkingLights.ToInt() << 11;
                field |= ShouldSnapComponents.ToInt() << 12;
                field |= IsStarport.ToInt() << 13;
                field |= ShouldAutoRotate.ToInt() << 14;
                field |= IsAnimated.ToInt() << 15;
                field |= HasCylidricalBase.ToInt() << 16;
                field |= IsRemotelyOperated.ToInt() << 17;
                field |= HasScanAnimation.ToInt() << 18;
                field |= HasPriorityControls.ToInt() << 19;
                field |= HasAntiMeteorLaser.ToInt() << 20;
                field |= HasLightningRod.ToInt() << 21;
                field |= IsDisasterDetector.ToInt() << 22;

                return field;
            } 
        }
    }
}
