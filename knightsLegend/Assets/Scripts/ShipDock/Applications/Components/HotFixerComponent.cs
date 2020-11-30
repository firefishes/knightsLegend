using ILRuntime.Runtime.Enviorment;
using ShipDock.Loader;
using UnityEngine;

namespace ShipDock.Applications
{
    public class HotFixerComponent : HotFixer
    {
        [SerializeField]
        protected string m_HotFixABName;
        [SerializeField]
        protected string m_HotFixDLL;
        [SerializeField]
        protected string m_HotFixPDB;

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

        protected override void Init()
        {
            base.Init();

            AssetBundles abs = ShipDockApp.Instance.ABs;
            TextAsset dll = abs.Get<TextAsset>(m_HotFixABName, m_HotFixDLL);
            TextAsset pdb = abs.Get<TextAsset>(m_HotFixABName, m_HotFixPDB);

            StartHotfix(dll.bytes, pdb.bytes);
        }
    }

}