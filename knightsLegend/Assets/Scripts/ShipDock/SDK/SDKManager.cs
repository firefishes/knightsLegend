using ShipDock.Tools;
using UnityEngine;

#pragma warning disable
namespace ShipDock.SDK
{
    /// <summary>
    /// 
    /// SDK管理器
    /// 
    /// add by Tian Yazong
    /// merge by Minghua.ji
    /// 
    /// </summary>
    public class SDKManager : Singletons<SDKManager>
    {
        private IPlatformSDK mPlatformSdk = null;
        private IPlatformSDK mDetectFace = null;
        private IPlatformSDK mHuaweiSdk = null;

        public SDKManager()
        {
            ISDKCreator creator = null;

            switch(Application.platform)
            {
                case RuntimePlatform.IPhonePlayer:
                    creator = SDKCreatorFactory.GetCreator(PlatformType.IOS);
                    break;
                case RuntimePlatform.Android:
                    creator = SDKCreatorFactory.GetCreator(PlatformType.Android);
                    break;
                default:
                    creator = SDKCreatorFactory.GetCreator(PlatformType.Default);
                    break;
            }

            if(creator != null)
            {
                mPlatformSdk = creator.Create();
            }
            else { }
        }

        public T GetSDK<T>()
        {
            if (typeof(T).Name == nameof(IDetectFaceSDK))
            {
                if (mDetectFace == null)
                {
                    mDetectFace = new DetectFaceSDK();
                    mDetectFace.Init();
                }
                else { }

                return (T)mDetectFace;
            }
            else if (typeof(T).Name == nameof(IHuaweiSDK))
            {
                if (mHuaweiSdk == null)
                {
                    mHuaweiSdk = new SDKHuawei();
                    mHuaweiSdk.Init();
                }
                else { }

                return (T)mHuaweiSdk;
            }
            else { }

            return (T)mPlatformSdk;
        }

    }

}

