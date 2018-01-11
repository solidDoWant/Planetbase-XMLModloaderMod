using System;
using System.Collections.Generic;

namespace XMLModloaderModV2
{
    public class DeserializerRegistry : List<DeserializableTypeMapper>
    {
        private static DeserializerRegistry Instance { get; set; }

        public static DeserializerRegistry Registry
        {
            get
            {
                if (Instance == null)
                {
                    Instance = new DeserializerRegistry();
                }

                return Instance;
            }
        }

        public new void Add(DeserializableTypeMapper item)
        {
            if (!IsValidClass(item))
            {
                throw new ArgumentException("The Deserializer Registry already contains an item for version " + item.Version, nameof(item));
            }

            base.Add(item);
        }

        public new void AddRange(IEnumerable<DeserializableTypeMapper> collection)
        {
            foreach (DeserializableTypeMapper deserializableClass in collection)
            {
                if (!IsValidClass(deserializableClass))
                {
                    throw new ArgumentException("The Deserializer Registry already contains an item for version " + deserializableClass.Version, nameof(collection));
                }
            }

            base.AddRange(collection);
        }

        public DeserializableTypeMapper GetVersionTypeMapper(string Version)
        {
            foreach (DeserializableTypeMapper typeMapper in Registry)
            {
                if (typeMapper.Version.Equals(Version))
                {
                    return typeMapper;
                }
            }

            return null;
        }

        public static bool IsValidClass(DeserializableTypeMapper item)
        {
            foreach (DeserializableTypeMapper typeMapper in Registry)
            {
                if (typeMapper.Version.Equals(item.Version))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
