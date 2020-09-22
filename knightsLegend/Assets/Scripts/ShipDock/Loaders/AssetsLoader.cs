#define G_LOG

using ShipDock.Applications;
using ShipDock.Interfaces;
using ShipDock.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Loader
{
    public class AssetsLoader : IDispose
    {

        private int mIndex;
        private int mDepsWalkMax;
        private Loader mLoader;
        private LoaderOpertion mCurrentOption;
        private Queue<LoaderOpertion> mOpertions;
        private List<string> mDependences;

        public AssetsLoader()
        {
            mOpertions = new Queue<LoaderOpertion>();
            mLoader = Loader.GetAssetBundleLoader();
            mLoader.CompleteEvent.AddListener(OnCompleted);

            if (ShipDockApp.Instance.IsStarted)
            {
                ABs = ShipDockApp.Instance.ABs;
            }
            else
            {
                ABs = new AssetBundles();
            }
            AessetManifest = ABs.GetManifest();
        }

        public void Dispose()
        {
            Utils.Reclaim(mLoader);

            mLoader = default;
        }

        public AssetsLoader AddRemote(string url)
        {
            if (mLoader != default)
            {
                LoaderOpertion opertion = new LoaderOpertion()
                {
                    url = url
                };
                mOpertions.Enqueue(opertion);
            }
            return this;
        }

        public AssetsLoader Add(string relativeName, string manifestName)
        {
            if (mLoader != default)
            {
                LoaderOpertion opertion = new LoaderOpertion()
                {
                    manifestName = manifestName,
                    relativeName = relativeName,
                    isManifest = true,
                };
                mOpertions.Enqueue(opertion);
            }
            return this;
        }

        public AssetsLoader Add(string relativeName, bool isDependenciesLoader = true)
        {
            if(mLoader != default)
            {
                LoaderOpertion opertion = new LoaderOpertion()
                {
                    relativeName = relativeName,
                    isGetDependencies = isDependenciesLoader
                };
                mOpertions.Enqueue(opertion);
            }
            return this;
        }

        public void Load(out int statu)
        {
            statu = 0;
            if (mCurrentOption == default)
            {
                LoadDependeceItem(out statu);
            }
            else
            {
                statu = 1;
            }
        }

        private void LoadDependeceItem(out int statu)
        {
            statu = 0;
            if ((mOpertions != default) && (mOpertions.Count > 0))
            {
                mCurrentOption = mOpertions.Dequeue();

                string source = mCurrentOption.relativeName;
                if (mCurrentOption.isGetDependencies)
                {
                    InitDependencesList(source);
                    mIndex = 0;
                    source = GetValidSourceByIndex(ref source);
                    mIndex++;
                }
                else if (!mCurrentOption.isManifest)
                {
                    source = mCurrentOption.url;
                }

                if(!string.IsNullOrEmpty(source))
                {
                    "load res".Log(source);
                    mLoader.Load(source);
                }
                else
                {
                    statu = 2;
                }
            }
            else
            {
                statu = 2;
            }
        }

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
                    "deps".Log(dep, deped);
                    WalkDependences(dep);
                    if (!mDependences.Contains(dep))
                    {
                        mDependences.Add(dep);
                    }
                }
            }
        }

        private string GetValidSourceByIndex(ref string source)
        {
            if(mIndex >= mDependences.Count)
            {
                "empty deps".Log();
                return string.Empty;
            }
            source = mDependences[mIndex];
            if (ABs.HasBundel(source))
            {
                mIndex++;
                source = GetValidSourceByIndex(ref source);
            }
            else
            {
                source = AppPaths.StreamingResDataRoot.Append(source);//TODO 根据版本号决定是缓存目录还是项目目录获取
            }
            return source;
        }

        private void OnCompleted(bool isSuccessd, Loader target)
        {
            if(isSuccessd)
            {
                LoadSuccessd(ref target);
            }
            else
            {
                LoadFailed(ref target);
            }
        }

        private void LoadFailed(ref Loader target)
        {
            "error".Log(target.LoadError);
            "loader failed".Log(mLoader.Url);
            CompleteEvent?.Invoke(false, this);
        }

        private void LoadSuccessd(ref Loader target)
        {
            "loader success".Log(target.Url);
            if (mCurrentOption.isManifest)
            {
                GetAssetManifest(ref target);
            }
            else if(mCurrentOption.isGetDependencies)
            {
                GetNextDependenced(ref target);
            }
            else
            {
                GetRemote(ref target);
            }

            Load(out int statu);
            if(statu == 2)
            {
                CompleteEvent?.Invoke(true, this);
            }
        }

        private void GetRemote(ref Loader target)
        {
            ABs.Add(target.Assets);
            //TODO 版本控制
            mCurrentOption = default;
        }

        private void GetNextDependenced(ref Loader target)
        {
            ABs.Add(target.Assets);
            if(mIndex < mDependences.Count)
            {
                string source = mDependences[mIndex];
                mIndex++;
                if(ABs.HasBundel(source))
                {
                    GetNextDependenced(ref target);
                }
                else
                {
                    "loader deps".Log(source);
                    mLoader.Load(AppPaths.StreamingResDataRoot.Append(source));//TODO 根据版本号决定是缓存目录还是项目目录获取，拼接不同的地址
                }
            }
            else
            {
                mCurrentOption = default;
            }
        }

        private void GetAssetManifest(ref Loader target)
        {
            ABs.Add(mCurrentOption.manifestName, target.Assets);
            AessetManifest = ABs.GetManifest();
            mCurrentOption = default;
        }
        
        public string[] DirectDependencies { get; private set; }
        public OnAssetLoaderCompleted CompleteEvent { get; private set; } = new OnAssetLoaderCompleted();
        public AssetBundleManifest AessetManifest { get; private set; }
        public AssetBundles ABs { get; private set; }
    }
}
