
namespace ShipDock.Applications
{
    /// <summary>
    /// 热更特性应用接口，使框架核心单例或其他单例实现此接口
    /// </summary>
    public interface IAppILRuntime
    {
        ILRuntimeHotFix ILRuntimeHotFix { get; }
    }
}