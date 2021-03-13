using UnityEngine;

namespace ShipDock.Applications
{
    [System.Serializable]
    public class HotFixerStartUpInfo
    {
        [Header("热更端桥接类、方法信息")]
        [SerializeField]
        [Tooltip("是否脱离框架单独运行")]
        protected bool m_ApplyRunStandalone;
        [SerializeField]
        [Tooltip("是否一启动立刻加载热更逻辑")]
        protected bool m_RunInAwake = true;
        [SerializeField]
        [Tooltip("是否应用启动的热更类名")]
        private bool m_ApplyClassName = true;
        [SerializeField]
        [Tooltip("热更端的类名")]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf("m_ApplyClassName", true)]
#endif
        protected string m_ClassName;
        [SerializeField]
        [Tooltip("主项目启动热更端的入口方法")]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf("m_ApplyClassName", true)]
#endif
        protected string m_IniterMethodName = "ShellInited";
        [SerializeField]
        [Tooltip("是否应用固定帧更新回调方法")]
        private bool m_ApplyFixedUpdate;
        [SerializeField]
        [Tooltip("固定帧更新回调方法名")]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf("m_ApplyFixedUpdate", true)]
#endif
        private string m_FixedUpdateMethodName = "FixedUpdate";
        [SerializeField]
        [Tooltip("是否应用帧更新回调方法")]
        private bool m_ApplyUpdate;
        [SerializeField]
        [Tooltip("帧更新回调方法名")]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf("m_ApplyUpdate", true)]
#endif
        private string m_UpdateMethodName = "Update";
        [SerializeField]
        [Tooltip("是否应用延迟帧更新回调方法")]
        private bool m_ApplyLateUpdate;
        [SerializeField]
        [Tooltip("延迟帧更新回调方法名")]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ShowIf("m_ApplyLateUpdate", true)]
#endif
        private string m_LateUpdateMethodName = "LateUpdate";
        [Tooltip("调试端口")]
        private int m_DebugPort = 56000;

        public int DebugPort
        {
            get
            {
                return m_DebugPort;
            }
        }

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
            set
            {
                m_ApplyFixedUpdate = value;
            }
        }

        public bool ApplyUpdate
        {
            get
            {
                return m_ApplyUpdate;
            }
            set
            {
                m_ApplyUpdate = value;
            }
        }

        public bool ApplyLateUpdate
        {
            get
            {
                return m_ApplyLateUpdate;
            }
            set
            {
                m_ApplyLateUpdate = value;
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

        public bool ApplyClassName
        {
            get
            {
                return m_ApplyClassName;
            }
            set
            {
                m_ApplyClassName = value;
            }
        }
    }
}