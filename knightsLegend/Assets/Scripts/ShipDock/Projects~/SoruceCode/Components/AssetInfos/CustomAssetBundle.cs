using ShipDock.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Loader
{
    public class CustomAssetBundle : ICustomAssetBundle
    {
        public KeyValueList<string, List<int>> mBundleMapper;
        public List<KeyValueList<string, CustomAsset>> mAssetsCacher;

        public CustomAssetBundle()
        {
            mBundleMapper = new KeyValueList<string, List<int>>();
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

                    if (!mBundleMapper.ContainsKey(bundleName))
                    {
                        mBundleMapper[bundleName] = new List<int>();
                    }
                    int index = mAssetsCacher.Count - 1;
                    mBundleMapper[bundleName].Add(index);

                    if (comp.Assets != default)
                    {
                        int n = comp.Assets.Count;
                        for (int j = 0; j < n; j++)
                        {
                            item = comp.Assets[j];
                            cahcer[item.assetName] = item;
                        }
                    }
                }
            }
        }

        public T GetCustomAsset<T>(string name, string path) where T : Object
        {
            T result = default;
            if(mBundleMapper.ContainsKey(name))
            {
                List<int> list = mBundleMapper[name];
                int index;
                int max = list.Count;
                for (int i = 0; i < max; i++)
                {
                    index = list[i];
                    KeyValueList<string, CustomAsset> cahceItem = mAssetsCacher[index];
                    CustomAsset asset = ((cahceItem != default) && cahceItem.ContainsKey(path)) ? cahceItem[path] : default;
                    result = (asset != default) ? asset.GetAsset<T>() : default;
                    if (result != default)
                    {
                        break;
                    }
                }
            }
            return result;
        }
    }

}