using System;
using System.IO;
using PlanetbaseFramework;

namespace XMLModloaderModV2
{
    [Serializable]
    public abstract class XmlMod : ModBase
    {
        public static DeserializableTypeMapper TypeMapper => throw new NotImplementedException();

        public override string ModPath => BasePath + "XML Modloader" + Path.DirectorySeparatorChar + ModName +
                                          Path.DirectorySeparatorChar;
    }
}
