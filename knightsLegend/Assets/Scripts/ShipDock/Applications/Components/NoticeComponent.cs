using System.Collections.Generic;
using ShipDock.Infos;
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
            int max = m_Notices.Count;
            for (int i = 0; i < max; i++)
            {
                m_Notices[i].Init();
            }
        }

        private void OnDestroy()
        {
            int max = m_Notices.Count;
            for (int i = 0; i < max; i++)
            {
                m_Notices[i].Deinit();
            }
        }
    }
}
