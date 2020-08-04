using ShipDock.Interfaces;
using ShipDock.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{
    public class EffectManager : IDispose
    {
        private class Effect
        {
            public int total;
            public ResPrefabBridge source;

            public void Init()
            {
                Surplus = total;
                UniqueCache = new List<GameObject>();
            }

            public void Clean()
            {
                total = 0;
                Surplus = 0;
                source = default;
                UniqueCache = default;

                List<GameObject> list = UniqueCache;
                Utils.Reclaim(ref list);
            }

            public void CreateAndFill(out GameObject result, bool isFromPool = false)
            {
                result = default;
                if (ShouldCreate())
                {
                    Surplus--;
                    result = source.CreateAsset(isFromPool);
                    UniqueCache.Add(result);
                }
            }

            public bool ShouldCreate()
            {
                return Surplus > 0;
            }

            internal GameObject GetUniqueCache()
            {
                CacheIndex = Math.Max(CacheIndex, 0);
                CacheIndex = Math.Min(CacheIndex, UniqueCache.Count - 1);
                int index = CacheIndex;
                CacheIndex++;
                return UniqueCache[index];
            }

            private int CacheIndex { get; set; }
            private List<GameObject> UniqueCache { get; set; }

            public int Surplus { get; private set; }

            internal void CollectEffect(GameObject target)
            {
                UniqueCache.Remove(target);
                source.CollectAsset(target);
            }
        }

        private KeyValueList<int, Effect> mPrefabRaw;

        public EffectManager() : base()
        {
            mPrefabRaw = new KeyValueList<int, Effect>();
        }

        public void Dispose()
        {
            int max = mPrefabRaw != default ? mPrefabRaw.Size : 0;
            if (max > 0)
            {
                List<Effect> list = mPrefabRaw.Values;
                for (int i = 0; i < max; i++)
                {
                    list[i].Clean();
                }
            }

            Utils.Reclaim(ref mPrefabRaw);
        }

        public bool HasEffectRaw(int id)
        {
            return mPrefabRaw.ContainsKey(id);
        }

        public void CreateSource(int id, ref ResPrefabBridge source, int total, int preCreate = 0)
        {
            Effect effect;
            if (mPrefabRaw.ContainsKey(id))
            {
                effect = mPrefabRaw[id];
            }
            else
            {
                effect = new Effect
                {
                    source = source,
                    total = total
                };
                effect.Init();
                mPrefabRaw[id] = effect;
            }
            int max = preCreate;
            for (int i = 0; i < max; i++)
            {
                CreateEffect(id, out _);
            }
        }

        public void FillFromSource(int id, ref ResPrefabBridge source)
        {
            if (mPrefabRaw.ContainsKey(id))
            {
                source = mPrefabRaw[id].source;
            }
        }

        public void CreateEffect(int id, out GameObject result)
        {
            result = default;
            if (mPrefabRaw.ContainsKey(id))
            {
                Effect effect = mPrefabRaw[id];
                if (effect.ShouldCreate())
                {
                    effect.CreateAndFill(out result);
                }
                else
                {
                    result = effect.GetUniqueCache();
                }
            }
        }

        public void CollectEffect(int id, GameObject target)
        {
            if (mPrefabRaw.ContainsKey(id))
            {
                Effect effect = mPrefabRaw[id];
                effect.CollectEffect(target);
            }
            else
            {
                UnityEngine.Object.Destroy(target);
            }
        }

        public void RemoveEffectRaw(int id, ref ResPrefabBridge source)
        {
            if (mPrefabRaw.ContainsKey(id))
            {
                Effect effect = mPrefabRaw.Remove(id);
                effect.Clean();
            }
        }
    }
}