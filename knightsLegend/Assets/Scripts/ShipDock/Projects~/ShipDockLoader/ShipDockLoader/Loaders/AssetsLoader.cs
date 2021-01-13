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
        /// <summary>正在处理的依赖资源列表</summary>
        private List<string> mDependences;

        public string[] DirectDependencies { get; private set; }
        public OnAssetLoaderCompleted CompleteEvent { get; private set; } = new OnAssetLoaderCompleted();
        public OnRemoteAssetUpdated RemoteAssetUpdated { get; private set; } = new OnRemoteAssetUpdated();
        public AssetBundleManifest AessetManifest { get; private set; }
        public AssetBundles ABs { get; private set; }

        public AssetsLoader()
        {
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
                "warning:".Log("AssetBundles is null, will create new one.");
                ABs = new AssetBundles();
            }

            AessetManifest = ABs.GetManifest();
        }

        public void Dispose()
        {
            Utils.Reclaim(mLoader);

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
                mLoader = Loader.GetAssetBundleLoader();
                mLoader.CompleteEvent.AddListener(OnCompleted);
            }
        }

        private void InitDefaultLoader()
        {
            if (mLoader.LoadType != Loader.LOADER_DEFAULT)
            {
                if (mLoader != default)
                {
                    mLoader.Dispose();
                }
                mLoader = new Loader();
                mLoader.CompleteEvent.AddListener(OnCompleted);
            }
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
                LoaderOpertion opertion = new LoaderOpertion()
                {
                    remoteURL = url,
                    relativeName = relativeName,
                    isRemote = true,
                };
                "log: Loader add opertions remote file: {0}".Log(opertion.relativeName);
                mOpertions.Enqueue(opertion);
            }
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
                "log: Loader add opertions main manifest: {0}".Log(opertion.relativeName);
                mOpertions.Enqueue(opertion);
            }
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
                "log: Loader add opertions asset(dep): {0}".Log(opertion.relativeName);
                mOpertions.Enqueue(opertion);
            }
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
                "log: Loader add opertions config: {0}".Log(opertion.relativeName);
                mOpertions.Enqueue(opertion);
            }
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
                    InitDependencesList(source);
                    InitAssetLoader();
                    mIndex = 0;
                    source = GetValidSourceByIndex(ref source, ref mCurrentOption);
                    mIndex++;
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
                        "error".Log("Loader option error.");
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

        private string GetPathByLoaderOption(LoaderOpertion opertion, string source)
        {
            //TODO 根据版本号决定是缓存目录还是项目目录获取
            string path = opertion.isPersistentPath ?
                AppPaths.PersistentResDataRoot.Append(source) :
                AppPaths.StreamingResDataRoot.Append(source);
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
        private void InitDependencesList(string source)
        {
            Utils.Reclaim(ref mDependences);
            if (mDependences == default)
            {
                mDependences = new List<string>();
            }

            mDepsWalkMax = 0;
            string deped = source;
            WalkDependences(deped);
            mDependences.Add(source);
        }

        /// <summary>
        /// 遍历依赖资源列表，为加载所有依赖资源做准备
        /// </summary>
        /// <param name="deped"></param>
        private void WalkDependences(string deped)
        {
            mDepsWalkMax++;
            if (mDepsWalkMax > 100)
            {
                "walk deps".Log();
                return;
            }
            string[] list = AessetManifest.GetDirectDependencies(deped);
            int max = list.Length;
            string dep = string.Empty;
            for (int i = 0; i < max; i++)
            {
                dep = list[i];
                if (dep.Length > 0)
                {
                    "deps".Log(deped, dep);
                    WalkDependences(dep);
                    if (!mDependences.Contains(dep))
                    {
                        mDependences.Add(dep);
                    }
                }
            }
        }

        /// <summary>
        /// 获取有效的资源名
        /// 
        /// 无效的资源名包括：
        /// 1、要处理的依赖资源索引超限
        /// 2、传入了一个已加载过的资源名
        /// 3、标识了一个未缓存过的资源的资源名
        /// 
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        private string GetValidSourceByIndex(ref string source, ref LoaderOpertion opertion)
        {
            if (mIndex >= mDependences.Count)
            {
                return string.Empty;
            }
            source = mDependences[mIndex];
            if (ABs.HasBundel(source))
            {
                mIndex++;
                source = GetValidSourceByIndex(ref source, ref opertion);
            }
            else
            {
                source = GetPathByLoaderOption(opertion, source);
            }
            return source;
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
                GetAssetManifest(ref target);
            }
            else if (mCurrentOption.isGetDependencies)
            {
                GetNextDependenced(ref target);
            }
            else if (mCurrentOption.isConfig)
            {
                StartLoad(out _);
            }
            else
            {
                GetRemote(ref target);
            }

            Load(out int statu);
            if (statu == 2)
            {
                CompleteEvent?.Invoke(true, this);
            }
        }

        /// <summary>
        /// 获取刚加载好的远端服务器资源
        /// </summary>
        /// <param name="target"></param>
        private void GetRemote(ref Loader target)
        {
            //TODO 版本控制
            string path = AppPaths.PersistentResDataRoot.Append(mCurrentOption.relativeName);
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
            ABs.Add(target.Assets);
            if (mIndex < mDependences.Count)
            {
                string source = mDependences[mIndex];
                mIndex++;
                if (ABs.HasBundel(source))
                {
                    GetNextDependenced(ref target);
                }
                else
                {
                    "loader deps".Log(source);
                    LoadByCurrentOperation(ref source);
                }
            }
            else
            {
                mCurrentOption = default;
            }
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
    }
}
