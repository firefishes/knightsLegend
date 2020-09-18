using ShipDock.Interfaces;
using UnityEngine;
using UnityEngine.Networking;

namespace ShipDock.Loader
{
    public class Loader : IDispose
    {
        public const int LOADER_ASSETBUNDLE = 0;
        public const int LOADER_TEXT = 1;
        public const int LOADER_AUDIO_CLIP = 2;
        public const int LOADER_TEXTURE = 3;
        public const int LOADER_HTTP_GET = 4;
        public const int LOADER_HTTP_POST = 5;
        public const int LOADER_DEFAULT = 6;

        public static int retryMax = 5;

        public static Loader GetAssetBundleLoader()
        {
            Loader result = new Loader(LOADER_ASSETBUNDLE);
            return result;
        }

        private UnityWebRequest mRequester;

        public Loader()
        {
        }

        public Loader(int loadType) : this()
        {
            InitLoader(loadType);
        }

        public void Dispose()
        {
            CompleteEvent.RemoveAllListeners();

            SetUrl(string.Empty);

            if (AysncOperation != default)
            {
                AysncOperation.completed -= CheckResult;
            }

            AysncOperation = default;
            mRequester = default;

            AudioClip = default;
            ResultData = default;
            Assets = default;
            TextData = string.Empty;
        }

        public void InitLoader(int loadType)
        {
            Dispose();
            SetLoadType(loadType);
        }

        public void Load(string url)
        {
            if (Url != url)
            {
                SetUrl(url);
            }
            else
            {
                return;
            }

            CreateLoader();
            StartLoad();
        }

        private void Abort()
        {
            mRequester?.Abort();
            if (IsLoading)
            {
                if(AysncOperation != default)
                {
                    AysncOperation.completed -= CheckResult;
                }
                AysncOperation = default;
            }
        }

        private void CreateLoader()
        {
            if (IsLoading)
            {
                return;
            }
            switch (LoadType)
            {
                case LOADER_ASSETBUNDLE:
                    mRequester = UnityWebRequestAssetBundle.GetAssetBundle(Url);
                    break;
                case LOADER_AUDIO_CLIP:
                    mRequester = UnityWebRequestMultimedia.GetAudioClip(Url, AudioType);//MP3 格式在PC平台下需要做转换再加载
                    break;
                case LOADER_TEXT:
                    mRequester = UnityWebRequest.Get(Url);
                    break;
                case LOADER_TEXTURE:
                    mRequester = UnityWebRequest.Get(Url);
                    mRequester.downloadHandler = new DownloadHandlerTexture(true);
                    break;
                default:
                    mRequester = new UnityWebRequest(Url)
                    {
                        downloadHandler = new DownloadHandlerBuffer()
                    };
                    break;
            }
        }

        private void CreateResult()
        {
            switch (LoadType)
            {
                case LOADER_ASSETBUNDLE:
                    Assets = DownloadHandlerAssetBundle.GetContent(mRequester);
                    break;
                case LOADER_TEXT:
                    TextData = mRequester.downloadHandler.text;
                    break;
                case LOADER_AUDIO_CLIP:
                    AudioClip = DownloadHandlerAudioClip.GetContent(mRequester);
                    break;
                case LOADER_TEXTURE:
                    DownloadHandlerTexture downloadHandler = mRequester.downloadHandler as DownloadHandlerTexture;
                    Texture2D = downloadHandler.texture;
                    TextureText = downloadHandler.text;
                    break;
                default:
                    DownloadHandlerBuffer handler = mRequester.downloadHandler as DownloadHandlerBuffer;
                    ResultData = handler.data;
                    break;
            }
        }

        private void StartLoad()
        {
            if (mRequester != null)
            {
                IsLoading = true;
                SetAsync();
                AysncOperation.completed += CheckResult;
            }
        }

        private void SetAsync()
        {
#if UNITY_5_6_5
            mAsyncOperation = mRequester.Send();
#elif UNITY_5_6_OR_NEWER
            AysncOperation = mRequester.SendWebRequest();
#endif
        }

        private void CheckResult(AsyncOperation sync)
        {
            IsLoading = false;
            if (IsError())
            {
                LoadFailed();
            }
            else
            {
                if ((mRequester.downloadHandler != default) && (mRequester.responseCode == 200))
                {
                    Loaded();
                }
                else
                {
                    RetryCount++;
                    if (RetryCount >= retryMax)
                    {
                        LoadFailed();
                    }
                    else
                    {
                        StartLoad();
                    }
                }
            }
        }

        protected virtual void Loaded()
        {
            AysncOperation.completed -= CheckResult;
            CreateResult();
            CompleteEvent.Invoke(true, this);
        }

        private bool IsError()
        {
#if UNITY_5_6_5
            return (mRequester.isError);
#elif UNITY_5_6_OR_NEWER
            return mRequester.isNetworkError || mRequester.isHttpError;
#else
            return false;
#endif
        }

        protected virtual void LoadFailed()
        {
            LoadError = mRequester.error;
            if (AlwaysRetry)
            {
                SetUrl(string.Empty);
                Load(Url);
            }
            else
            {
                AysncOperation.completed -= CheckResult;
                CompleteEvent.Invoke(false, this);
            }
        }

        public void SetLoadType(int type)
        {
            LoadType = type;
        }

        public void SetUrl(string url)
        {
            Abort();
            Url = url;
        }

        public float Progress
        {
            get
            {
                return AysncOperation != default ? AysncOperation.progress : 0f;
            }
        }
        
        protected int RetryCount { get; set; }

        public AsyncOperation AysncOperation { get; private set; }
        public OnLoaderCompleted CompleteEvent { get; private set; } = new OnLoaderCompleted();
        public AudioType AudioType { get; set; }
        public int LoadType { get; private set; } = LOADER_DEFAULT;
        public bool IsLoading { get; private set; }
        public bool AlwaysRetry { get; set; }
        public string LoadError { get; private set; }
        public string Url { get; private set; }
        public string TextData { get; private set; }
        public byte[] ResultData { get; private set; }
        public AssetBundle Assets { get; private set; }
        public AudioClip AudioClip { get; private set; }
        public Texture2D Texture2D { get; private set; }
        public string TextureText { get; private set; }
    }

}
