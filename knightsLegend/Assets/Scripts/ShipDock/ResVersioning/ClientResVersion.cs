using ShipDock.Applications;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ShipDock.Versioning
{
    public interface IClientResVersion
    {

    }

    [CreateAssetMenu(fileName = "ClientResVersions", menuName = "ShipDock : 客户端资源版本", order = 100)]
    public class ClientResVersion : ScriptableObject, IClientResVersion
    {
        [Header("资源版本信息")]
        [SerializeField]
        private ResDataVersion m_Res;
#if UNITY_EDITOR
        [Header("以下仅用于编辑器")]
        [SerializeField]
        private ResVersion[] m_ResChanged;

        public void SetChanges(ResVersion[] resChanges)
        {
            m_ResChanged = resChanges;
        }
#endif

        private ResDataVersion mCached;

        public string Source
        {
            get
            {
                return JsonUtility.ToJson(m_Res);
            }
            set
            {
                m_Res = JsonUtility.FromJson<ResDataVersion>(value);
            }
        }

        public ResDataVersion Versions
        {
            get
            {
                if (m_Res == default)
                {
                    m_Res = new ResDataVersion();
                }
                return m_Res;
            }
        }

        public ResDataVersion CachedVersion { get; private set; }

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
                    res_version = remoteVersions.res_version - 1
                };
                CachedVersion = Versions;
            }
            else
            {
                CachedVersion = temp;
            }
        }

        public void LoadRemoteVersion(UnityAction<bool, Loader.Loader> handler)
        {
            Loader.Loader loader = new Loader.Loader();
            loader.CompleteEvent.AddListener(handler);
            loader.Load(Versions.res_gateway.Append(ResDataVersion.FILE_RES_DATA_VERSIONS_NAME));
        }

        public List<ResVersion> CompareClientVersion(ResDataVersion remoteVersions)
        {
            CachedVersion.UpdateCompleted = default;
            CachedVersion.UpdateCompleted += () =>
            {
                CachedVersion.res_version = remoteVersions.res_version;
                CachedVersion.WriteAsCached();
            };
            List<ResVersion> result = CachedVersion.CheckUpdates(Versions, ref remoteVersions);
            CachedVersion.WriteAsCached();
            return result;
        }
    }

}