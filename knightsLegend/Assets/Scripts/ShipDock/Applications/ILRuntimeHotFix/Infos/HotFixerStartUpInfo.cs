using UnityEngine;

namespace ShipDock.Applications
{
    [System.Serializable]
    public class HotFixerStartUpInfo
    {
        [SerializeField]
        [Tooltip("是否脱离框架单独运行")]
        protected bool m_ApplyRunStandalone;
        [SerializeField]
        [Tooltip("是否一启动立刻加载热更逻辑")]
        protected bool m_RunInAwake = true;
        [SerializeField]
        [Tooltip("热更端的类名")]
        protected string m_ClassName;
        [SerializeField]
        [Tooltip("主项目启动热更端的入口方法")]
        protected string m_IniterMethodName = "ShellInited";
        [SerializeField]
        [Tooltip("是否应用固定帧更新回调方法")]
        private bool m_ApplyFixedUpdate;
        [SerializeField]
        [Tooltip("固定帧更新回调方法名")]
        private string m_FixedUpdateMethodName = "FixedUpdate";
        [SerializeField]
        [Tooltip("是否应用帧更新回调方法")]
        private bool m_ApplyUpdate;
        [SerializeField]
        [Tooltip("帧更新回调方法名")]
        private string m_UpdateMethodName = "Update";
        [SerializeField]
        [Tooltip("是否应用延迟帧更新回调方法")]
        private bool m_ApplyLateUpdate;
        [SerializeField]
        [Tooltip("延迟帧更新回调方法名")]
        private string m_LateUpdateMethodName = "LateUpdate";

        public bool ApplyRunStandalone
        {
            get
            {
                return m_ApplyRunStandalone;
            }
            set
            {
                m_ApplyRunStandalone = value;
            }
        }

        public string ClassName
        {
            get
            {
                return m_ClassName;
            }
        }

        public string IniterMethodName
        {
            get
            {
                return m_IniterMethodName;
            }
        }

        public bool ApplyFixedUpdate
        {
            get
            {
                return m_ApplyFixedUpdate;
            }
        }

        public bool ApplyUpdate
        {
            get
            {
                return m_ApplyUpdate;
            }
        }

        public bool ApplyLateUpdate
        {
            get
            {
                return m_ApplyLateUpdate;
            }
        }

        public string FixedUpdateMethodName
        {
            get
            {
                return m_FixedUpdateMethodName;
            }
        }

        public string UpdateMethodName
        {
            get
            {
                return m_UpdateMethodName;
            }
        }

        public string LateUpdateMethodName
        {
            get
            {
                return m_LateUpdateMethodName;
            }
        }

        public bool RunInAwake
        {
            get
            {
                return m_RunInAwake;
            }
        }
    }
}