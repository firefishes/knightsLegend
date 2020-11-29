using ILRuntime.Runtime.Enviorment;

namespace ShipDock.Applications
{
    public class HotFixerComponent : HotFixer
    {
        private AppDomain mILRuntimeAppDomain;
        private ComponentBridge mCompBridge;

        public override AppDomain Enviorment()
        {
            if (mILRuntimeAppDomain == default)
            {
                mILRuntimeAppDomain = ShipDockApp.Instance.ILRuntimeHotFix.ILAppDomain;
            }
            return mILRuntimeAppDomain;
        }

        protected override void Awake()
        {
            m_ApplyRunStandalone = false;

            base.Awake();
        }

        protected override void Purge()
        {
            mCompBridge?.Dispose();
            mCompBridge = default;

            mILRuntimeAppDomain = default;
        }

        protected override void RunWithinFramework()
        {
            mCompBridge = new ComponentBridge(Init);
            mCompBridge.Start();
        }

        protected override void InitILRuntime()
        {
            base.InitILRuntime();

            Enviorment().DelegateManager.RegisterMethodDelegate<int>();
        }
    }

}