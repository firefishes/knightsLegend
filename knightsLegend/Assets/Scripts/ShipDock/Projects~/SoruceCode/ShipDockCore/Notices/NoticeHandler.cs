using ShipDock.Interfaces;
using System;
using UnityEngine;

namespace ShipDock.Notices
{
    public class NoticeHandler<NameT> : IDispose
    {
        public static NoticeHandler<NameT> Create(NameT name, INotificationSender sender = default)
        {
            NoticeHandler<NameT> result = new NoticeHandler<NameT>
            {
                Name = name,
                Sender = sender
            };
            return result;
        }
        
        private Action<INoticeBase<NameT>> mHandler;

        public NoticeHandler()
        {
            NoticeCount = 0;
            Action<INoticeBase<NameT>> newDelegate = null;
            mHandler = newDelegate;
        }

        public void Add(Action<INoticeBase<NameT>> handler)
        {
            if (handler == null)
            {
                return;
            }
            mHandler += handler;
            NoticeCount++;
        }

        public void Remove(Action<INoticeBase<NameT>> handler)
        {
            if (handler == null)
            {
                return;
            }
            mHandler -= handler;
            NoticeCount--;
            NoticeCount = Mathf.Max(0, NoticeCount);
        }

        public void Invoke(ref INoticeBase<NameT> param)
        {
            mHandler?.Invoke(param);
        }

        public void Dispose()
        {
            NoticeCount = 0;

            mHandler = null;
            Name = default;
            Sender = default;
        }

        public NameT Name { get; set; }
        public int NoticeCount { get; private set; }
        public INotificationSender Sender { get; set; }
    }

}
