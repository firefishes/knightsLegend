using UnityEngine;

namespace ShipDock.Applications
{
    public class ParticalsCollecter : MonoBehaviour
    {
        [SerializeField]
        private float m_Time;
        [SerializeField]
        private bool m_ApplyPool;
        [SerializeField]
        private bool m_ApplyEffectPool;
        [SerializeField]
        private bool m_ApplyDestroy;
        [SerializeField]
        private int m_PoolName;
        [SerializeField]
        private float m_TimeUpdating;

        private bool mWillUpdate;

        private void Start()
        {
            mWillUpdate = true;
            m_TimeUpdating = m_Time;
        }

        private void OnWillRenderObject()
        {
            if (!mWillUpdate)
            {
                mWillUpdate = true;
                m_TimeUpdating = m_Time;
            }
        }

        private void Update()
        {
            if (m_TimeUpdating > 0f && mWillUpdate)
            {
                m_TimeUpdating -= Time.deltaTime;
                if (m_TimeUpdating <= 0f)
                {
                    m_TimeUpdating = 0f;
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
                        mWillUpdate = false;
                        gameObject.SetActive(false);
                    }
                }
                else { }
            }
            else { }
        }
    }
}
