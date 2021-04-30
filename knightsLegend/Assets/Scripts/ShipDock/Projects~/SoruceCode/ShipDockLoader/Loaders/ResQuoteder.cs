using ShipDock.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Loader
{
    public class ResQuoteder
    {

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
                "log:New quote {0}, id is {1}, ab name is {2}, asset name is {3}".Log(
                    typeof(T).Name, id.ToString(), abName, assetName);
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
                    if (isCustome)
                    {
                        "error: {0} is using, count is {1}".Log(key, bundlesCounter[key].ToString());
                    }
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
