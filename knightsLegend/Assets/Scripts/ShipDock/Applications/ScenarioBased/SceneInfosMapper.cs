using ShipDock.Tools;
using UnityEngine;

namespace ShipDock.Applications
{

    public abstract class SceneInfosMapper<K, V> : KeyValueList<K, V>
    {
        [SerializeField]
        protected bool m_DisposeSkillInfos;

        [SerializeField]
        public V[] infos;

        public virtual void InitMapper()
        {
            V info;
            int max = infos.Length;
            for (int i = 0; i < max; i++)
            {
                info = infos[i];
                Put(GetInfoKey(ref info), info);
                AfterInitItem(ref info);
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            Utils.Reclaim(ref infos, true, m_DisposeSkillInfos);
        }

        protected virtual void AfterInitItem(ref V item) { }

        public abstract K GetInfoKey(ref V item);

    }
}