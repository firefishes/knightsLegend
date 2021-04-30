#pragma warning disable
namespace ShipDock.SDK
{
    /// <summary>
    /// 
    /// <-请描述该类的功能->
    /// 
    /// Created by 田亚宗 on 2019/11/27.
    ///
    /// </summary>
    public sealed class MacCreator : ISDKCreator
    {
        public IPlatformSDK Create()
        {
            return new SDKMac();
        }
    }
}

