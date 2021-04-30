using ShipDock.Notices;
using ShipDock.Versioning;
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
        public string assetNameResData = "res_data";
        [Tooltip("位于Resource目录的首屏加载弹窗资源路径")]
        public string resUpdatePopupPath = "res_update_popup/ResUpdatePopup";
        [Tooltip("加载资源时是否自动识别使用本地缓存还是Streaming目录")]
        public bool applyManifestAutoPath = false;
        [Tooltip("是否启动IOC功能")]
        public bool startUpIOC = false;
        [Tooltip("是否包含多语言本地化配置")]
        public bool hasLocalsConfig = false;
        [Tooltip("是否包含多语言本地化配置")]
        public bool isDeletePlayerPref;
        [Tooltip("远程资源客户端配置")]
        public ClientResVersion remoteAssetVersions;
        [Tooltip("预加载的配置列表")]
        public string[] configNames;
        [Tooltip("预加载的资源列表")]
        public string[] assetNamePreload;
        [Tooltip("服务容器子组")]
        public ServerContainerSubgroup loadConfig;

        /// <summary>是否应用远程资源服务器</summary>
        public bool ApplyRemoteAssets
        {
            get
            {
                return remoteAssetVersions != default;
            }
        }
    }
}