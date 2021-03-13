
using ILRuntime.Runtime.Enviorment;
using ShipDock.Loader;
using ShipDock.Notices;
using ShipDock.Pooling;
using UnityEngine;

namespace ShipDock.Applications
{
    public class HotFixerComponent : HotFixer
    {

        [SerializeField]
        private HotFixerSubgroup m_Settings = new HotFixerSubgroup();
        [SerializeField]
        private HotFixerLoadedNoticeInfo m_LoadedNoticeInfo = new HotFixerLoadedNoticeInfo();

        public TextAsset hotFixAsset;

        private ComponentBridge mCompBridge;
        private INoticeBase<int> mIDAsNotice;

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
            m_Settings = default;
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

            TextAsset dll, pdb = default;
            if (hotFixAsset != default)
            {
                dll = hotFixAsset;
            }
            else
            {
                AssetBundles abs = ShipDockApp.Instance.ABs;
                dll = abs.Get<TextAsset>(m_Settings.HotFixABName, m_Settings.HotFixDLL);
                pdb = abs.Get<TextAsset>(m_Settings.HotFixABName, m_Settings.HotFixPDB);
            }

            StartHotFixeByAsset(this, dll, pdb);
        }

        protected override void ILRuntimeLoaded()
        {
            base.ILRuntimeLoaded();

            SendLoadedNotice();
        }

        private void SendLoadedNotice()
        {
            if (m_LoadedNoticeInfo.IsSendIDAsNotice)
            {
                if (m_LoadedNoticeInfo.ApplyDefaultNoticeType)
                {
                    mIDAsNotice = Pooling<Notice>.From();
                }
                else
                {
                    ApplyCustomNotice();
                }

                if (m_LoadedNoticeInfo.ApplyCallLate)
                {
                    UpdaterNotice.SceneCallLater(SendLoadedNoticeAndRelease);
                }
                else
                {
                    SendLoadedNoticeAndRelease(0);
                }
            }
            else { }
        }

        private void SendLoadedNoticeAndRelease(int time)
        {
            if (m_Settings == default)
            {
                return;
            }
            else { }

            int noticeName = m_LoadedNoticeInfo.ApplyGameObjectID ? gameObject.GetInstanceID() : GetInstanceID();
            noticeName.Broadcast(mIDAsNotice);
            if (mIDAsNotice is IPoolable item)
            {
                item.ToPool();
            }
            else
            {
                mIDAsNotice?.Dispose();
            }
        }

        private void ApplyCustomNotice()
        {
            string methodName = m_LoadedNoticeInfo.GetIDAsCustomNoticeMethod;
            if (string.IsNullOrEmpty(methodName))
            {
                mIDAsNotice = Pooling<Notice>.From();
            }
            else
            {
                ILRuntimeUtils.InvokeMethodILR(ShellBridge, m_StartUpInfo.ClassName, methodName, 0, OnGetIDAsNoticeHandler);
                if (mIDAsNotice == default)
                {
                    mIDAsNotice = Pooling<Notice>.From();
                }
                else { }
            }
        }

        private void OnGetIDAsNoticeHandler(InvocationContext context)
        {
            mIDAsNotice = context.ReadObject<INoticeBase<int>>();
        }

        public ValueSubgroup GetDataField(string keyField)
        {
            return m_Settings.GetDataField(ref keyField);
        }

        public SceneNodeSubgroup GetSceneNode(string keyField)
        {
            return m_Settings.GetSceneNode(ref keyField);
        }

        public void DisableReadyNotice()
        {
            m_LoadedNoticeInfo?.DisableReadyNotice();
        }
    }
}