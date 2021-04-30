#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using UnityEngine;

namespace ShipDock.Applications
{
    [System.Serializable]
    public class HotFixerStartUpInfo
    {
#if !ODIN_INSPECTOR
        [Header("热更端桥接类、方法信息")]
#endif
        [SerializeField, Tooltip("是否脱离框架单独运行")]
#if ODIN_INSPECTOR
        [LabelText("脱离框架运行")]
#endif
        protected bool m_ApplyRunStandalone;

        [SerializeField, Tooltip("是否一启动立刻加载热更逻辑")]
#if ODIN_INSPECTOR
        [LabelText("在Awake时启动")]
#endif
        protected bool m_RunInAwake = true;

        [SerializeField, Tooltip("是否应用启动的热更类名")]
#if ODIN_INSPECTOR
        [LabelText("启用热更端类名")]
#endif
        private bool m_ApplyClassName = true;

        [SerializeField, Tooltip("热更端的类名")]
#if ODIN_INSPECTOR
        [LabelText("热更端类名"), ShowIf("m_ApplyClassName", true), SuffixLabel("须包含命名空间", overlay: true)]
#endif
        protected string m_ClassName;

        [SerializeField, Tooltip("主项目启动热更端的入口方法")]
#if ODIN_INSPECTOR
        [LabelText("热更端对接的方法"), ShowIf("m_ApplyClassName", true), HideIf("@this.m_IsMonoBehaviorMode == true")]
#endif
        protected string m_IniterMethodName = "ShellInited";

        [SerializeField, Tooltip("是否应用固定帧更新回调方法")]
#if ODIN_INSPECTOR
        [LabelText("启用 FixedUpdate 模拟方法"), HideIf("@this.m_IsMonoBehaviorMode == true")]
#endif
        private bool m_ApplyFixedUpdate;

        [SerializeField, Tooltip("固定帧更新回调方法名")]
#if ODIN_INSPECTOR
        [LabelText("FixedUpdate 方法名"), ShowIf("m_ApplyFixedUpdate", true), HideIf("@this.m_IsMonoBehaviorMode == true")]
#endif
        private string m_FixedUpdateMethodName = "FixedUpdate";

        [SerializeField, Tooltip("是否应用帧更新回调方法")]
#if ODIN_INSPECTOR
        [LabelText("启用 Update 模拟方法"), HideIf("@this.m_IsMonoBehaviorMode == true")]
#endif
        private bool m_ApplyUpdate;

        [SerializeField, Tooltip("帧更新回调方法名")]
#if ODIN_INSPECTOR
        [LabelText("Update 方法名"), ShowIf("m_ApplyUpdate", true), HideIf("@this.m_IsMonoBehaviorMode == true")]
#endif
        private string m_UpdateMethodName = "Update";

        [SerializeField, Tooltip("是否应用延迟帧更新回调方法")]
#if ODIN_INSPECTOR
        [LabelText("启用 LateUpdate 模拟方法"), HideIf("@this.m_IsMonoBehaviorMode == true")]
#endif
        private bool m_ApplyLateUpdate;

        [SerializeField, Tooltip("延迟帧更新回调方法名")]
#if ODIN_INSPECTOR
        [LabelText("LateUpdate 方法名"), ShowIf("m_ApplyLateUpdate", true), HideIf("@this.m_IsMonoBehaviorMode == true")]
#endif
        private string m_LateUpdateMethodName = "LateUpdate";

        [SerializeField, Tooltip("MonoBehavior 模式下热更端类名必须设置为对应的 MonoBehavior 子类")]
#if ODIN_INSPECTOR
        [LabelText("启用 MonoBehavior 模式")]
#endif
        private bool m_IsMonoBehaviorMode = false;

        [Tooltip("调试端口")]
#if ODIN_INSPECTOR
        [LabelText("热更调试端口号")]
#endif
        private int m_DebugPort = 56000;

        public bool IsMonoBehaviorMode
        {
            get
            {
                return m_IsMonoBehaviorMode;
            }
        }

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