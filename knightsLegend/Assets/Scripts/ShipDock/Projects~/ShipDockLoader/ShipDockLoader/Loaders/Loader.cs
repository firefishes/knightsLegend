using ShipDock.Interfaces;
using System.Collections;
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

            Abort();
            Url = string.Empty;

            AfterCheckResult();

            ApplyAssetBundleVersion = false;
            Asyncer = default;
            mRequester = default;

            AudioClip = default;
            ResultData = default;
            TextData = default;
            Assets = default;
            TextData = string.Empty;
        }

        private void AfterCheckResult()
        {
            if (Asyncer != default)
            {
                Asyncer.completed -= CheckResult;
            }
        }

        private void BeforeCheckResult()
        {
            if (Asyncer != default)
            {
                Asyncer.completed += CheckResult;
            }
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
                AfterCheckResult();
                Asyncer = default;
            }
        }

        private void CreateLoader()
        {
            if (!IsLoading)
            {
                switch (LoadType)
                {
                    case LOADER_ASSETBUNDLE:
                        mRequester?.Abort();
                        mRequester = default;
                        break;
                    case LOADER_AUDIO_CLIP:
                        mRequester = UnityWebRequestMultimedia.GetAudioClip(Url, AudioType);//MP3 格式在PC平台下需要做转换再加载
                        break;
                    case LOADER_TEXT:
                        mRequester = UnityWebRequest.Get(Url);
                        break;
                    case LOADER_TEXTURE:
                        mRequester = UnityWebRequestTexture.GetTexture(Url);
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
        }

        private void CreateResult()
        {
            switch (LoadType)
            {
                case LOADER_ASSETBUNDLE:
                    AssetBundleCreateRequest sync = Asyncer as AssetBundleCreateRequest;
                    Assets = sync.assetBundle;
                    break;
                case LOADER_TEXT:
                    DownloadHandler download = mRequester.downloadHandler;
                    SetResultData(download.data, download.text);
                    break;
                case LOADER_AUDIO_CLIP:
                    AudioClip = DownloadHandlerAudioClip.GetContent(mRequester);
                    break;
                case LOADER_TEXTURE:
                    Texture2D = DownloadHandlerTexture.GetContent(mRequester);
                    TextureText = mRequester.downloadHandler.text;

                    byte[] vs = Texture2D.GetRawTextureData();
                    SetResultData(vs, TextureText);
                    break;
                default:
                    DownloadHandlerBuffer handler = mRequester.downloadHandler as DownloadHandlerBuffer;
                    SetResultData(handler.data, handler.text);
                    break;
            }
        }

        private void SetResultData(byte[] vs, string text)
        {
            "error: Loader get 0 length bytes data. url is {0}".Log(vs.Length == 0, Url);
            ResultData = vs;
            TextData = text;
        }

        private void StartLoad()
        {
            if (mRequester != null)
            {
                IsLoading = true;
                SetAsync();
                BeforeCheckResult();
            }
            else
            {
                if (LoadType == LOADER_ASSETBUNDLE)
                {
                    IsLoading = true;
                    Asyncer = AssetBundle.LoadFromFileAsync(Url);
                    BeforeCheckResult();
                }
            }
        }

        private void SetAsync()
        {
#if UNITY_5_6_5
            mAsyncOperation = mRequester.Send();
#elif UNITY_5_6_OR_NEWER
            Asyncer = mRequester.SendWebRequest();
#endif
        }

        private IEnumerator AsyncLoad(UnityWebRequest request)
        {
            yield return request.SendWebRequest();

            CheckResult(default);
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
                bool flag = false;
                if (mRequester != default)
                {
                    flag = (mRequester.downloadHandler != default) && (mRequester.responseCode == 200);
                }
                else if(Asyncer != default)
                {
                    flag = Asyncer.isDone;
                }
                if (flag)
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
            AfterCheckResult();
            CreateResult();
            CompleteEvent.Invoke(true, this);
        }

        private bool IsError()
        {
            if (mRequester != default)
            {
#if UNITY_5_6_5
                return (mRequester.isError);
#elif UNITY_5_6_OR_NEWER
                return mRequester.isNetworkError || mRequester.isHttpError;
#else
                return false;
#endif
            }
            else
            {
                return false;
            }
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
                AfterCheckResult();
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
                return Asyncer != default ? Asyncer.progress : 0f;
            }
        }
        
        protected int RetryCount { get; set; }

        public AsyncOperation Asyncer { get; private set; }
        public OnLoaderCompleted CompleteEvent { get; private set; } = new OnLoaderCompleted();
        public AudioType AudioType { get; set; }
        public int LoadType { get; private set; } = LOADER_DEFAULT;
        public uint AssetBundleVersion { get; set; }
        public bool IsLoading { get; private set; }
        public bool AlwaysRetry { get; set; }
        public bool ApplyAssetBundleVersion { get; set; }
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
