using System;
using UnityEngine;

namespace ShipDock.Applications
{
    [Serializable]
    public class DevelopSubgroup
    {
        [Tooltip("配置初始化完成的消息名")]
        public int configInitedNoticeName = ShipDockConsts.NOTICE_CONFIG_PRELOADED;
        [Tooltip("资源总依赖清单所在的ab包文件")]
        public string assetNameResData = "res_data/res_data";
        [Tooltip("是否包含多语言本地化配置")]
        public bool hasLocalsConfig = false;
        [Tooltip("预加载的配置列表")]
        public string[] configNames;
        [Tooltip("预加载的资源列表")]
        public string[] assetNamePreload;
        [Tooltip("服务容器子组")]
        public ServerContainerSubgroup loadConfig;
    }
}