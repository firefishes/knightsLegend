#define G_LOG

using ShipDock.Applications;
using ShipDock.Datas;
using ShipDock.Notices;
using ShipDock.Server;
using ShipDock.Testers;
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

    public static void MakeResolver<I>(this string serverName, string alias, string resolverName, ResolveDelegate<I> handler)
    {
        serverName.GetServer().MakeResolver(alias, resolverName, handler);
    }

    public static void AddToWarehouse(this IDataProxy target)
    {
        ShipDockApp.Instance.Datas.AddData(target);
    }

    public static T GetData<T>(this int target) where T : IDataProxy
    {
        return ShipDockApp.Instance.Datas.GetData<T>(target);
    }

    [System.Diagnostics.Conditional("G_LOG")]
    public static void Log(this string logID, params string[] args)
    {
        Tester.Instance.Log(logID, args);
    }

    [System.Diagnostics.Conditional("G_LOG")]
    public static void Log(this string target, bool logFilters, params string[] args)
    {
        Tester.Instance.Log(target, logFilters, args);
    }

    [System.Diagnostics.Conditional("G_LOG")]
    public static void AssertLog(this string target, string title, string assertTarget, params string[] args)
    {
        Tester.Instance.LogAndAssert(target, title, assertTarget, args.Length == 0 ? new string[] { assertTarget } : args);
    }

    [System.Diagnostics.Conditional("G_LOG")]
    public static void AssertLog(this string target, bool logFilters, string title, string assertTarget, params string[] args)
    {
        if (logFilters)
        {
            target.AssertLog(title, assertTarget, args);
        }
    }

    [System.Diagnostics.Conditional("G_LOG")]
    public static void Assert(this string target, string assertTarget, params string[] args)
    {
        Tester.Instance.Asserting(target, assertTarget);
        if (args.Length > 0)
        {
            target.Log(args);
        }
    }
}
