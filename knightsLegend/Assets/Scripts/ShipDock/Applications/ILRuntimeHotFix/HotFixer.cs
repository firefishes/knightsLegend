#define _LOG_STATU
#define _LOG_HOT_FIX_COMP_START

#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
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

#if ODIN_INSPECTOR
        [TitleGroup("热更组件")]
#endif
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
            else { }
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
#if LOG_STATU
            int statu = 0;
#endif
            bool hasDll = dll != default;

            if (ILRuntimeIniter.HasLoadAnyAssembly)
            {
                if (hasDll)
                {
                    mILRuntimeIniter.Build(dll, pdb);
                }
                else { }
            }
            else
            {
                if (hasDll)
                {
                    mILRuntimeIniter.Build(dll, pdb);
                }
#if LOG_STATU
                else
                {
                    statu = 1;//没有任何热更端的文件被加载
                }
#else
                else { }
#endif
            }
#if LOG_STATU
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

#if LOG_STATU && LOG_HOT_FIX_COMP_START
            "HotFixer InstantiateFromIL, class name is {0}".Log(m_StartUpInfo.ClassName);
            "HotFixer {0} loaded.".Log(statu != 0 && ShellBridge != default ? ShellBridge.ToString() : "do not need, may be is UI");
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
            ILRuntimeHotFix.StartDebugServices(m_StartUpInfo.DebugPort);
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
#if LOG_STATU
                    Debug.Log(GetType().Name + "'s ClassName is null.");
#endif
                    return;
                }
                else { }
            }
            else
            {
                return;
            }

            string clsName = m_StartUpInfo.ClassName;
            if (m_StartUpInfo.IsMonoBehaviorMode)
            {
                ILRuntimeUtils.InstantiateMonoFromIL(gameObject, clsName);
            }
            else
            {
                HotFixBaseMode(ref clsName);
            }
        }

        /// <summary>
        /// 官方不推荐直接在热更端使用组件，故提供一种绕过 MonoBehaviour 组件纯热更的开发方式
        /// </summary>
        /// <param name="clsName"></param>
        private void HotFixBaseMode(ref string clsName)
        {
            ShellBridge = ILRuntimeUtils.InstantiateFromIL(clsName);

            string method = "GetUpdateMethods";
            string className = m_StartUpInfo.ClassName;

            if (m_StartUpInfo.ApplyFixedUpdate)
            {
                ILRuntimeUtils.InvokeMethodILR(ShellBridge, className, method, 1, OnGetFixedUpdateMethod, m_StartUpInfo.FixedUpdateMethodName);//模拟 FixedUpdate
            }
            else { }

            if (m_StartUpInfo.ApplyUpdate)
            {
                ILRuntimeUtils.InvokeMethodILR(ShellBridge, className, method, 1, OnGetUpdateMethod, m_StartUpInfo.UpdateMethodName);//模拟 Update
            }
            else { }

            if (m_StartUpInfo.ApplyLateUpdate)
            {
                ILRuntimeUtils.InvokeMethodILR(ShellBridge, className, method, 1, OnGetLateUpdateMethod, m_StartUpInfo.LateUpdateMethodName);//模拟 LateUpdate
            }
            else { }

            ILRuntimeUtils.InvokeMethodILR(ShellBridge, className, method, 1, OnGetDestroyMethod, "OnDestroy");//模拟 OnDestroy
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