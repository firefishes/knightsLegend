using ShipDock.Notices;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace ShipDock.Applications
{
    /// <summary>
    /// UI热更代理组件，用于加载热更端的类
    /// </summary>
    public class HotFixerUIAgent : HotFixerComponent, INotificationSender
    {
#if ODIN_INSPECTOR
        private void OnEnabledReadyValueChange()
        {
            if (m_EnableReadyNotice)
            {
                EnableReadyNotice();
            }
            else
            {
                DisableReadyNotice();
            }
        }

        [TitleGroup("UI 热更"), LabelText("启用发送就绪消息功能（默认不开启）"), OnValueChanged("OnEnabledReadyValueChange")]
#endif
        [SerializeField]
        private bool m_EnableReadyNotice;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("UI 模块名")]
#endif
        private string m_UIModularName;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("UI 界面交互名")]
#endif
        private string m_UIInteractorName;

        public HotFixerUI Bridge { get; private set; }

        public string UIInteractorName
        {
            get
            {
                return m_UIInteractorName;
            }
        }

        public string UIModularName
        {
            get
            {
                return m_UIModularName;
            }
        }

        protected override void Awake()
        {
            m_StartUpInfo.ApplyClassName = false;//不使用热更端的类启动，而使用 m_UIInteractorName 定义的类名启动

            base.Awake();

            if (!m_EnableReadyNotice)
            {
                DisableReadyNotice();
            }
            else { }
        }

        protected override void Purge()
        {
            base.Purge();

            Bridge = default;
        }

        protected override void ILRuntimeLoaded()
        {
            Bridge = gameObject.AddComponent<HotFixerUI>();
            Bridge.SetHotFixerAgent(this);

            base.ILRuntimeLoaded();
        }
    }
}