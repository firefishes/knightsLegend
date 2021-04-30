using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Tools;
using System;

namespace ShipDock.Notices
{
    public class NoticeManagerBase<T> : Singletons<NoticeManagerBase<T>>
    {
        public Notifications<T> Notificater { get; private set; }

        public NoticeManagerBase()
        {
            Notificater = new Notifications<T>();
        }

        public override void Dispose()
        {
            base.Dispose();

            Notificater.Dispose();
        }
    }

    public class NotificatonsInt : NoticeManagerBase<int> { }

}

public static class NoticesExtensions
{
    public static void Add(this int target, Action<INoticeBase<int>> handler)
    {
        NotificatonsInt.Instance.Notificater?.Add(target, handler);
    }

    public static void Add(this INotificationSender target, Action<INoticeBase<int>> handler)
    {
        NotificatonsInt.Instance.Notificater?.Add(target, handler);
    }

    public static void Remove(this int target, Action<INoticeBase<int>> handler)
    {
        NotificatonsInt.Instance.Notificater?.Remove(target, handler);
    }

    public static void Remove(this INotificationSender target, Action<INoticeBase<int>> handler)
    {
        NotificatonsInt.Instance.Notificater?.Remove(target, handler);
    }

    public static void Broadcast(this int noticeName, INoticeBase<int> notice = default)
    {
        bool defaultNotice = notice == default;
        if (defaultNotice)
        {
            notice = new Notice();
        }
        else { }

        notice.SetNoticeName(noticeName);
        NotificatonsInt.Instance.Notificater?.Broadcast(notice);
        if (defaultNotice)
        {
            notice.Dispose();
        }
        else { }
    }

    public static void BroadcastWithParam<T>(this int noticeName, T vs)
    {
        ParamNotice<T> notice = new ParamNotice<T>
        {
            ParamValue = vs
        };
        notice.SetNoticeName(noticeName);
        NotificatonsInt.Instance.Notificater?.Broadcast(notice);
        notice.Dispose();
    }

    public static void Dispatch(this INotificationSender target, INoticeBase<int> notice)
    {
        notice.NotifcationSender = target;
        NotificatonsInt.Instance.Notificater.Dispatch(notice);
    }

    public static void Dispatch<T>(this INotificationSender target, int noticeName, T vs)
    {
        ParamNotice<T> notice = new ParamNotice<T>
        {
            ParamValue = vs
        };
        notice.SetNoticeName(noticeName);
        notice.NotifcationSender = target;

        NotificatonsInt.Instance.Notificater.Dispatch(notice);

        notice.Dispose();
    }

    public static void Dispatch(this INotificationSender target, int noticeName, INoticeBase<int> notice = default)
    {
        bool defaultNotice = notice == default;
        if (defaultNotice)
        {
            notice = new Notice();
        }
        else { }

        notice.SetNoticeName(noticeName);
        notice.NotifcationSender = target;

        NotificatonsInt.Instance.Notificater.Dispatch(notice);

        if (defaultNotice)
        {
            notice.Dispose();
        }
        else { }
    }
}