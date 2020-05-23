using ShipDock.Applications;
using ShipDock.Datas;
using ShipDock.Notices;
using ShipDock.Server;
using System;

public static class ShipDockAppExtension
{
    public static void Add(this int target, Action<INoticeBase<int>> handler)
    {
        ShipDockApp.Instance.Notificater?.Add(target, handler);
    }

    public static void Add(this INotificationSender target, Action<INoticeBase<int>> handler)
    {
        ShipDockApp.Instance.Notificater?.Add(target, handler);
    }

    public static void Remove(this int target, Action<INoticeBase<int>> handler)
    {
        ShipDockApp.Instance.Notificater?.Remove(target, handler);
    }

    public static void Remove(this INotificationSender target, Action<INoticeBase<int>> handler)
    {
        ShipDockApp.Instance.Notificater?.Remove(target, handler);
    }

    public static void Broadcast(this int noticeName, INoticeBase<int> notice = default)
    {
        bool defaultNotice = notice == default;
        if (defaultNotice)
        {
            notice = new Notice();
        }
        notice.SetNoticeName(noticeName);
        ShipDockApp.Instance.Notificater?.Broadcast(notice);
        if(defaultNotice)
        {
            notice.Dispose();
        }
    }

    public static void Dispatch(this INotificationSender target, INoticeBase<int> notice)
    {
        notice.NotifcationSender = target;
        ShipDockApp.Instance.Notificater.Dispatch(notice);
    }

    public static void Dispatch(this INotificationSender target, int noticeName, INoticeBase<int> notice)
    {
        notice.SetNoticeName(noticeName);
        notice.NotifcationSender = target;
        ShipDockApp.Instance.Notificater.Dispatch(notice);
    }
    
    public static T GetServer<T>(this string serverName) where T : IServer
    {
        return ShipDockApp.Instance.Servers.GetServer<T>(serverName);
    }

    public static void MakeResolver<I>(this string serverName, string alias, string resolverName, ResolveDelegate<I> handler)
    {
        serverName.GetServer<IServer>().MakeResolver(alias, resolverName, handler);
    }

    public static void AddToWarehouse(this IData target)
    {
        ShipDockApp.Instance.Datas.AddData(target);
    }

    public static T GetData<T>(this int target) where T : IData
    {
        return ShipDockApp.Instance.Datas.GetData<T>(target);
    }
}
