using ShipDock.Interfaces;
using ShipDock.Tools;
using System;

namespace ShipDock.Notices
{

    public class HandlerMapper<NameT> : KeyValueList<NameT, NoticeHandler<NameT>>
    {
        public void Reclaim()
        {
            KeyValueList<NameT, NoticeHandler<NameT>> mapper = this;
            Utils.Reclaim(ref mapper, true, true);
        }
    }

    public class HandlerWithSenderMapper<NameT> : KeyValueList<INotificationSender, NoticeHandler<NameT>>
    {
        public void Reclaim()
        {
            KeyValueList<INotificationSender, NoticeHandler<NameT>> mapper = this;
            Utils.Reclaim(ref mapper, true, true);
        }
    }

    /// <summary>
    /// 
    /// 消息控制器
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class Notifications<NameT> : IDispose, INotificationSender
    {
        #region 私有属性
        private HandlerMapper<NameT> mNoticeHandlers;
        private HandlerWithSenderMapper<NameT> mHandlersWithSender;
        #endregion

        #region 构造函数
        public Notifications()
        {
            mNoticeHandlers = new HandlerMapper<NameT>();
            mHandlersWithSender = new HandlerWithSenderMapper<NameT>();
        }
        #endregion

        #region 销毁
        public void Dispose()
        {
            mNoticeHandlers?.Reclaim();
            mHandlersWithSender?.Reclaim();
        }
        #endregion

        public void Add(NameT name, Action<INoticeBase<NameT>> handler)
        {
            if (mNoticeHandlers == null)
            {
                return;
            }
            NoticeHandler<NameT> noticeHolder;
            if (!mNoticeHandlers.IsContainsKey(name))
            {
                noticeHolder = NoticeHandler<NameT>.Create(name);
                mNoticeHandlers[name] = noticeHolder;
            }
            else
            {
                noticeHolder = mNoticeHandlers[name];
            }
            noticeHolder.Add(handler);
        }
        
        public void Add(INotificationSender sender, Action<INoticeBase<NameT>> handler)
        {
            if (mHandlersWithSender == null)
            {
                return;
            }
            NoticeHandler<NameT> noticeHolder;
            if (!mHandlersWithSender.IsContainsKey(sender))
            {
                noticeHolder = NoticeHandler<NameT>.Create(default, sender);
                mHandlersWithSender[sender] = noticeHolder;
            }
            else
            {
                noticeHolder = mHandlersWithSender[sender];
            }
            noticeHolder.Add(handler);
        }

        public void Remove(NameT name, Action<INoticeBase<NameT>> handler)
        {
            if (mNoticeHandlers == null)
            {
                return;
            }
            if (mNoticeHandlers.IsContainsKey(name))
            {
                NoticeHandler<NameT> noticeHolder = mNoticeHandlers[name];
                noticeHolder.Remove(handler);
                if (noticeHolder.NoticeCount <= 0)
                {
                    mNoticeHandlers.Remove(name);
                    noticeHolder.Dispose();
                }
            }
        }
        
        public void Remove(INotificationSender sender, Action<INoticeBase<NameT>> handler)
        {
            if (mHandlersWithSender == null)
            {
                return;
            }
            if (mHandlersWithSender.IsContainsKey(sender))
            {
                NoticeHandler<NameT> noticeHolder = mHandlersWithSender[sender];
                noticeHolder.Remove(handler);
                if (noticeHolder.NoticeCount <= 0)
                {
                    mHandlersWithSender.Remove(sender);
                    noticeHolder.Dispose();
                }
            }
        }
        
        public void Broadcast(INoticeBase<NameT> notice)
        {
            if ((notice == default) || (mNoticeHandlers == default))
            {
                return;
            }
            NameT noticeName = notice.Name;
            if(mNoticeHandlers.IsContainsKey(noticeName))
            {
                NoticeHandler<NameT> noticeHolder = mNoticeHandlers[noticeName];
                noticeHolder.Invoke(ref notice);
            }
        }

        public void Dispatch(INoticeBase<NameT> notice)
        {
            if ((notice == default) || 
                (mHandlersWithSender == default) || 
                (notice.NotifcationSender == default))
            {
                return;
            }
            INotificationSender sender = notice.NotifcationSender;
            if (mHandlersWithSender.IsContainsKey(sender))
            {
                NoticeHandler<NameT> noticeHolder = mHandlersWithSender[sender];
                noticeHolder.Invoke(ref notice);
            }
        }

    }
}