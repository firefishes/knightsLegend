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
    public sealed class SDKCreatorFactory
	{
        public static ISDKCreator GetCreator(PlatformType type)
        {
            switch (type)
            {
                case PlatformType.IOS:
                    return new IOSCreator();
                case PlatformType.Android:
                    return new AndroidCreator();
                default:
                    return new MacCreator();
            }
        }
    }	
}

