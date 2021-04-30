using Sirenix.OdinInspector;
using UnityEngine;

namespace ShipDock.Applications
{
    public class ParticalsCollecter : MonoBehaviour
    {
        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("生存时间")]
#endif
        private float m_Time;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("生存时间结束后自动销毁"), HideIf("@this.m_ApplyPool || this.m_ApplyEffectPool")]
#endif
        private bool m_ApplyDestroy;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("启用对象池模式"), HideIf("@this.m_ApplyEffectPool || this.m_ApplyDestroy")]
#endif
        private bool m_ApplyPool;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("启用特效池模式"), HideIf("@this.m_ApplyPool || this.m_ApplyDestroy")]
#endif
        private bool m_ApplyEffectPool;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("对象池 id"), ShowIf("@this.m_ApplyPool || this.m_ApplyEffectPool")]
#endif
        private int m_PoolName;
        [SerializeField]

#if ODIN_INSPECTOR
        [ReadOnly, LabelText("剩余时间")]
#endif
        private float m_TimeUpdating;

#if ODIN_INSPECTOR
        [ReadOnly]
#endif
        private bool mWillUpdate;

#if ODIN_INSPECTOR
        [ReadOnly]
#endif
        private bool mWillStartTime;

        private void Start()
        {
            mWillUpdate = true;
            m_TimeUpdating = m_Time;
        }

        public void SetCollectTime(float time)
        {
            m_Time = time;
        }

        public void SetPoolName(int poolName, bool isEffectPool)
        {
            m_PoolName = poolName;
            m_ApplyEffectPool = isEffectPool;
            m_ApplyPool = !isEffectPool;

            m_ApplyDestroy = false;
        }

        private void OnWillRenderObject()
        {
            if (!mWillUpdate && enabled)
            {
                mWillStartTime = true;
            }
            else { }
        }

        private void Update()
        {
            if (mWillStartTime)
            {
                mWillUpdate = true;
                m_TimeUpdating = m_Time;

                mWillStartTime = false;
            }
            else { }

            if (m_TimeUpdating > 0f && mWillUpdate)
            {
                m_TimeUpdating -= Time.deltaTime;
                if (m_TimeUpdating <= 0f)
                {
                    m_TimeUpdating = 0f;
                    mWillUpdate = false;

                    gameObject.SetActive(false);
                    if (m_ApplyDestroy)
                    {
                        Destroy(gameObject);
                    }
                    else
                    {
                        if (m_ApplyPool)
                        {
                            ShipDockApp.Instance.AssetsPooling.ToPool(m_PoolName, gameObject);
                        }
                        else if (m_ApplyEffectPool)
                        {
                            ShipDockApp.Instance.Effects.CollectEffect(m_PoolName, gameObject);
                        }
                        else { }
                    }
                }
                else { }
            }
            else { }
        }
    }
}
