
using ShipDock.Applications;
using ShipDock.Datas;
using ShipDock.Notices;
using ShipDock.Server;
using ShipDock.Testers;
using System;
using UnityEngine;

public static class ShipDockAppExtension
{
#if ADD
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

    public static void Dispatch(this INotificationSender target, int noticeName, INoticeBase<int> notice = default)
    {
        bool defaultNotice = notice == default;
        if (defaultNotice)
        {
            notice = new Notice();
        }
        notice.SetNoticeName(noticeName);
        notice.NotifcationSender = target;
        ShipDockApp.Instance.Notificater.Dispatch(notice);
        if (defaultNotice)
        {
            notice.Dispose();
        }
    }
    
    public static IServer GetServer(this string serverName)
    {
        return ShipDockApp.Instance.Servers.GetServer<IServer>(serverName);
    }
#endif

    public static void MakeResolver<I>(this string serverName, string alias, string resolverName, ResolveDelegate<I> handler)
    {
        serverName.GetServer().MakeResolver(alias, resolverName, handler);
    }

    public static string Language(this string target, params string[] args)
    {
        return ShipDockApp.Instance.Locals.Language(target, args);
    }

    public static GameObject Create(this GameObject target, int poolID = int.MaxValue, bool selfActive = true)
    {
        if (poolID != int.MaxValue)
        {
            return ShipDockApp.Instance.AssetsPooling.FromPool(poolID, ref target, default, selfActive);
        }
        else
        {
            return UnityEngine.Object.Instantiate(target);
        }
    }

    public static void Terminate(this GameObject target, int poolID = int.MaxValue)
    {
        if (poolID == int.MaxValue)
        {
            UnityEngine.Object.Destroy(target);
        }
        else
        {
            ShipDockApp.Instance.AssetsPooling.ToPool(poolID, target);
        }
    }
}
