using ShipDock.Datas;
using ShipDock.Notices;
using ShipDock.Server;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ShipDock.Applications
{

    [Serializable]
    internal class GetDataProxyEvent : UnityEvent<IParamNotice<IDataProxy[]>> { }

    [Serializable]
    internal class GetLocalsConfigItemNotice : UnityEvent<Dictionary<int, string>, IConfigNotice> { }

    [Serializable]
    internal class GetServerConfigsEvent : UnityEvent<IParamNotice<IResolvableConfig[]>> { }

    [Serializable]
    internal class InitProfileEvent : UnityEvent<IParamNotice<int[]>> { }

    [Serializable]
    internal class GetGameServersEvent : UnityEvent<IParamNotice<IServer[]>> { }

    [Serializable]
    internal class InitProfileDataEvent : UnityEvent<IConfigNotice> { }

    [Serializable]
    internal class ShipDockCloseEvent : UnityEvent { }

    [Serializable]
    public class GameApplicationEvents
    {
        [SerializeField]
        [Header("创建测试驱动器事件")]
        internal UnityEvent createTestersEvent = new UnityEvent();
        [SerializeField]
        [Header("获取游戏服务容器事件")]
        internal GetGameServersEvent getGameServersEvent = new GetGameServersEvent();
        [SerializeField]
        [Header("服务容器就绪事件")]
        internal UnityEvent serversFinishedEvent = new UnityEvent();
        [SerializeField]
        [Header("用户对象初始化事件")]
        internal InitProfileEvent initProfileEvent = new InitProfileEvent();
        [SerializeField]
        [Header("用户数据初始化事件")]
        internal InitProfileDataEvent initProfileDataEvent = new InitProfileDataEvent();
        [SerializeField]
        [Header("获取服务容器解析配置事件")]
        internal GetServerConfigsEvent getServerConfigsEvent = new GetServerConfigsEvent();
        [SerializeField]
        [Header("获取多语言本地化配置项事件，用于对多语言映射数据赋值")]
        internal GetLocalsConfigItemNotice getLocalsConfigItemEvent = new GetLocalsConfigItemNotice();
        [SerializeField]
        [Header("游戏进入事件")]
        internal UnityEvent enterGameEvent = new UnityEvent();
        [SerializeField]
        [Header("数据代理初始化事件")]
        internal GetDataProxyEvent getDataProxyEvent = new GetDataProxyEvent();
        [SerializeField]
        [Header("框架关闭事件")]
        internal ShipDockCloseEvent frameworkCloseEvent = new ShipDockCloseEvent();
    }

}