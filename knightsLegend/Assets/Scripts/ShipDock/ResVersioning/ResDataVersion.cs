using ShipDock.Applications;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ShipDock.Versioning
{
    /// <summary>
    /// 
    /// 资源版本
    /// 
    /// TODO 
    /// 需要增加zip包更新的功能（这个功能与单独文件更新该如何配合？）
    /// 
    /// </summary>
    [Serializable]
    public class ResDataVersion
    {
        /// <summary>默认的版本号初始值</summary>
        public const int DEFAULT_VERSION = 100;

        /// <summary>版本配置文件的文件名</summary>
        public static string FILE_RES_DATA_VERSIONS_NAME = "res_data_versions.sd";

#if UNITY_EDITOR
        private List<ResVersion> mResChanged;

        /// <summary>本版数据包含的所有变更过的资源版本</summary>
        public ResVersion[] ResChanges { get; private set; }

        public void CreateNewResVersion(ref string remoteRootURL, bool isUpdateVersion, bool isUpdateResVersion, bool isSyncAppVersion, ref ResDataVersion remoteVers, ref string[] abNames)
        {
            res_gateway = remoteRootURL;

            Init();
            remoteVers?.Init();

            CheckMainManifestVersion(ref abNames);
            FillChangedABList(out List<string> changeds);

            int max = abNames.Length;
            CheckTotalVersion(isUpdateResVersion, isSyncAppVersion, max);
            CheckResVersions(max, ref abNames, ref remoteVers, isUpdateVersion, ref changeds);
        }

        private void CheckResVersions(int max, ref string[] abNames, ref ResDataVersion remoteVers, bool isUpdateVersion, ref List<string> changeds)
        {
            string abName = string.Empty;
            ResVersion item, remote = default;
            for (int i = 0; i < max; i++)
            {
                abName = abNames[i];
                item = GetResVersion(abName);
                int baseVersion = item == default ? DEFAULT_VERSION : item.version;//若未找到对应的资源版本，基于默认的版本号进行设置

                if (remoteVers != default)
                {
                    remote = remoteVers.GetResVersion(abName);
                    baseVersion = remote != default ? remote.version : baseVersion;//若已存在线上版本，基于线上版本号进行设置
                }
                NewResVersion(baseVersion, isUpdateVersion, ref item, ref abName);
                UpdateVersionChange(ref changeds, ref abName, ref item);
            }
        }

        private void CheckTotalVersion(bool isUpdateResVersion, bool isSyncAppVersion, int changeABCount)
        {
            if (isUpdateResVersion && changeABCount > 0)
            {
                res_version++;
            }
            if (isSyncAppVersion)
            {
                app_version = Application.version;
            }
        }

        private void FillChangedABList(out List<string> changeds)
        {
            int max = ResChanges != default ? ResChanges.Length : 0;
            changeds = new List<string>();
            string abName = string.Empty;
            for (int i = 0; i < max; i++)
            {
                abName = ResChanges[i].name;
                changeds.Add(abName);
            }
        }

        private void CheckMainManifestVersion(ref string[] abNames)
        {
            ResVersion resData = GetResVersion(AppPaths.resData);
            if (resData == default)
            {
                AddNewRes(AppPaths.resData, DEFAULT_VERSION);
            }
            else
            {
                if (abNames.Length > 0)
                {
                    resData.version++;
                }
            }
        }

        private void NewResVersion(int baseVersion, bool isUpdateVersion, ref ResVersion item, ref string name)
        {
            if (item == default)
            {
                item = AddNewRes(name, DEFAULT_VERSION);
                mAllResMapper[name] = mRes.Count;
            }
            else
            {
                int version = isUpdateVersion ? baseVersion + 1 : baseVersion;
                item.version = version;
            }
        }

        private void UpdateVersionChange(ref List<string> changeds, ref string abName, ref ResVersion item)
        {
            if (changeds.Contains(abName))
            {
                int index = changeds.IndexOf(abName);
                mResChanged[index].version = item.version;
            }
            else
            {
                if (item != default)
                {
                    mResChanged.Add(item);
                }
                else
                {
                    Debug.LogError("Version item is null");
                }
            }
        }
#endif

        /// <summary>
        /// 作为本地正式缓存写入本地文件
        /// </summary>
        public void WriteAsCached()
        {
            Refresh();
            SaveUpdatings();

            string versions = JsonUtility.ToJson(this);
            FileOperater.WriteUTF8Text(versions, AppPaths.PersistentResDataRoot.Append(FILE_RES_DATA_VERSIONS_NAME.Append(".json~")));
            FileOperater.WriteBytes(versions, AppPaths.PersistentResDataRoot.Append(FILE_RES_DATA_VERSIONS_NAME));
        }

        /// <summary>资源配置版本号</summary>
        public int res_version;
        /// <summary>App版本号</summary>
        public string app_version;
        /// <summary>远程资源服务器网关</summary>
        public string res_gateway;
        /// <summary>总资源数</summary>
        public int res_total;
        /// <summary>更新的资源数</summary>
        public int updating_total;
        /// <summary>本版数据包含的所有资源版本</summary>
        public ResVersion[] res;
        [HideInInspector]
        /// <summary>需要加载的资源映射，用于排除重复更新/summary>
        public ResUpdating[] updatings;

        private int mUpdatingCount;
        private List<ResVersion> mRes;
        private List<ResUpdating> mUpdatings;
        private Dictionary<string, int> mAllResMapper;
        private Dictionary<string, ResUpdating> mUpdatingMapper;

        public IClientResVersion ClientResVersion { get; set; }
        public Action UpdateCompleted { get; set; }
        
        public void Init(bool isRefresh = false)
        {
            if (isRefresh)
            {
                Refresh();
            }
            if (mAllResMapper != default)
            {
                mRes.Clear();
                mAllResMapper.Clear();
                mUpdatings.Clear();
            }
            mAllResMapper = new Dictionary<string, int>();
            int max = IsVersionsEmpty() ? 0 : res.Length;
            for (int i = 0; i < max; i++)
            {
                mAllResMapper[res[i].name] = i;
            }
            mRes = IsVersionsEmpty() ? new List<ResVersion>() : new List<ResVersion>(res);
#if UNITY_EDITOR
            mResChanged = ResChanges == default ? new List<ResVersion>() : new List<ResVersion>(ResChanges);
#endif
            if (updatings != default)
            {
                mUpdatings = new List<ResUpdating>(updatings);
                Array.Clear(updatings, 0, updatings.Length);
            }
            else
            {
                mUpdatings = new List<ResUpdating>();
            }
        }

        public void Clean()
        {
            Refresh();

            mRes.Clear();
            mAllResMapper.Clear();
            mUpdatingMapper?.Clear();
            mUpdatings.Clear();

            UpdateCompleted = default;
            mAllResMapper = default;
            mUpdatingMapper = default;
            mUpdatings = default;
        }

        public void Refresh()
        {
            if (mRes != default)
            {
                res = mRes.ToArray();
            }
#if UNITY_EDITOR
            if (mResChanged != default)
            {
                ResChanges = mResChanged.ToArray();
            }
#endif
            res_total = res.Length;
        }

        private void SaveUpdatings()
        {
            var enumer = mUpdatingMapper.GetEnumerator();
            int max = mUpdatingMapper.Count;
            mUpdatings.Clear();
            updatings = new ResUpdating[max];
            ResUpdating item;
            for (int i = 0; i < max; i++)
            {
                item = enumer.Current.Value;
                if (item != default)
                {
                    mUpdatings.Add(item);
                }
                enumer.MoveNext();
            }
            updatings = mUpdatings.ToArray();
            updating_total = updatings.Length;
            "log: Updating saved, total {0}".Log(mUpdatings.Count.ToString());
        }

        public void CloneVersionsFrom(ref ResDataVersion target)
        {
            res_total = 0;
            updating_total = 0;
            if (res != default)
            {
                Array.Clear(res, 0, res.Length);
                res = new ResVersion[0];
            }
            if (updatings != default)
            {
                Array.Clear(updatings, 0, updatings.Length);
                updatings = new ResUpdating[0];
            }
            if (target.IsVersionsEmpty())
            {
                target.Init();
            }

            app_version = target.app_version;
            res_version = target.res_version;
            res_gateway = target.res_gateway;

            res = new ResVersion[target.res.Length];
            target.res.CopyTo(res, 0);
            
            if (target.updatings == default)
            {
                target.updatings = new ResUpdating[0];
            }
            updatings = new ResUpdating[target.updatings.Length];
            target.updatings.CopyTo(updatings, 0);

            mUpdatings = new List<ResUpdating>(updatings);
        }

        public List<ResVersion> CheckUpdates(ResDataVersion clientVersions, ref ResDataVersion remoteVersions)
        {
            bool isVersionsEmpty = IsVersionsEmpty();
            if (isVersionsEmpty)
            {
                CloneVersionsFrom(ref clientVersions);//复制应用中附带的版本
            }

            Init();

            ResVersion remoteItem, cachedItem = default;
            if (isVersionsEmpty)
            {
                int willUpdate = mRes.Count;
                for (int i = 0; i < willUpdate; i++)
                {
                    remoteItem = mRes[i];
                    SetUpdate(ref remoteItem);
                }
            }

            SyncUpdatingToMapper();
            
            ResVersion[] list = remoteVersions.res;
            int max = list.Length;
            bool needUpdate, isResNotExist;
            for (int i = 0; i < max; i++)
            {
                remoteItem = list[i];
                isResNotExist = IsResExist(ref remoteItem);
                if (isResNotExist)
                {
                    CampareVersion(ref cachedItem, ref remoteItem, out needUpdate);//查找本地没有的资源及版本号不同的更新
                }
                else
                {
                    needUpdate = true;
                    cachedItem = AddNewRes(remoteItem.name, remoteItem.version);//增加本地新资源的版本
                }
                if (needUpdate)
                {
                    SetUpdate(ref remoteItem);//设置需要更新的资源版本
                }
            }
            Refresh();//将中间数据更新到正式数据

            ResVersion item;
            List<ResVersion> result = new List<ResVersion>();
            int n = mUpdatings.Count;
            string abName;
            for (int i = 0; i < n; i++)
            {
                abName = mUpdatings[i].name;
                item = GetResVersion(abName);
                if (item != default)
                {
                    item.Url = res_gateway.Append(abName);
                }
                else
                {
                    item = new ResVersion
                    {
                        name = abName,
                        version = DEFAULT_VERSION,
                        Url = res_gateway.Append(abName),
                    };
                }
                result.Add(item);
            }
            mUpdatingCount = result.Count;
            return result;
        }

        private void SyncUpdatingToMapper()
        {
            if (mUpdatingMapper == default)
            {
                mUpdatingMapper = new Dictionary<string, ResUpdating>();
            }
            else
            {
                mUpdatingMapper.Clear();//清空，重新映射数据
            }
            CreateUpdatingFromCached();
        }

        private void CreateUpdatingFromCached()
        {
            string abName;
            ResUpdating resUpdate;
            int max = mUpdatings.Count;
            for (int i = 0; i < max; i++)
            {
                resUpdate = mUpdatings[i];
                abName = resUpdate.name.ToLower();
                if (!mUpdatingMapper.ContainsKey(abName))
                {
                    mUpdatingMapper[abName] = resUpdate;
                }
            }
        }

        private void SetUpdate(ref ResVersion remoteItem)
        {
            bool hasUpdating = mUpdatingMapper.TryGetValue(remoteItem.name, out _);
            if (!hasUpdating)
            {
                string name = remoteItem.name.ToLower();
                ResUpdating newUpdating = new ResUpdating()
                {
                    name = name,
                    version = remoteItem.version,
                };
                mUpdatings.Add(newUpdating);
                mUpdatingMapper[name] = newUpdating;
            }
        }

        private void CampareVersion(ref ResVersion cachedItem, ref ResVersion remoteItem, out bool needUpdate)
        {
            needUpdate = false;
            cachedItem = GetResVersion(remoteItem.name);
            if (cachedItem == default)
            {
                needUpdate = true;
                cachedItem = AddNewRes(remoteItem.name, remoteItem.version);//增加本地新资源的版本
            }
            else
            {
                if (cachedItem.version != remoteItem.version)
                {
                    needUpdate = true;//更新本地资源的版本
                    cachedItem.version = remoteItem.version;
                }
            }
        }

        private bool IsDiffFrom(ref ResDataVersion remoteVersions)
        {
            return (res_version != remoteVersions.res_version) ||
                    (app_version != remoteVersions.app_version);
        }

        private bool IsResExist(ref ResVersion version)
        {
            string filePath = AppPaths.PersistentResDataRoot.Append(version.name);
            return version != default && File.Exists(filePath);
        }

        public bool IsVersionsEmpty()
        {
            return res == default || res.Length == 0;
        }

        private ResVersion AddNewRes(string abName, int version)
        {
            ResVersion item = new ResVersion()
            {
                name = abName,
                version = version,
            };
            mRes.Add(item);
            return item;
        }

        public ResVersion GetResVersion(string abName)
        {
            ResVersion result = default;
            if (mAllResMapper != default)
            {
                bool hasValue = mAllResMapper.TryGetValue(abName, out int value);
                if (hasValue)
                {
                    result = res[value];
                }
            }
            return result;
        }

        public void RemoveUpdate(string abName)
        {
            if (mUpdatingMapper.ContainsKey(abName))
            {
                mUpdatingMapper.Remove(abName);
                mUpdatingCount--;
            }
            "log: Patch updated, remains {0}".Log(mUpdatingCount.ToString());

            if (mUpdatingCount < 5)
            {
                foreach(var i in mUpdatingMapper)
                {
                    Debug.Log(i.Key);
                }
            }

            if (mUpdatingCount <= 0)
            {
                mUpdatingCount = 0;
                UpdateCompleted?.Invoke();
                UpdateCompleted = default;

                "log".Log("Patches update Completed!");
            }
        }
    }
}
