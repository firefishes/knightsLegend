using ShipDock.Notices;
using UnityEngine;

namespace ShipDock.Applications
{
    /// <summary>
    /// UI热更代理组件，用于加载热更端的类
    /// </summary>
    public class HotFixerUIAgent : HotFixerComponent, INotificationSender
    {
        [SerializeField]
        private bool m_EnableReadyNotice;
        [SerializeField]
        private string m_UIModularName;
        [SerializeField]
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
            m_StartUpInfo.ApplyClassName = false;//不使用热更类启动，使用 m_UIInteractorName 定义的类名启动

            base.Awake();

            if (!m_EnableReadyNotice)
            {
                DisableReadyNotice();
            }
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