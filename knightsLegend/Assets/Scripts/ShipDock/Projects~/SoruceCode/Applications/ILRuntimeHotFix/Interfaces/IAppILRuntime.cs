namespace ShipDock.Applications
{
    /// <summary>
    /// 热更特性应用接口，使框架核心单例或其他单例实现此接口
    /// </summary>
    public interface IAppILRuntime
    {
        IHotFixConfig GetHotFixConfig();
        void SetHotFixSetting(ILRuntimeHotFix value, IHotFixConfig config);
        ILRuntimeHotFix ILRuntimeHotFix { get; }
    }
}