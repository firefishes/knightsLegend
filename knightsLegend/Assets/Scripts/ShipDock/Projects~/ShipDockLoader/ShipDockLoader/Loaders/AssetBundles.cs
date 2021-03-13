#define _LOG_ASSET_QUOTEDER//是否开启资源引用器的日志

using ShipDock;
using ShipDock.Loader;
using ShipDock.Tools;
using System.Collections.Generic;
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

        public void Remove(string name, bool unloadAllLoaded = false)
        {
            AssetBundle ab = RemoveBundle(ref name);
            ab?.Unload(unloadAllLoaded);
        }

        public void Remove(AssetBundle bundle, bool unloadAllLoaded = false)
        {
            if (bundle == default)
            {
                return;
            }
            string name = bundle.name;
            RemoveBundle(ref name);
            bundle?.Unload(unloadAllLoaded);
        }

        private AssetBundle RemoveBundle(ref string name)
        {
            AssetBundle resul = default;
            if (mCaches.ContainsKey(name))
            {
                mABManifests.Remove(name);

                IAssetBundleInfo info = mCaches.Remove(name);
                resul = info.Asset;
                info.Dispose();
            }
            return resul;
        }

        public AssetBundleManifest GetManifest(string name = "")
        {
            name = string.IsNullOrEmpty(name) && (mABManifests.Size > 0) ? mABManifests.Keys[0] : name;
            return HasBundel(name) ? mABManifests[name] : default;
        }

        private const int IDColumnSize = 10;
        private IntegerID<string> abNameIDs = new IntegerID<string>();
        private IntegerID<string> assetNameIDs = new IntegerID<string>();
        private KeyValueList<int, Object> rawMapper = new KeyValueList<int, Object>();
        private KeyValueList<Object, AssetQuoteder> assetMapper = new KeyValueList<Object, AssetQuoteder>();
        private KeyValueList<int, AssetQuoteder> quotederMapper = new KeyValueList<int, AssetQuoteder>();
        private KeyValueList<string, int> bundlesCounter = new KeyValueList<string, int>();

        public T GetAndQuote<T>(string abName, string assetName, out AssetQuoteder quoteder) where T : Object
        {
            int abID = abNameIDs.GetID(ref abName);
            int assetID = assetNameIDs.GetID(ref assetName);
            int id = abID * IDColumnSize + assetID;//id 阵列转换
            Object raw;
            if (rawMapper.ContainsKey(id))
            {
                raw = rawMapper[id];
            }
            else
            {
                raw = Get<T>(abName, assetName);
                rawMapper[id] = raw;
            }
            quoteder = default;
            int mapperID = id;
            id = raw.GetInstanceID();
            if (quotederMapper.ContainsKey(id))
            {
                quoteder = quotederMapper[id];
            }
            else
            {
                quoteder = new AssetQuoteder(assetMapper, quotederMapper, bundlesCounter, rawMapper);
                quoteder.SetRaw(mapperID, ref abName, ref assetName, raw);
#if LOG_ASSET_QUOTEDER
                "log:New quote {0}, id is {1}, ab name is {2}, asset name is {3}".Log(
                    typeof(T).Name, id.ToString(), abName, assetName);
#endif
            }
            T result = quoteder.Instantiate<T>();
            return result;
        }

        public void UnloadUselessAssetBundles(params string[] abNames)
        {
            bool isCustome = abNames.Length > 0;
            List<string> list = isCustome ? new List<string>(abNames) : bundlesCounter.Keys;

            string key;
            List<string> deletes = new List<string>();
            int max = list.Count;
            for (int i = 0; i < max; i++)
            {
                key = list[i];
                if (bundlesCounter[key] == 0)
                {
                    deletes.Add(key);
                }
                else
                {
#if LOG_ASSET_QUOTEDER
                    if (isCustome)
                    {
                        "error: {0} is using, count is {1}".Log(key, bundlesCounter[key].ToString());
                    }
#endif
                }
            }
            max = deletes.Count;
            for (int i = 0; i < max; i++)
            {
                key = deletes[i];
                bundlesCounter.Remove(deletes[i]);
                Remove(key, true);
            }
        }

        public void UnloadQuote(string abName, string assetName)
        {
            int abID = abNameIDs.GetID(ref abName);
            int assetID = assetNameIDs.GetID(ref assetName);
            int id = abID * IDColumnSize + assetID;//id 阵列转换
            if (rawMapper.ContainsKey(id))
            {
                Object raw = rawMapper[id];
                id = raw.GetInstanceID();
                if (quotederMapper.ContainsKey(id))
                {
                    AssetQuoteder quoteder = quotederMapper[id];
                    quoteder.Dispose();
                }
            }
        }

        public void DestroyAsset(Object target, bool isAutoDispose = false)
        {
            if (assetMapper.Keys.Contains(target))
            {
                AssetQuoteder quoteder = assetMapper[target];
                quoteder.Destroy(target, false, isAutoDispose);
            }
        }
    }
}

public static class AssetBundlesExtensions
{
    public static void Destroy(this Object target, bool isAutoDispose = false)
    {
        AssetBundles abs = Framework.Instance.GetUnit<AssetBundles>(Framework.UNIT_AB);
        abs?.DestroyAsset(target, isAutoDispose);
    }
}

