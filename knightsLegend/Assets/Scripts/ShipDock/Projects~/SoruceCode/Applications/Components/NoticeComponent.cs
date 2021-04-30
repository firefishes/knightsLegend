using ShipDock.Notices;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{

    public class NoticeComponent : MonoBehaviour
    {
        [SerializeField]
        private List<NotificationInfo> m_Notices;

        private bool mIsApplicationExited;

        private void Awake()
        {
            int max = m_Notices == default ? 0 : m_Notices.Count;
            for (int i = 0; i < max; i++)
            {
                m_Notices[i].Init();
            }
        }

        private void OnDestroy()
        {
            int max = m_Notices == default ? 0 : m_Notices.Count;
            for (int i = 0; i < max; i++)
            {
                m_Notices[i].Deinit();
            }
        }
    }
}
