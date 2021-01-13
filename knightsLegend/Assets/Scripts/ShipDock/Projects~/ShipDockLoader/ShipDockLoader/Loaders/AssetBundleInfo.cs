
using UnityEngine;

namespace ShipDock.Loader
{
    public class AssetBundleInfo : IAssetBundleInfo
    {
        public AssetBundleInfo(AssetBundle asset)
        {
            Asset = asset;
        }

        public void Dispose()
        {
            Asset = default;
        }

        public T GetAsset<T>(string path) where T : Object
        {
            return (Asset != default) ? Asset.LoadAsset<T>(path) : default;
        }

        public GameObject GetAsset(string path)
        {
            return (Asset != default) ? Asset.LoadAsset<GameObject>(path) : default;
        }

        public string Name
        {
            get
            {
                return Asset != default ? Asset.name : string.Empty;
            }
        }
        
        public AssetBundle Asset { get; private set; }
    }
}

