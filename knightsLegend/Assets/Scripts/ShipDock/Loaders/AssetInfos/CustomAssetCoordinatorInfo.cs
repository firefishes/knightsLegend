using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Loader
{
    [CreateAssetMenu(fileName = "ShipDockAssetCoordinatorInfo", menuName = "ShipDock : 临时资源协调器信息", order = 100)]
    public class CustomAssetCoordinatorInfo : ScriptableObject
    {
        [SerializeField]
        public List<CustomAssetComponentInfo> assets;
    }
}