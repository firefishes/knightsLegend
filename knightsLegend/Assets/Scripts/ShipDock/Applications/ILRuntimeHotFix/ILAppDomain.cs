using ILRuntime.Runtime.Enviorment;

namespace ShipDock.Applications
{
    /// <summary>
    /// 
    /// 运行时热更管理器，用于实现全局可访问的 AppDomain
    /// 
    /// </summary>
    public class ILRuntimeHotFix
    {
        public AppDomain ILAppDomain { get; private set; }
        public ILMethodCacher MethodCacher { get; private set; }

        public ILRuntimeHotFix(IAppILRuntime app)
        {
            this.InitFromApp(app);

            ILAppDomain = new AppDomain();
            MethodCacher = new ILMethodCacher();
        }

        /// <summary>
        /// 自有框架或应用退出时调用此方法
        /// </summary>
        public void Clear()
        {
            this.ClearExtension();
            MethodCacher?.Clear();

            MethodCacher = default;
            ILAppDomain = default;
        }
    }
}