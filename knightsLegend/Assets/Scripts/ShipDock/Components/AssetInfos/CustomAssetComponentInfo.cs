using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Loader
{
    [Serializable]
    public class CustomAssetComponentInfo
    {
        public bool valid;
        public string bundleName;
        [SerializeField]
        public List<CustomAssetInfo> assetItems;
    }
}