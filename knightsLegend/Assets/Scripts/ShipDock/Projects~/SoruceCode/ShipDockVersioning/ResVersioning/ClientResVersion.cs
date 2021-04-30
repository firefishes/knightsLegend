using ShipDock.Applications;
using ShipDock.Loader;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Versioning
{
    [Serializable]
    public class RemoteGatewayItem
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf("selected", true)]
#endif
        public string name;
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ToggleGroup("selected", "$name")]
#endif
        public bool selected;
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf("selected", true)]
#endif
        public string gateway;
    }

    /// <summary>
    /// 客户端安装包资源版本配置数据对象
    /// </summary>
    [CreateAssetMenu(fileName = "ClientResVersions", menuName = "ShipDock : 客户端资源版本", order = 100)]
    public class ClientResVersion : ScriptableObject
    {
        [Header("客户端安装包默认的资源版本配置")]
        [SerializeField]
        [Tooltip("子服务名，用于在同一个客户端下获取不同服务器的资源")]
        private string m_RemoteName = string.Empty;
        [SerializeField]
        private bool m_ApplyCurrentResGateway;
        [SerializeField]
        private string m_ResRemoteGateway;
        [SerializeField]
        private ResDataVersion m_Res;

        #region 编辑器扩展相关
#if UNITY_EDITOR
        [Header("以下仅用于编辑器")]
        [SerializeField]
        private ResVersion[] m_ResChanged;
        [SerializeField]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf("m_ApplyCurrentResGateway", true)]
#endif
        private RemoteGatewayItem[] m_OptionalGateways;

        [SerializeField]
        private TextAsset m_Preview = default;
        private ResDataVersion mBeforePreview;
        private bool mIsShowPreview = false;

        private int mRemoteSelected = -1;
        private bool mAskForDeletePersistent = false;
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf("m_ApplyCurrentResGateway", true)]
        [Sirenix.OdinInspector.Button(name: "应用 Gateway")]
        private void UpdateRemoteGatewaySelected()
        {
            mRemoteSelected = -1;
            int max = m_OptionalGateways.Length;
            for (int i = 0; i < max; i++)
            {
                if (m_OptionalGateways[i].selected)
                {
                    mRemoteSelected = i;
                    break;
                }
            }
            max = m_OptionalGateways.Length;
            for (int i = 0; i < max; i++)
            {
                if (mRemoteSelected != i)
                {
                    m_OptionalGateways[i].selected = false;
                }
            }
            if (mRemoteSelected != -1)
            {
                m_ResRemoteGateway = m_OptionalGateways[mRemoteSelected].gateway;
                
            }
        }

        [Sirenix.OdinInspector.Button(name: "预览其他")]
        [Sirenix.OdinInspector.ShowIf("@this.mIsShowPreview == false && m_Preview != null")]
        private void PreviewVersionsData()
        {
            if (m_Preview != default)
            {
                mBeforePreview = m_Res;
                mIsShowPreview = true;
                m_Res = JsonUtility.FromJson<ResDataVersion>(m_Preview.text);
            }
        }

        [Sirenix.OdinInspector.Button(name: "返回")]
        [Sirenix.OdinInspector.ShowIf("@this.mIsShowPreview == true")]
        private void ClosePreviewVersions()
        {
            mIsShowPreview = false;
            if (mBeforePreview != default)
            {
                m_Res = mBeforePreview;
            }
        }

        [Sirenix.OdinInspector.Button(name: "清空缓存资源")]
        [Sirenix.OdinInspector.ShowIf("@this.mAskForDeletePersistent == false")]
        private void WillDeletePersistent()
        {
            mAskForDeletePersistent = true;
        }

        [Sirenix.OdinInspector.ButtonGroup("是否清空缓存资源？")]
        [Sirenix.OdinInspector.Button(name: "取消")]
        [Sirenix.OdinInspector.ShowIf("@this.mAskForDeletePersistent == true")]
        private void CancelDeletePersistent()
        {
            mAskForDeletePersistent = false;
        }

        [Sirenix.OdinInspector.ButtonGroup("是否清空缓存资源？")]
        [Sirenix.OdinInspector.Button(name: "确定")]
        [Sirenix.OdinInspector.ShowIf("@this.mAskForDeletePersistent == true")]
        private void ConfirmDeletePersistent()
        {
            mAskForDeletePersistent = false;
            ClearPersistent();
        }
#endif

        public void SetChanges(ResVersion[] resChanges)
        {
            m_ResChanged = resChanges;
        }

        private void OnEnable()
        {
#if ODIN_INSPECTOR
            if (mIsShowPreview && mBeforePreview != default && m_Preview == default)
            {
                ClosePreviewVersions();
            }
#endif
        }
#endif
        #endregion

        private int mRemoteResVersion;

        public bool ApplyCurrentResGateway
        {
            get
            {
                return m_ApplyCurrentResGateway;
            }
        }

        /// <summary>本地缓存的资源版本配置</summary>
        public ResDataVersion CachedVersion { get; private set; }
        /// <summary>单个资源更新完成的回调函数</summary>
        public Action<bool, float> UpdateHandler { get; private set; }
        /// <summary>单个资源更新完成的回调函数</summary>
        public Func<bool> VersionInvalidHandler { get; private set; }

        public int UpdatingLoaded
        {
            get
            {
                return CachedVersion != default ? CachedVersion.UpdatingLoaded : 0;
            }
        }

        public int UpdatingMax
        {
            get
            {
                return CachedVersion != default ? CachedVersion.UpdatingMax : 0;
            }
        }

        public string Source
        {
            get
            {
                return JsonUtility.ToJson(m_Res);
            }
            set
            {
                m_Res = JsonUtility.FromJson<ResDataVersion>(value);
                m_Res.resVersionType = ResDataVersionType.Client;
                SyncResGateway();
            }
        }

        public ResDataVersion Versions
        {
            get
            {
#if UNITY_EDITOR && ODIN_INSPECTOR
                ClosePreviewVersions();
#endif
                if (m_Res == default)
                {
                    m_Res = new ResDataVersion();
                }
                if (m_Res.resVersionType != ResDataVersionType.Client)
                {
                    m_Res.resVersionType = ResDataVersionType.Client;
                }
                SyncResGateway();
                return m_Res;
            }
        }

        public string ClientResVersionGateway()
        {
            return m_ResRemoteGateway;
        }

        private void SyncResGateway()
        {
            if (m_ApplyCurrentResGateway)
            {
                m_Res.res_gateway = m_ResRemoteGateway;
            }
            else
            {
                m_ResRemoteGateway = m_Res.res_gateway;
            }
        }

        /// <summary>
        /// 以客户端安装包默认的资源版本配置为基准创建一个新的本地缓存
        /// </summary>
        /// <param name="remoteVersions"></param>
        public void CreateVersionsCached(ref ResDataVersion remoteVersions)
        {
            string path = AppPaths.PersistentResDataRoot.Append(ResDataVersion.FILE_RES_DATA_VERSIONS_NAME);
            string data = FileOperater.ReadUTF8Text(path);

            ResDataVersion temp = JsonUtility.FromJson<ResDataVersion>(data);
            if (temp == default || temp.IsVersionsEmpty())
            {
                "log".Log("Verision data is empty, will create new one.");
                temp = new ResDataVersion
                {
                    res_gateway = remoteVersions.res_gateway,
                    app_version = remoteVersions.app_version,
                    res_version = remoteVersions.res_version
                };
                
                ResDataVersion client = Versions;
                temp.CloneVersionsFrom(ref client);
            }
            CachedVersion = temp;
            CachedVersion.resVersionType = ResDataVersionType.Cached;
        }

        /// <summary>
        /// 加载远端资源版本配置
        /// </summary>
        /// <param name="handler"></param>
        public void LoadRemoteVersion(Action<bool, float> updateHandler, Func<bool> versionInvalidHandler, out int statu)
        {
            statu = 0;
            if (Application.internetReachability == NetworkReachability.NotReachable)
            {
                statu = 1;//无网络
                updateHandler?.Invoke(true, 1f);
            }
            else
            {
                UpdateHandler = updateHandler;
                VersionInvalidHandler = versionInvalidHandler;
                Loader.Loader loader = new Loader.Loader();
                loader.CompleteEvent.AddListener(OnLoadComplete);
                loader.Load(Versions.res_gateway.Append(ResDataVersion.FILE_RES_DATA_VERSIONS_NAME));
            }
        }

        public string RemoteAppVersion { get; private set; }

        private void OnLoadComplete(bool success, Loader.Loader target)
        {
            if (success)
            {
                string json = target.TextData;
                target.Dispose();

                ResDataVersion remoteVersions = JsonUtility.FromJson<ResDataVersion>(json);
                remoteVersions.resVersionType = ResDataVersionType.Remote;
                CreateVersionsCached(ref remoteVersions);

                RemoteAppVersion = remoteVersions.app_version;
                if (VersionInvalidHandler != default)
                {
                    bool flag = VersionInvalidHandler();
                    if (flag)
                    {
                        "warning:There have a newest App.".Log();
                        return;
                    }
                }

                mRemoteResVersion = remoteVersions.res_version;
                List<ResVersion> resUpdate = CachedVersion.CheckUpdates(Versions, ref remoteVersions);
                CachedVersion.WriteAsCached();

                if (resUpdate.Count == 0)
                {
                    UpdateHandler?.Invoke(true, 1f);
                }
                else
                {
                    StartLoadPatchRes(ref resUpdate);
                }
            }
            else
            {
                "error: Load remote version failed, url is {0}".Log(target.Url);
            }
        }

        /// <summary>
        /// 更新资源补丁
        /// </summary>
        /// <param name="resUpdate"></param>
        private void StartLoadPatchRes(ref List<ResVersion> resUpdate)
        {
            AssetsLoader loader = new AssetsLoader();
            loader.CompleteEvent.AddListener(OnPatchLoadFinished);
            loader.RemoteAssetUpdated.AddListener(OnResItemUpdated);

            ResVersion item;
            string url, resName;
            int max = resUpdate.Count;
            for (int i = 0; i < max; i++)
            {
                item = resUpdate[i];
                url = item.Url;
                resName = item.name;
                loader.AddRemote(url, resName, true);
            }
            loader.Load(out _);
        }

        private void OnPatchLoadFinished(bool flag, AssetsLoader loader)
        {
            loader.Dispose();
        }

        /// <summary>
        /// 单个资源更新加载完成
        /// </summary>
        /// <param name="flag"></param>
        /// <param name="name"></param>
        private void OnResItemUpdated(bool flag, string name)
        {
            if (CachedVersion == default)
            {
                return;
            }

            CachedVersion.RemoveUpdate(name);

            float min = CachedVersion.UpdatingLoaded;
            float max = CachedVersion.UpdatingMax;
            bool isCompleted = CachedVersion.UpdatingLoaded >= CachedVersion.UpdatingMax;

            UpdateHandler?.Invoke(isCompleted, min / max);

            bool isFinished = min >= max;
            if (isFinished)
            {
                CachedVersion.res_version = mRemoteResVersion;
                CachedVersion.WriteAsCached();
                UpdateHandler = default;
                CachedVersion.ResetUpdatingsCount();
            }
        }

        public void CacheResVersion(bool isExit)
        {
            if (CachedVersion == default)
            {
                return;
            }
            if (CachedVersion.resVersionType == ResDataVersionType.Cached)
            {
                CachedVersion?.WriteAsCached();
            }
            if (isExit)
            {
                CachedVersion.Clean();
                CachedVersion = default;
            }
        }
        public void ClearPersistent()
        {
            string path = AppPaths.PersistentResDataRoot;
            if (System.IO.Directory.Exists(path))
            {
                System.IO.Directory.Delete(path, true);
            }
        }
    }

}