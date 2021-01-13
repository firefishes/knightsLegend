using ShipDock.Applications;
using ShipDock.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace ShipDock.Versioning
{
    public enum ResDataVersionType
    {
        Empty = 0,
        Client,
        Remote,
        Cached,
    }

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

        #region 编辑器相关
#if UNITY_EDITOR
        private List<ResVersion> mResChanged;

        /// <summary>本版数据包含的所有变更过的资源版本</summary>
        public ResVersion[] ResChanges { get; private set; }

        /// <summary>
        /// 新建一个资源版本
        /// </summary>
        /// <param name="remoteRootURL">远端资源服务器网关</param>
        /// <param name="isUpdateVersion">是否迭代资源版本</param>
        /// <param name="isUpdateResVersion">是否迭代资源大版本</param>
        /// <param name="isSyncAppVersion">是否同步应用版本</param>
        /// <param name="remoteVers">远端资源服务器的资源配置</param>
        /// <param name="abNames">需要迭代版本的资源名</param>
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

        /// <summary>
        /// 迭代资源总依赖文件的版本
        /// </summary>
        /// <param name="abNames"></param>
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
                mResIndexMapper[name] = mRes.Count;
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

        private void RefreshInEditor()
        {
            List<ResVersion> repeates = default;
            GetRepeatsVersionRes(ref repeates);

            int max = repeates.Count;
            Debug.Log("Repeate items total " + max);
            for (int i = 0; i < max; i++)
            {
                mRes.Remove(repeates[i]);
            }

            CreateIndexsMapper();
        }

        private void GetRepeatsVersionRes(ref List<ResVersion> repeates)
        {
            repeates = new List<ResVersion>();
            KeyValueList<string, ResVersion> realMapper = new KeyValueList<string, ResVersion>();
            foreach (var item in mRes)
            {
                if (realMapper.ContainsKey(item.name))
                {
                    if (realMapper[item.name].version >= item.version)
                    {
                        Debug.LogWarning("Repeate version item, name is " + item.name);
                        repeates.Add(item);
                    }
                    else
                    {
                        realMapper[item.name] = item;
                    }
                }
                else
                {
                    realMapper[item.name] = item;
                }
            }
        }

        private void InitResChangedInEditor()
        {
            mResChanged = ResChanges == default ? new List<ResVersion>() : new List<ResVersion>(ResChanges);
        }

        private void RefreshResChangedInEditor()
        {
            if (mResChanged != default)
            {
                ResChanges = mResChanged.ToArray();
            }
        }
#endif
        #endregion

        /// <summary>
        /// 作为本地正式缓存写入本地文件
        /// </summary>
        public void WriteAsCached()
        {
            Refresh();
            SaveUpdatings();

            string versions = JsonUtility.ToJson(this);
#if !RELEASE
            FileOperater.WriteUTF8Text(versions, AppPaths.PersistentResDataRoot.Append(FILE_RES_DATA_VERSIONS_NAME.Append(".json~")));//写入一个用于查看的文件
#endif
            "todo".Log("资源版本写入设备时需要加密");
            FileOperater.WriteBytes(versions, AppPaths.PersistentResDataRoot.Append(FILE_RES_DATA_VERSIONS_NAME));
        }

        /// <summary>资源配置的类别</summary>
        [Sirenix.OdinInspector.ReadOnly]
        public ResDataVersionType resVersionType = ResDataVersionType.Empty;
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
        private Dictionary<string, int> mResIndexMapper;
        private Dictionary<string, ResUpdating> mUpdatingMapper;

        public int UpdatingMax { get; private set; }
        public Action UpdateCompleted { get; set; }

        public int UpdatingLoaded
        {
            get
            {
                int result = UpdatingMax - mUpdatingCount;

                Mathf.Max(0, result);
                Mathf.Min(UpdatingMax, result);
                return result;
            }
        }
        
        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            InitTemporaryData();
#if UNITY_EDITOR
            InitResChangedInEditor();
#endif
        }

        private void InitTemporaryData()
        {
            mRes?.Clear();
            mUpdatings?.Clear();

            CreateIndexsMapper();

            mRes = IsVersionsEmpty() ? new List<ResVersion>() : new List<ResVersion>(res);
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

        private void CreateIndexsMapper()
        {
            string resName;
            ResVersion item;
            mResIndexMapper?.Clear();
            mResIndexMapper = new Dictionary<string, int>();
            int max = IsVersionsEmpty() ? 0 : res.Length;
            for (int i = 0; i < max; i++)
            {
                item = res[i];
                resName = item.name;
                mResIndexMapper[resName] = i;
            }
        }

        private void CleanResAndUpdatinsRaw()
        {
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
        }

        public void Clean(bool isRrefresh = true, bool deleteAll = false)
        {
            if (isRrefresh)
            {
                Refresh();
            }
            if (deleteAll)
            {
                CleanResAndUpdatinsRaw();
            }

            mRes?.Clear();
            mUpdatings?.Clear();
            mResIndexMapper?.Clear();
            mUpdatingMapper?.Clear();

            UpdateCompleted = default;
            mResIndexMapper = default;
            mUpdatingMapper = default;
            mUpdatings = default;
        }

        /// <summary>
        /// 将中间数据更新到正式数据
        /// </summary>
        public void Refresh()
        {
            if (mRes != default)
            {
#if UNITY_EDITOR
                RefreshInEditor();
#endif
                res = mRes.ToArray();
            }
#if UNITY_EDITOR
            RefreshResChangedInEditor();
#endif
            res_total = res.Length;
            updating_total = updatings.Length;
        }

        /// <summary>
        /// 保存正在等待更新的资源版本
        /// </summary>
        private void SaveUpdatings()
        {
            mUpdatings.Clear();

            var enumer = mUpdatingMapper.GetEnumerator();
            int max = mUpdatingMapper.Count;
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
            "log: Updating saved, total {0}".Log(mUpdatings.Count > 0, mUpdatings.Count.ToString());
        }

        /// <summary>
        /// 从另一个资源版本对象复制数据
        /// </summary>
        /// <param name="copyFrom"></param>
        public void CloneVersionsFrom(ref ResDataVersion copyFrom)
        {
            Clean(false, true);

            if (copyFrom.IsVersionsEmpty())
            {
                copyFrom.Init();
            }
            app_version = copyFrom.app_version;
            res_version = copyFrom.res_version;
            res_gateway = copyFrom.res_gateway;

            int resSize = copyFrom.res.Length;
            res = new ResVersion[resSize];
            for (int i = 0; i < resSize; i++)
            {
                res[i] = new ResVersion()
                {
                    name = copyFrom.res[i].name,
                    version = copyFrom.res[i].version,
                };
            }
            
            int updatingsSize = copyFrom.updatings != default ? copyFrom.updatings.Length : 0;
            updatings = new ResUpdating[updatingsSize];
            for (int i = 0; i < updatingsSize; i++)
            {
                updatings[i] = new ResUpdating()
                {
                    name = copyFrom.updatings[i].name,
                    version = copyFrom.updatings[i].version,
                };
            }

            res_total = res.Length;
            updating_total = updatings.Length;
        }

        /// <summary>
        /// 检查版本差异
        /// </summary>
        /// <param name="clientVersions"></param>
        /// <param name="remoteVersions"></param>
        /// <returns></returns>
        public List<ResVersion> CheckUpdates(ResDataVersion clientVersions, ref ResDataVersion remoteVersions)
        {
            if (clientVersions.resVersionType != ResDataVersionType.Client || 
                remoteVersions.resVersionType != ResDataVersionType.Remote)
            {
                return new List<ResVersion>(0);
            }

            bool isVersionsEmpty = IsVersionsEmpty();
            if (isVersionsEmpty)
            {
                CloneVersionsFrom(ref clientVersions);//复制安装包中默认的资源版本
            }
            Init();

            ResVersion remoteItem = default, cachedItem = default;
            if (isVersionsEmpty)
            {
                AddUpdatingsFromExisted(ref remoteItem, ref cachedItem);
            }
            SyncCachedUpdatingToMapper();
            AddUpdatingsFromRemote(ref remoteVersions, ref remoteItem, ref cachedItem);

            Refresh();

            GetWillUpdateList(out List<ResVersion> result);
            return result;
        }

        /// <summary>
        /// 获取更新列表
        /// </summary>
        /// <param name="result"></param>
        private void GetWillUpdateList(out List<ResVersion> result)
        {
            mUpdatingCount = mUpdatings.Count;
            UpdatingMax = mUpdatings.Count;
            result = new List<ResVersion>();
            string abName;
            ResVersion item;
            int max = mUpdatings.Count;
            for (int i = 0; i < max; i++)
            {
                abName = mUpdatings[i].name;
                item = GetResVersion(abName);
                if (item == default)
                {
                    item = new ResVersion
                    {
                        name = abName,
                        version = DEFAULT_VERSION,
                    };
                }
                item.Url = res_gateway.Append(abName);
                result.Add(item);
            }
        }

        /// <summary>
        /// 从远端资源版本对比资源差异，并标记需要更新的资源
        /// </summary>
        /// <param name="remoteVersions"></param>
        /// <param name="remoteItem"></param>
        /// <param name="cachedItem"></param>
        private void AddUpdatingsFromRemote(ref ResDataVersion remoteVersions, ref ResVersion remoteItem, ref ResVersion cachedItem)
        {
            if (remoteVersions.resVersionType == ResDataVersionType.Remote)
            {
                bool isResExist;
                ResVersion[] list = remoteVersions.res;
                int max = list.Length, statu = 0;
                for (int i = 0; i < max; i++)
                {
                    statu = 0;
                    remoteItem = list[i];
                    isResExist = IsResExist(ref remoteItem);
                    if (isResExist)
                    {
                        CampareVersion(ref cachedItem, ref remoteItem, out statu);
                        if (statu == 1)
                        {
                            cachedItem = AddNewRes(remoteItem.name, remoteItem.version);//增加本地新资源的版本
                        }
                    }
                    else
                    {
                        statu = 2;//资源版本不存在
                        cachedItem = AddNewRes(remoteItem.name, remoteItem.version);//增加本地新资源的版本
                    }
                    if (statu != 0)
                    {
                        AddToUpdate(ref remoteItem);
                    }
                }
            }
        }

        /// <summary>
        /// 标记本版本包含的所有资源为需要更新
        /// </summary>
        /// <param name="remoteItem"></param>
        /// <param name="cachedItem"></param>
        private void AddUpdatingsFromExisted(ref ResVersion remoteItem, ref ResVersion cachedItem)
        {
            int willUpdate = mRes.Count;
            for (int i = 0; i < willUpdate; i++)
            {
                remoteItem = mRes[i];
                AddToUpdate(ref remoteItem);
            }
        }

        /// <summary>
        /// 同步上次记录的更新列表（仅对类别为本地缓存的资源版本配置有效）
        /// </summary>
        private void SyncCachedUpdatingToMapper()
        {
            if (resVersionType == ResDataVersionType.Cached)
            {
                mUpdatingMapper?.Clear();//清空，为重新映射数据做准备
                if (mUpdatingMapper == default)
                {
                    mUpdatingMapper = new Dictionary<string, ResUpdating>();
                }
                string abName;
                ResUpdating resUpdate;
                int max = mUpdatings.Count;
                for (int i = 0; i < max; i++)
                {
                    resUpdate = mUpdatings[i];
                    abName = resUpdate.name;
                    abName = abName.ToLower();//资源打包后的文件名均为小写
                    if (!mUpdatingMapper.ContainsKey(abName))
                    {
                        mUpdatingMapper[abName] = resUpdate;
                    }
                }
            }
        }

        /// <summary>
        /// 将资源版本标记为需要更新
        /// </summary>
        /// <param name="remoteItem"></param>
        private void AddToUpdate(ref ResVersion remoteItem)
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

        /// <summary>
        /// 查找本地没有的资源及版本号不同的更新
        /// </summary>
        /// <param name="cachedItem"></param>
        /// <param name="remoteItem"></param>
        /// <param name="needUpdate"></param>
        private void CampareVersion(ref ResVersion cachedItem, ref ResVersion remoteItem, out int statu)
        {
            statu = 0;
            cachedItem = GetResVersion(remoteItem.name);
            if (cachedItem == default)
            {
                statu = 1;//未包含此资源版本
            }
            else
            {
                if (cachedItem.version != remoteItem.version)
                {
                    statu = 2;//需要更新资源的版本
                    cachedItem.version = remoteItem.version;
                }
            }
        }

        /// <summary>
        /// 是否为不同的资源版本配置（对比应用版本及资源大版本）
        /// </summary>
        /// <param name="remoteVersions"></param>
        /// <returns></returns>
        private bool IsDiffVersionData(ref ResDataVersion remoteVersions)
        {
            return (res_version != remoteVersions.res_version) ||
                    (app_version != remoteVersions.app_version);
        }

        /// <summary>
        /// 判断资源版本所代表的文件是否存在于本地缓存
        /// </summary>
        /// <param name="version"></param>
        /// <returns></returns>
        private bool IsResExist(ref ResVersion version)
        {
            string filePath = AppPaths.PersistentResDataRoot.Append(version.name);
            return (version != default) && File.Exists(filePath);
        }

        /// <summary>
        /// 是否一个空的资源版本配置
        /// </summary>
        /// <returns></returns>
        public bool IsVersionsEmpty()
        {
            return res == default || res.Length == 0;
        }

        /// <summary>
        /// 新增一个资源版本
        /// </summary>
        /// <param name="abName"></param>
        /// <param name="version"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 获取资源版本
        /// </summary>
        /// <param name="abName"></param>
        /// <returns></returns>
        public ResVersion GetResVersion(string abName)
        {
            ResVersion result = default;
            if (mResIndexMapper != default)
            {
                bool hasValue = mResIndexMapper.TryGetValue(abName, out int value);
                if (hasValue)
                {
                    result = res[value];
                }
            }
            return result;
        }

        /// <summary>
        /// 从更新列表中移除一个已更新的资源版本
        /// </summary>
        /// <param name="abName"></param>
        public void RemoveUpdate(string abName)
        {
            if (mUpdatingMapper.ContainsKey(abName))
            {
                mUpdatingMapper.Remove(abName);
                mUpdatingCount--;
            }
            "log: Patch updated, remains {0}".Log(mUpdatingCount.ToString());

            if (mUpdatingCount <= 0)
            {
                UpdateCompleted?.Invoke();
                UpdateCompleted = default;

                "log".Log("Patches update Completed!");
            }
        }

        public void ResetUpdatingsCount()
        {
            mUpdatingCount = UpdatingMax;
        }
    }
}
