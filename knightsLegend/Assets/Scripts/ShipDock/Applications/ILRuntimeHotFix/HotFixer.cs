#define _LOG_INOVKED_METHOD_BY_ARGS_COUNT
#define _LOG_INITER
#define _LOG_HOT_FIX_COMP_START

using ILRuntime.Runtime.Enviorment;
using UnityEngine;

namespace ShipDock.Applications
{
    /// <summary>
    /// 热更组件抽象类
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public abstract class HotFixer : MonoBehaviour
    {
        private static bool isDebugServiceStarted;

        public static void StartHotFixeByAsset(HotFixer target, TextAsset dll, TextAsset pdb = default)
        {
            byte[] dllVS = dll != default ? dll.bytes : default;
#if RELEASE
            target.StartHotfix(dllVS);//正式发布时不加载pdb文件
#else
            byte[] pdbVS = pdb != default ? pdb.bytes : default;
            target.StartHotfix(dllVS, pdbVS);
#endif
        }

        [SerializeField]
        protected HotFixerStartUpInfo m_StartUpInfo = new HotFixerStartUpInfo();

        private ILRuntimeIniter mILRuntimeIniter;

        public object ShellBridge { get; private set; }

        public HotFixerStartUpInfo StartUpInfo
        {
            get
            {
                return m_StartUpInfo;
            }
        }

        /// <summary>
        /// ILRuntime热更应用域引用
        /// </summary>
        /// <returns></returns>
        protected virtual AppDomain ILAppDomain()
        {
            return ILRuntimeUtils.GetILRuntimeHotFix().ILAppDomain;
        }

        protected virtual void Awake()
        {
            if (m_StartUpInfo.RunInAwake)
            {
                Run();
            }
        }

        private void Run()
        {
            if (m_StartUpInfo.ApplyRunStandalone)
            {
                Init();
            }
            else
            {
                RunWithinFramework();
            }
        }

        protected virtual void OnDestroy()
        {
            Purge();

            mILRuntimeDestroy?.Invoke();

            mILRuntimeIniter?.Clear();
            mILRuntimeIniter = default;

            mILRuntimeDestroy = default;
            mILRuntimeFixedUpdate = default;
            mILRuntimeUpdate = default;
            mILRuntimeLateUpdate = default;
        }

        protected abstract void RunWithinFramework();
        protected abstract void Purge();

        /// <summary>
        /// 子类重写此方法，扩展初始化的逻辑
        /// </summary>
        protected virtual void Init()
        {
            mILRuntimeIniter = new ILRuntimeIniter(ILAppDomain());
        }

        /// <summary>
        /// 子类重写此方法，扩展加载热更代码后的逻辑
        /// </summary>
        /// <param name="dll"></param>
        /// <param name="pdb"></param>
        protected virtual void StartHotfix(byte[] dll, byte[] pdb = default)
        {
#if LOG_INITER
            int statu = 0;
#endif
            bool hasDll = dll != default;

            if (ILRuntimeIniter.HasLoadAnyAssembly)
            {
                if (hasDll)
                {
                    mILRuntimeIniter.Build(dll, pdb);
                }
#if LOG_INITER
                else
                {
                    if (!ILRuntimeIniter.ApplySingleHotFixMode)
                    {
                        statu = 2;//多热更端的模式下，必须指定热更文件
                    }
                }
#endif
            }
            else
            {
                if (hasDll)
                {
                    mILRuntimeIniter.Build(dll, pdb);
                }
#if LOG_INITER
                else
                {
                    statu = 1;//没有任何热更端的文件被加载
                }
#endif
            }
#if LOG_INITER
            switch (statu)
            {
                case 1:
                    Debug.LogError(m_StartUpInfo.ClassName + "- it must a dll need add at lest.");
                    break;
                case 2:
                    Debug.LogError(m_StartUpInfo.ClassName + "- dll do not allow an null value, in MultDomain Mode.");
                    break;
                default:
                    Debug.Log((string.IsNullOrEmpty(m_StartUpInfo.ClassName) ? name : m_StartUpInfo.ClassName) + "- dll loaded");
                    break;
            }
#endif
            InitILRuntime();
            ILRuntimeLoaded();

#if LOG_HOT_FIX_COMP_START
            "HotFixer InstantiateFromIL, class name is {0}".Log(m_StartUpInfo.ClassName);
            "HotFixer {0} loaded.".Log(statu != 0 && mShellBridge != default ? mShellBridge.ToString() : "do not need, may be is UI");
#endif
        }

        public virtual void RunHotFix()
        {
            Run();
        }

        /// <summary>
        /// 
        /// 初始化热更运行时
        /// 
        /// 由于Unity的Profiler接口只允许在主线程使用，为了避免出异常，
        /// 需要设置ILRuntime主线程的线程ID才能正确将函数运行耗时报告给Profiler
        /// 
        /// </summary>
        protected virtual void InitILRuntime()
        {
#if DEBUG && (UNITY_EDITOR || UNITY_ANDROID || UNITY_IPHONE)
            if (!isDebugServiceStarted)
            {
                isDebugServiceStarted = true;
                ILAppDomain().UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
                ILAppDomain().DebugService.StartDebugService(m_StartUpInfo.DebugPort);
            }
#endif
        }

#region Unity周期函数回调
        private System.Action mILRuntimeUpdate;
        private System.Action mILRuntimeFixedUpdate;
        private System.Action mILRuntimeLateUpdate;
        private System.Action mILRuntimeDestroy;
#endregion

        private void FixedUpdate()
        {
            mILRuntimeFixedUpdate?.Invoke();
        }

        private void Update()
        {
            mILRuntimeUpdate?.Invoke();
        }

        private void LateUpdate()
        {
            mILRuntimeLateUpdate?.Invoke();
        }

        protected virtual void ILRuntimeLoaded()
        {
            if (m_StartUpInfo.ApplyClassName)
            {
                if (string.IsNullOrEmpty(m_StartUpInfo.ClassName))
                {
#if LOG_INITER
                    Debug.Log(GetType().Name + "'s ClassName is null.");
#endif
                    return;
                }
            }
            else
            {
                return;
            }

            ShellBridge = ILRuntimeUtils.InstantiateFromIL(m_StartUpInfo.ClassName);

            string method = "GetUpdateMethods";
            string className = m_StartUpInfo.ClassName;
            if (m_StartUpInfo.ApplyFixedUpdate)
            {
                ILRuntimeUtils.InvokeMethodILR(ShellBridge, className, method, 1, OnGetFixedUpdateMethod, m_StartUpInfo.FixedUpdateMethodName);
            }
            if (m_StartUpInfo.ApplyUpdate)
            {
                ILRuntimeUtils.InvokeMethodILR(ShellBridge, className, method, 1, OnGetUpdateMethod, m_StartUpInfo.UpdateMethodName);
            }
            if (m_StartUpInfo.ApplyLateUpdate)
            {
                ILRuntimeUtils.InvokeMethodILR(ShellBridge, className, method, 1, OnGetLateUpdateMethod, m_StartUpInfo.LateUpdateMethodName);
            }
            ILRuntimeUtils.InvokeMethodILR(ShellBridge, className, method, 1, OnGetDestroyMethod, "OnDestroy");
            ILRuntimeUtils.InvokeMethodILR(ShellBridge, className, m_StartUpInfo.IniterMethodName, 1, this);
        }

        private void OnGetDestroyMethod(InvocationContext context)
        {
            mILRuntimeDestroy = context.ReadObject<System.Action>();
        }

        private void OnGetLateUpdateMethod(InvocationContext context)
        {
            mILRuntimeLateUpdate = context.ReadObject<System.Action>();
        }

        private void OnGetUpdateMethod(InvocationContext context)
        {
            mILRuntimeUpdate = context.ReadObject<System.Action>();
        }

        private void OnGetFixedUpdateMethod(InvocationContext context)
        {
            mILRuntimeFixedUpdate = context.ReadObject<System.Action>();
        }
    }
}