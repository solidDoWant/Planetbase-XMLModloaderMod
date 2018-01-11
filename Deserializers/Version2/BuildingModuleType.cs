using System.Collections.Generic;
using Planetbase;
using PlanetbaseFramework;
using UnityEngine;

namespace XMLModloaderModV2.Deserializers.Version2
{
    public class BuildingModuleType : ModuleType
    {
        public ResourceAmount[] BaseResourceCosts { get; set; } = new ResourceAmount[0];

        public List<ResourceAmounts> ResourceCosts { get;} = new List<ResourceAmounts>();

        public GameObject[] ModuleObjects { get; set; }

        public BuildingModuleType()
        {
            mMinSize = 0;
        }

        public override ResourceAmounts calculateCost(int sizeIndex)
        {
            if (ResourceCosts.Count - 1 < sizeIndex)
            {
                ResourceCosts.AddRange(new ResourceAmounts[sizeIndex - (ResourceCosts.Count - 1)]);
            }

            if (ResourceCosts[sizeIndex] == null)
            {
                ResourceCosts[sizeIndex] = new ResourceAmounts();
                
                foreach (ResourceAmount baseResource in BaseResourceCosts)
                {
                    ResourceCosts[sizeIndex].add(baseResource.mResourceType, baseResource.mAmount + sizeIndex);
                }
            }

            return ResourceCosts[sizeIndex];
        }

        //Most of thiis is directly copy/pasted from the Framework. Unless otherwise commented, refer to the framework for more information.
        public override GameObject loadPrefab(int sizeIndex)
        {
            GameObject prefabObject = ModuleObjects[sizeIndex];

            prefabObject.calculateSmoothMeshRecursive(mMeshes);

            prefabObject.transform.RecursivelyAddColliders();

            foreach (MeshRenderer childRenderer in prefabObject.GetComponentsInChildren<MeshRenderer>())
            {
                childRenderer.enabled = true;
            }

            GameObject moduleObject2 = GameObject.Find(GroupName) ?? new GameObject { name = GroupName };

            prefabObject.transform.SetParent(moduleObject2.transform, false);

            return prefabObject;
        }
    }
}
