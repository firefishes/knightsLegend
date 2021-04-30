#pragma warning disable
namespace ShipDock.SDK
{
    /// <summary>
    /// 
    /// Android SDK 交互对象创建器
    /// 
    /// Created by 田亚宗 on 2019/05/23.
    ///
    /// </summary>
    public sealed class AndroidCreator : ISDKCreator
    {
        IPlatformSDK ISDKCreator.Create()
        {
            IPlatformSDK sdk = new SDKAndroid();
            sdk.Init();
            return sdk;
        }
    }
}

