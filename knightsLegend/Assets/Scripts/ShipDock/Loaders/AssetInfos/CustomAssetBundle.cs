using ShipDock.Interfaces;
using ShipDock.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Loader
{
    public class CustomAssetBundle : IDispose
    {
        public KeyValueList<string, int> mBundleMapper;
        public List<KeyValueList<string, CustomAsset>> mAssetsCacher;

        public CustomAssetBundle()
        {
            mBundleMapper = new KeyValueList<string, int>();
            mAssetsCacher = new List<KeyValueList<string, CustomAsset>>();
        }

        public void Dispose()
        {
            Utils.Reclaim(ref mAssetsCacher);
            Utils.Reclaim(ref mBundleMapper);
        }

        public void FillAssets(ref List<CustomAssetComponent> assets)
        {
            CustomAsset item;
            CustomAssetComponent comp;
            KeyValueList<string, CustomAsset> cahcer;

            string bundleName;
            int max = assets.Count;
            for (int i = 0; i < max; i++)
            {
                comp = assets[i];
                if (comp != default)
                {
                    bundleName = comp.GetBundleName();
                    cahcer = new KeyValueList<string, CustomAsset>();
                    mAssetsCacher.Add(cahcer);
                    mBundleMapper[bundleName] = mAssetsCacher.Count - 1;

                    int n = comp.Assets.Count;
                    for (int j = 0; j < n; j++)
                    {
                        item = comp.Assets[j];
                        cahcer[item.assetName] = item;
                    }
                }
            }
        }

        public T GetCustomAsset<T>(string name, string path) where T : Object
        {
            T result = default;
            if(mBundleMapper.ContainsKey(name))
            {
                int index = mBundleMapper[name];
                KeyValueList<string, CustomAsset> cahceItem = mAssetsCacher[index];
                CustomAsset asset = ((cahceItem != default) && cahceItem.ContainsKey(path)) ? cahceItem[path] : default;
                result = (asset != default) ? asset.GetAsset<T>() : default;
            }
            return result;
        }
    }

}