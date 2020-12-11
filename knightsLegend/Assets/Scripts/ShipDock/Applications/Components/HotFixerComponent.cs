
using ILRuntime.Runtime.Enviorment;
using ShipDock.Loader;
using UnityEngine;

namespace ShipDock.Applications
{
    public class HotFixerComponent : HotFixer
    {
        [SerializeField]
        private HotFixerSubgroup m_Settings = new HotFixerSubgroup();

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
            m_StartUpInfo.ApplyRunStandalone = false;

            base.Awake();

#if UNITY_EDITOR
            m_Settings?.Sync();
#endif
        }

        protected override void Purge()
        {
            mCompBridge?.Dispose();
            mCompBridge = default;
            m_Settings.Clear();

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

            ShipDockApp.Instance.ILRuntimeHotFix.Start();
        }

        protected override void Init()
        {
            base.Init();

            m_Settings.Init();

            AssetBundles abs = ShipDockApp.Instance.ABs;
            TextAsset dll = abs.Get<TextAsset>(m_Settings.HotFixABName, m_Settings.HotFixDLL);

#if RELEASE
            StartHotfix(dll.bytes, default);//正式发布不加载pdb文件
#else
            TextAsset pdb = abs.Get<TextAsset>(m_Settings.HotFixABName, m_Settings.HotFixPDB);
            StartHotfix(dll.bytes, pdb.bytes);
#endif
#if LOG_HOT_FIX_COMP_START
            "HotFixer InstantiateFromIL, class name is {0}".Log(m_StartUpInfo.ClassName);
            "HotFixer {0} loaded.".Log(mShellBridge.ToString());
#endif
        }

        public ValueSubgroup GetDataField(string keyField)
        {
            return m_Settings.GetDataField(ref keyField);
        }

        public SceneNodeSubgroup GetSceneNode(string keyField)
        {
            return m_Settings.GetSceneNode(ref keyField);
        }
    }
}