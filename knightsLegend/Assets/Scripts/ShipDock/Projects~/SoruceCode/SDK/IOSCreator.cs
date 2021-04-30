#pragma warning disable
namespace ShipDock.SDK
{
    /// <summary>
    /// 
    /// 
    /// 
    /// Created by 田亚宗 on 2019/05/23.
    ///
    /// </summary>
    public sealed class IOSCreator : ISDKCreator
    {
        IPlatformSDK ISDKCreator.Create()
        {
            IPlatformSDK sdk = new SDKIos();
            sdk.Init();
            return sdk;
        }
    }
}

