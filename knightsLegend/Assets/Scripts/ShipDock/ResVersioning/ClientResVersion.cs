using ShipDock.Applications;
using ShipDock.Loader;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Versioning
{
    /// <summary>
    /// 客户端安装包资源版本配置数据对象
    /// </summary>
    [CreateAssetMenu(fileName = "ClientResVersions", menuName = "ShipDock : 客户端资源版本", order = 100)]
    public class ClientResVersion : ScriptableObject
    {
        [Header("客户端安装包默认的资源版本配置")]
        [SerializeField]
        private string m_ResRemoteGateway;
        [SerializeField]
        private ResDataVersion m_Res;
#if UNITY_EDITOR
        [Header("以下仅用于编辑器")]
        [SerializeField]
        private ResVersion[] m_ResChanged;
        [SerializeField]
        private TextAsset m_Preview;

        private bool mIsShowPreview = false;
        private ResDataVersion mBeforePreview;
        
        public void SetChanges(ResVersion[] resChanges)
        {
            m_ResChanged = resChanges;
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

        private void OnEnable()
        {
            if (mIsShowPreview && mBeforePreview != default && m_Preview == default)
            {
                ClosePreviewVersions();
            }
        }
#endif

        private int mRemoteResVersion;

        /// <summary>本地缓存的资源版本配置</summary>
        public ResDataVersion CachedVersion { get; private set; }
        /// <summary>单个资源更新完成的回调函数</summary>
        public Action<bool, float> UpdateHandler { get; private set; }

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
#if UNITY_EDITOR
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

        private void SyncResGateway()
        {
            m_Res.res_gateway = !string.IsNullOrEmpty(m_ResRemoteGateway) ? m_ResRemoteGateway : m_Res.res_gateway;
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
            if (temp == default)
            {
                "log".Log("Verision is empty, will create new one.");
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
        public void LoadRemoteVersion(Action<bool, float> updateHandler, out int statu)
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
                Loader.Loader loader = new Loader.Loader();
                loader.CompleteEvent.AddListener(OnLoadComplete);
                loader.Load(Versions.res_gateway.Append(ResDataVersion.FILE_RES_DATA_VERSIONS_NAME));
            }
        }

        private void OnLoadComplete(bool success, Loader.Loader target)
        {
            if (success)
            {
                string json = target.TextData;
                target.Dispose();

                ResDataVersion remoteVersions = JsonUtility.FromJson<ResDataVersion>(json);
                remoteVersions.resVersionType = ResDataVersionType.Remote;
                CreateVersionsCached(ref remoteVersions);

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
            CachedVersion.RemoveUpdate(name);

            float min = CachedVersion.UpdatingLoaded;
            float max = CachedVersion.UpdatingMax;
            bool isCompleted = CachedVersion.UpdatingLoaded >= CachedVersion.UpdatingMax;

            UpdateHandler?.Invoke(isCompleted, min / max);

            bool isFinished = min >= max;
            if (isFinished)
            {
                if (mRemoteResVersion != 0)
                {
                    CachedVersion.res_version = mRemoteResVersion;
                }
                CachedVersion.WriteAsCached();
                UpdateHandler = default;
                CachedVersion.ResetUpdatingsCount();
            }
        }
    }

}