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
        private string m_UIInteractorName;

        public HotFixerUI Bridge { get; private set; }

        public string UIInteractorName
        {
            get
            {
                return m_UIInteractorName;
            }
        }

        protected override void Awake()
        {
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

        protected override void InitILRuntime()
        {
            base.InitILRuntime();

            Bridge = gameObject.AddComponent<HotFixerUI>();
            Bridge.SetHotFixerAgent(this);
        }
    }
}