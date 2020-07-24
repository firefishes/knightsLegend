using ShipDock.Notices;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace ShipDock.Infos
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
            m_Notice.Add(OnNoticeListenerHandler);
        }

        public void Deinit()
        {
            m_Notice.Remove(OnNoticeListenerHandler);
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