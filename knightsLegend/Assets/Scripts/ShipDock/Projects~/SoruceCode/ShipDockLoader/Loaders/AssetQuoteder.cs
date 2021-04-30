#define _LOG_ASSET_QUOTEDER//是否开启资源引用器的日志

using ShipDock.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Loader
{

    public sealed class AssetQuoteder
    {
        public int MapperID { get; private set; }
        public int Count { get; private set; }
        public string ABName { get; private set; }
        public string AssetName { get; private set; }

        private Object mRaw;

        public KeyValueList<Object, AssetQuoteder> AssetMapper { get; private set; }
        public KeyValueList<int, AssetQuoteder> QuotederMapper { get; private set; }
        public KeyValueList<string, int> BundlesCounter { get; private set; }
        public KeyValueList<int, Object> RawMapper { get; private set; }

        public AssetQuoteder(
            KeyValueList<Object, AssetQuoteder> assetMapper,
            KeyValueList<int, AssetQuoteder> quotederMapper,
            KeyValueList<string, int> bundlesCounter,
            KeyValueList<int, Object> rawMapper)
        {
            AssetMapper = assetMapper;
            QuotederMapper = quotederMapper;
            BundlesCounter = bundlesCounter;
            RawMapper = rawMapper;
        }

        public void Dispose()
        {
            int id = MapperID;
            if (RawMapper.ContainsKey(id))
            {
                Object raw = RawMapper[id];
                RawMapper.Remove(id);

                id = raw.GetInstanceID();
                if (QuotederMapper.ContainsKey(id))
                {
                    QuotederMapper.Remove(id);
                    CleanAllAsset();
                    if (BundlesCounter.ContainsKey(ABName) && BundlesCounter[ABName] > 0)
                    {
                        BundlesCounter[ABName]--;
                    }
                    else { }
                }
                else { }
            }
            else { }

            mRaw = default;
            RawMapper = default;
            QuotederMapper = default;
            AssetMapper = default;
            BundlesCounter = default;
            Count = 0;
        }

        private void CleanAllAsset()
        {
            List<Object> keys = AssetMapper.Keys;
            List<AssetQuoteder> list = AssetMapper.Values;
            List<Object> deletes = new List<Object>();
            int max = list.Count;
            for (int i = 0; i < max; i++)
            {
                if (list[i].MapperID == MapperID)
                {
                    deletes.Add(keys[i]);
                }
                else { }
            }
            Object asset;
            max = deletes.Count;
            for (int i = 0; i < max; i++)
            {
                asset = deletes[i];
                if (asset != default)
                {
#if LOG_ASSET_QUOTEDER
                    "log:Asset quoteder {0} clean all asset, ({1})".Log(mRaw.name, asset.name);
#endif
                    AssetMapper.Remove(asset);
                    Object.Destroy(asset);
                }
                else { }
            }
        }

        public void SetRaw(int id, ref string abName, ref string assetName, Object value)
        {
            if (mRaw == default)
            {
                MapperID = id;
                mRaw = value;
                ABName = abName;
                AssetName = assetName;
                Count = 0;

                int rawID = mRaw.GetInstanceID();
                QuotederMapper[rawID] = this;
            }
            else { }
        }

        public T Instantiate<T>() where T : Object
        {
            T result = default;
            bool flag = mRaw != default;
            if (flag)
            {
                result = Object.Instantiate((T)mRaw);
                AssetMapper[result] = this;
                Count++;

                AddAssetBundleQuoted();
#if LOG_ASSET_QUOTEDER
                "log:Get asset from quote {0}, total is {1}".Log(mRaw.name, Count.ToString());
#endif
            }
            else { }
            return result;
        }

        private void AddAssetBundleQuoted()
        {
            if (BundlesCounter.ContainsKey(ABName))
            {
                BundlesCounter[ABName]++;
            }
            else
            {
                BundlesCounter[ABName] = 1;
            }
#if LOG_ASSET_QUOTEDER
            "log:Asset bundle {0} quoted increaced to {1}".Log(ABName, BundlesCounter[ABName].ToString());
#endif
        }

        private void RemoveAssetBundleQuoted()
        {
            if (BundlesCounter.ContainsKey(ABName))
            {
                int value = BundlesCounter[ABName];
                if (value > 0)
                {
                    value--;
                    BundlesCounter[ABName] = value;
#if LOG_ASSET_QUOTEDER
                    "log:Asset bundle {0} quoted reduced to {1}".Log(ABName, value.ToString());
#endif
                }
                else { }
            }
            else { }
        }

        public void Destroy(Object target, bool checkIsExists = true, bool isAutoDispose = false)
        {
            if (checkIsExists)
            {
                if (AssetMapper != default)
                {
                    bool flag = AssetMapper.Keys.Contains(target);
#if LOG_ASSET_QUOTEDER
                    "error:Asset {0} do not contains in quoteder, raw name is {1}".Log(!flag, target.name, mRaw.name);
#endif
                    if (!flag)
                    {
                        return;
                    }
                    else { }
                }
                else { }
            }
            else { }
            AssetMapper.Remove(target);
#if LOG_ASSET_QUOTEDER
            "log".Log("Destroy asset".Append(target.name, " count remains ", Count.ToString()));
#endif
            Object.Destroy(target);
            RemoveAssetBundleQuoted();
            Count--;
            if (Count <= 0)
            {
                if (isAutoDispose)
                {
                    Dispose();
                }
                else { }
            }
            else { }
        }
    }
}
