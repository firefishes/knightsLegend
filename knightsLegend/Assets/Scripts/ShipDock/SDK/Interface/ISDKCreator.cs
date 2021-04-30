#pragma warning disable
namespace ShipDock.SDK
{
    /// <summary>
    /// 
    /// SDK创建器
    /// 
    /// Created by 田亚宗 on 2019/05/23.
    ///
    /// </summary>
    public interface ISDKCreator
	{
        IPlatformSDK Create();
	}
}

