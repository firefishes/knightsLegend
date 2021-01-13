using ShipDock.Tools;
using UnityEngine;

namespace ShipDock.Loader
{
    /// <summary>
    /// 
    /// 资源包管理器
    /// 
    /// </summary>
    public class AssetBundles : IAssetBundles
    {
        public const string ASSET_BUNDLE_MANIFEST = "AssetBundleManifest";

        private ICustomAssetBundle mCustomAssets;
        private KeyValueList<string, IAssetBundleInfo> mCaches;
        private KeyValueList<string, AssetBundleManifest> mABManifests;

        public AssetBundles()
        {
            mABManifests = new KeyValueList<string, AssetBundleManifest>();
            mCaches = new KeyValueList<string, IAssetBundleInfo>();
        }

        public void Dispose()
        {
            Utils.Reclaim(ref mCaches, false, true);
            Utils.Reclaim(ref mABManifests, false, true);
        }

        public void SetCustomBundles(ref ICustomAssetBundle value)
        {
            mCustomAssets = value;
        }

        public bool HasBundel(string name)
        {
            return (mCaches != default) && mCaches.ContainsKey(name);
        }

        public T Get<T>(string name, string path) where T : Object
        {
            T result = (mCustomAssets != default) ? mCustomAssets.GetCustomAsset<T>(name, path) : default;
            if((result == default) && HasBundel(name))
            {
                IAssetBundleInfo assetBundleInfo = mCaches[name];
                result = assetBundleInfo.GetAsset<T>(path);
            }
            return result;
        }

        public GameObject Get(string name, string path)
        {
            GameObject result = (mCustomAssets != default) ? mCustomAssets.GetCustomAsset<GameObject>(name, path) : default;
            if ((result == default) && HasBundel(name))
            {
                IAssetBundleInfo assetBundleInfo = mCaches[name];
                result = assetBundleInfo.GetAsset(path);
            }
            return result;
        }

        public void Add(AssetBundle bundle)
        {
            if(bundle == default)
            {
                return;
            }
            AddBundle(string.Empty, ref bundle);
        }

        public void Add(string manifestName, AssetBundle bundle)
        {
            if (bundle == default)
            {
                return;
            }
            AddBundle(manifestName, ref bundle);
        }

        private void AddBundle(string name, ref AssetBundle bundle)
        {
            name = string.IsNullOrEmpty(name) ? bundle.name : name;
            IAssetBundleInfo info;
            if (!mCaches.ContainsKey(name))
            {
                info = new AssetBundleInfo(bundle);
                mCaches[name] = info;
                if (name.Contains("_unityscene"))
                {
                    return;
                }
                mABManifests[name] = bundle.LoadAsset<AssetBundleManifest>(ASSET_BUNDLE_MANIFEST);
            }
        }

        public void Remove(string name)
        {
            RemoveBundle(ref name);
        }

        public void Remove(AssetBundle bundle)
        {
            if (bundle == default)
            {
                return;
            }
            string name = bundle.name;
            RemoveBundle(ref name);
        }

        private void RemoveBundle(ref string name)
        {
            if (mCaches.ContainsKey(name))
            {
                mABManifests.Remove(name);

                IAssetBundleInfo info = mCaches[name];
                info.Dispose();
            }
        }

        public AssetBundleManifest GetManifest(string name = "")
        {
            name = string.IsNullOrEmpty(name) && (mABManifests.Size > 0) ? mABManifests.Keys[0] : name;
            return HasBundel(name) ? mABManifests[name] : default;
        }
    }
}

