#define _G_LOG

using ShipDock.Applications;
using ShipDock.Interfaces;
using ShipDock.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Loader
{
    /// <summary>
    /// 资源加载器
    /// </summary>
    public class AssetsLoader : IDispose
    {
        /// <summary>正在处理的依赖资源索引号</summary>
        private int mIndex;
        /// <summary>遍历依赖资源时的数量</summary>
        private int mDepsWalkMax;
        /// <summary>通用加载器</summary>
        private Loader mLoader;
        /// <summary>正在处理的加载操作器</summary>
        private LoaderOpertion mCurrentOption;
        /// <summary>加载操作器的队列</summary>
        private Queue<LoaderOpertion> mOpertions;
        /// <summary>已检测过依赖资源的标记映射</summary>
        private List<string> mDepsSigned;

        public float LoadingProgress
        {
            get
            {
                float remains = Loaded * 1f;
                float totals = LoadingTotal * 1f;
                float result = LoadingTotal == 0f ? 1f : remains / totals;
                "log:Assets loader progress: {0}/{1}".Log(remains.ToString(), totals.ToString());
                "log:Assets loader progress: {0}%".Log(((int)(result * 100f)).ToString());
                return result;
            }
        }

        public int LoadingTotal
        {
            get
            {
                return ResList != default ? ResList.Count : 0;
            }
            set { }
        }

        public string LoaderKey
        {
            get
            {
                return mLoader != default ? mLoader.LoaderKey : string.Empty;
            }
            set
            {
                if (mLoader != default)
                {
                    mLoader.LoaderKey = value;
                }
                else { }
            }
        }

        public int Loaded { get; private set; }
        public List<string> ResList { get; private set; }
        public string[] DirectDependencies { get; private set; }
        public OnAssetLoaderCompleted CompleteEvent { get; private set; } = new OnAssetLoaderCompleted();
        public OnRemoteAssetUpdated RemoteAssetUpdated { get; private set; } = new OnRemoteAssetUpdated();
        public AssetBundleManifest AessetManifest { get; private set; }
        public AssetBundles ABs { get; private set; }

        public AssetsLoader()
        {
            ResList = new List<string>();
            mOpertions = new Queue<LoaderOpertion>();
            mLoader = Loader.GetAssetBundleLoader();
            mLoader.CompleteEvent.AddListener(OnCompleted);

            AssetBundles abs = Framework.Instance.GetUnit<AssetBundles>(Framework.UNIT_AB);
            if (Framework.Instance.IsStarted)
            {
                ABs = abs;
            }
            else
            {
                "warning:".Log("AB包管理器为空，请在 Framework 定制模块中定义");
                ABs = new AssetBundles();
            }

            AessetManifest = ABs.GetManifest();
        }

        public void Dispose()
        {
            mIndex = 0;
            Loaded = 0;
            Utils.Reclaim(mLoader);

            ResList.Clear();

            CompleteEvent?.RemoveAllListeners();
            RemoteAssetUpdated?.RemoveAllListeners();

            CompleteEvent = default;
            RemoteAssetUpdated = default;
            mLoader = default;
        }

        private void InitAssetLoader()
        {
            if (mLoader.LoadType != Loader.LOADER_ASSETBUNDLE)
            {
                if (mLoader != default)
                {
                    mLoader.Dispose();
                }
                else { }

                mLoader = Loader.GetAssetBundleLoader();
                mLoader.CompleteEvent.AddListener(OnCompleted);
            }
            else { }
        }

        private void InitDefaultLoader()
        {
            if (mLoader.LoadType != Loader.LOADER_DEFAULT)
            {
                if (mLoader != default)
                {
                    mLoader.Dispose();
                }
                else { }

                mLoader = new Loader();
                mLoader.CompleteEvent.AddListener(OnCompleted);
            }
            else { }
        }

        public string GetLoadError()
        {
            return mLoader?.LoadError;
        }

        public byte[] GetCurrentData()
        {
            return mLoader?.ResultData;
        }

        public string GetCurrentTextData()
        {
            return mLoader?.TextData;
        }

        /// <summary>
        /// 增加要加载的远程服务器资源
        /// </summary>
        /// <param name="url">资源url</param>
        /// <returns></returns>
        public AssetsLoader AddRemote(string url, string relativeName, bool toLower = false)
        {
            if (mLoader != default)
            {
                if (toLower)
                {
                    url = url.ToLower();
                    relativeName = relativeName.ToLower();
                }
                else { }

                LoaderOpertion opertion = new LoaderOpertion()
                {
                    remoteURL = url,
                    relativeName = relativeName,
                    isRemote = true,
                };
                "log: 添加远端资源队列: {0}".Log(opertion.relativeName);

                AddResList(ref url, false);
                mOpertions.Enqueue(opertion);
            }
            else { }

            return this;
        }

        /// <summary>
        /// 增加需要加载本地资源的主依赖文件
        /// </summary>
        /// <param name="relativeName">资源名，通常为资源相对路径中的一部分</param>
        /// <param name="manifestName">总依赖名</param>
        /// <returns></returns>
        public AssetsLoader AddManifest(string manifestName, bool isPersistent = false)
        {
            if (mLoader != default)
            {
                LoaderOpertion opertion = new LoaderOpertion()
                {
                    manifestName = manifestName,
                    relativeName = manifestName,
                    isManifest = true,
                    isPersistentPath = isPersistent,
                };
                "log: 添加资源主依赖队列: {0}".Log(opertion.relativeName);

                AddResList(ref manifestName, true);
                mOpertions.Enqueue(opertion);
            }
            else { }

            return this;
        }

        /// <summary>
        /// 增加需要加载的本地资源
        /// </summary>
        /// <param name="relativeName">资源名，通常为资源相对路径中的一部分</param>
        /// <param name="isDependenciesLoader">是否加载依赖资源文件</param>
        /// <returns></returns>
        public AssetsLoader Add(string relativeName, bool isDependenciesLoader = true, bool isPersistent = false)
        {
            if (mLoader != default)
            {
                LoaderOpertion opertion = new LoaderOpertion()
                {
                    relativeName = relativeName,
                    isGetDependencies = isDependenciesLoader,
                    isPersistentPath = isPersistent,
                };
                InitDependencesList(opertion.relativeName, isPersistent);
                "log: 添加本地资源队列: {0}".Log(opertion.relativeName);
                mOpertions.Enqueue(opertion);
            }
            else { }

            return this;
        }

        public AssetsLoader AddConfig(string relativeName, bool isPersistent = false)
        {
            if (mLoader != default)
            {
                LoaderOpertion opertion = new LoaderOpertion()
                {
                    relativeName = relativeName,
                    isPersistentPath = isPersistent,
                    isConfig = true,
                };
                "log: 添加配置资源队列: {0}".Log(opertion.relativeName);

                AddResList(ref relativeName, false);
                mOpertions.Enqueue(opertion);
            }
            else { }

            return this;
        }

        /// <summary>
        /// 启动加载
        /// </summary>
        /// <param name="statu"></param>
        public void Load(out int statu)
        {
            statu = 0;
            if (mCurrentOption == default)
            {
                StartLoad(out statu);
            }
            else
            {
                statu = 1;
            }
        }

        /// <summary>
        /// 加载资源依赖项
        /// </summary>
        /// <param name="statu">返回的处理状态，0=正常，1=操作器为空，2=没有依赖资源</param>
        private void StartLoad(out int statu)
        {
            statu = 0;
            mCurrentOption = default;
            if ((mOpertions != default) && (mOpertions.Count > 0))
            {
                mCurrentOption = mOpertions.Dequeue();

                string source = mCurrentOption.relativeName;
                if (mCurrentOption.isGetDependencies)//资源依赖
                {
                    InitAssetLoader();
                    source = GetPathByLoaderOption(mCurrentOption, source);
                }
                else
                {
                    if (mCurrentOption.isRemote)
                    {
                        InitDefaultLoader();
                        source = mCurrentOption.remoteURL;
                    }
                    else if (mCurrentOption.isManifest)//资源主依赖
                    {
                        InitAssetLoader();
                        source = GetPathByLoaderOption(mCurrentOption, source);
                    }
                    else if (mCurrentOption.isConfig)
                    {
                        InitDefaultLoader();
                        source = GetPathByLoaderOption(mCurrentOption, source);
                    }
                    else
                    {
                        "error".Log("加载的操作项出错.");
                    }
                }

                if (string.IsNullOrEmpty(source))
                {
                    LoadNext(out statu);//若依赖资源的路径为空，加载下一个资源
                    "empty deps".Log(statu == 2);
                }
                else
                {
                    if (mCurrentOption.isRemote || mCurrentOption.isConfig)
                    {
                        InitDefaultLoader();
                    }
                    else
                    {
                        InitAssetLoader();
                    }

                    "load res".Log(source);
                    mLoader.Load(source);//加载当前资源的依赖资源
                }
            }
            else
            {
                statu = 2;
            }
        }

        private static bool isLogTODO;

        private string GetPathByLoaderOption(LoaderOpertion opertion, string source)
        {
            if (!isLogTODO)
            {
                isLogTODO = true;
                "todo".Log("根据版本号决定是缓存目录还是项目目录获取");
            }
            else { }

            string path = opertion.isPersistentPath ?
                GetPersistentAResPath(out _, ref source) :
                AppPaths.StreamingResDataRoot.Append(source);
            return path;
        }

        private string GetPersistentAResPath(out string path, ref string relativeName)
        {
            path = AppPaths.PersistentResDataRoot.Append(relativeName);
            return path;
        }

        /// <summary>
        /// 加载下一个资源
        /// </summary>
        /// <param name="statu">返回的处理状态，0=正常，1=操作器为空，2=没有依赖资源</param>
        private void LoadNext(out int statu)
        {
            statu = 0;
            if (mOpertions.Count > 0)
            {
                mCurrentOption = default;
                Load(out statu);//继续加载下一个资源
            }
            else
            {
                statu = 2;
            }
        }

        /// <summary>
        /// 初始化依赖资源列表
        /// </summary>
        /// <param name="source"></param>
        private void InitDependencesList(string source, bool isPersistent)
        {
            if (mDepsSigned == default)
            {
                mDepsSigned = new List<string>();
            }
            else
            {
                mDepsSigned.Clear();
            }

            mDepsWalkMax = 0;
            string deped = source;
            WalkDependences(deped, isPersistent);
        }

        /// <summary>
        /// 遍历依赖资源列表，为加载所有依赖资源做准备
        /// </summary>
        /// <param name="deped"></param>
        private void WalkDependences(string deped, bool isPersistent)
        {
            mDepsWalkMax++;
            if (mDepsWalkMax > 100)
            {
                "walk deps".Log();
                return;
            }
            else { }

            string[] list = AessetManifest != default ? AessetManifest.GetDirectDependencies(deped) : default;
            "error".Log(AessetManifest == default, "遍历依赖资源时资源主依赖不可为空.");
            int max = list != default ? list.Length : 0;
            if (max > 0)
            {
                string dep = string.Empty;
                for (int i = 0; i < max; i++)
                {
                    dep = list[i];
                    if (!string.IsNullOrEmpty(dep))
                    {
                        "deps".Log(deped, dep);
                        WalkDependences(dep, isPersistent);

                        if (!mDepsSigned.Contains(dep))
                        {
                            mDepsSigned.Add(dep);
                            AddResList(ref dep, true);
                            LoaderOpertion opertion = new LoaderOpertion()
                            {
                                relativeName = dep,
                                isPersistentPath = isPersistent,
                                isGetDependencies = true,
                            };
                            mOpertions.Enqueue(opertion);
                        }
                        else { }
                    }
                    else { }
                }
            }
            else { }

            AddResList(ref deped, true);
        }

        private void OnCompleted(bool isSuccessd, Loader target)
        {
            if (isSuccessd)
            {
                LoadSuccessd(ref target);
            }
            else
            {
                LoadFailed(ref target);
            }
        }

        /// <summary>
        /// 加载失败
        /// </summary>
        /// <param name="target"></param>
        private void LoadFailed(ref Loader target)
        {
            "error".Log(target.LoadError);
            "loader failed".Log(mLoader.Url);
            CompleteEvent?.Invoke(false, this);
        }

        /// <summary>
        /// 加载成功
        /// </summary>
        /// <param name="target"></param>
        private void LoadSuccessd(ref Loader target)
        {
            "loader success".Log(target.Url);
            if (mCurrentOption.isManifest)
            {
                Loaded++;
                GetAssetManifest(ref target);
            }
            else if (mCurrentOption.isGetDependencies)
            {
                GetNextDependenced(ref target);
            }
            else if (mCurrentOption.isConfig)
            {
                Loaded++;
                StartLoad(out _);
            }
            else if (mCurrentOption.isRemote)
            {
                Loaded++;
                GetRemote(ref target);
            }
            else { }

            Load(out int statu);

            if (statu == 2)
            {
                CompleteEvent?.Invoke(true, this);
            }
            else { }
        }

        /// <summary>
        /// 获取刚加载好的远端服务器资源
        /// </summary>
        /// <param name="target"></param>
        private void GetRemote(ref Loader target)
        {
            "todo".Log("根据版本控制实时更新并加载");
            GetPersistentAResPath(out string path, ref mCurrentOption.relativeName);
            FileOperater.WriteBytes(target.ResultData, path);
            RemoteAssetUpdated?.Invoke(true, mCurrentOption.relativeName);
            mCurrentOption = default;
        }

        /// <summary>
        /// 获取下一个为记载过的依赖资源
        /// </summary>
        /// <param name="target"></param
        private void GetNextDependenced(ref Loader target)
        {
            bool flag = ABs.Add(target.Assets);
            "log:Asset bundle {0} is existed.".Log(!flag, target.Assets.name);

            if (mOpertions.Count > 0)
            {
                LoaderOpertion opertion = mOpertions.Dequeue();
                string source = opertion.relativeName;
                if (ABs.HasBundel(source))
                {
                    GetNextDependenced(ref target);
                }
                else
                {
                    Loaded++;
                    "loader deps".Log(source);
                    LoadByCurrentOperation(ref source);
                }
            }
            else
            {
                mCurrentOption = default;
            }
        }

        private void AddResList(ref string source, bool isCheckABs)
        {
            bool flag = isCheckABs ? !ABs.HasBundel(source) : true;
            if (!ResList.Contains(source) && flag)
            {
                "log: Assets loader res list created, name is {0}".Log(source);
                ResList.Add(source);
            }
            else { }
        }

        private void LoadByCurrentOperation(ref string source)
        {
            source = GetPathByLoaderOption(mCurrentOption, source);
            mLoader.Load(source);//加载依赖资源
        }

        /// <summary>
        /// 获取资源主依赖
        /// </summary>
        /// <param name="target"></param
        private void GetAssetManifest(ref Loader target)
        {
            ABs.Add(mCurrentOption.manifestName, target.Assets);
            AessetManifest = ABs.GetManifest();
            mCurrentOption = default;
        }

        public void FillResList(out List<string> result)
        {
            result = new List<string>();

            int max = ResList.Count;
            string resName;
            for (int i = 0; i < max; i++)
            {
                resName = ResList[i];
                result.Add(resName);
            }
        }
    }
}
