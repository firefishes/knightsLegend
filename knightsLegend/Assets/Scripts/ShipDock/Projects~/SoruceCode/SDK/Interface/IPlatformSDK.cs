#pragma warning disable
namespace ShipDock.SDK
{
    /// <summary>
    /// 
    /// 三方sdk平台接口
    /// 
    /// Created by 田亚宗 on 2019/05/23.
    /// Modifie by Minghua.ji on 2021/3/29
    ///
    /// </summary>
    public interface IPlatformSDK
	{
        void Init();
	}

    public enum PlatformType
    {
        Default =-1,
        Android = 0,
        IOS = 1
    }
}

