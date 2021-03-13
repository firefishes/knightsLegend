using System;
using UnityEngine;
using UnityEngine.Events;

namespace ShipDock.Notices
{
    [Serializable]
    public class NoticeHandlerEvent : UnityEvent<INoticeBase<int>> { }

    [Serializable]
    public class NotificationInfo
    {
        [SerializeField]
        private int m_Notice = int.MaxValue;
        
        [SerializeField]
        private NoticeHandlerEvent m_NoticeEvent = new NoticeHandlerEvent();

        public void Init()
        {
            NotificatonsInt.Instance.Notificater.Add(m_Notice, OnNoticeListenerHandler);
        }

        public void Deinit()
        {
            NotificatonsInt.Instance.Notificater.Remove(m_Notice, OnNoticeListenerHandler);
        }

        private void OnNoticeListenerHandler(INoticeBase<int> param)
        {
            m_NoticeEvent?.Invoke(param);
        }

        public int NoticeName
        {
            get
            {
                return m_Notice;
            }
        }
    }
}