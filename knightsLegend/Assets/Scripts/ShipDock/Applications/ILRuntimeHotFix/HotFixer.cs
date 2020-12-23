#define _LOG_INOVKED_METHOD_BY_ARGS_COUNT
#define _LOG_INITER

using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
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

        [SerializeField]
        protected HotFixerStartUpInfo m_StartUpInfo = new HotFixerStartUpInfo();

        protected object mShellBridge;

        private ILRuntimeIniter mILRuntimeIniter;

        public HotFixerStartUpInfo StartUpInfo
        {
            get
            {
                return m_StartUpInfo;
            }
        }

        protected ILMethodCacher MethodCacher { get; set; }

        /// <summary>
        /// ILRuntime热更应用域引用
        /// </summary>
        /// <returns></returns>
        protected virtual AppDomain ILAppDomain()
        {
            return ILRuntimeExtension.GetILRuntimeHotFix().ILAppDomain;
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
            MethodCacher = default;

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

            MethodCacher = ILRuntimeExtension.GetILRuntimeHotFix().MethodCacher;
        }

        /// <summary>
        /// 子类重写此方法，扩展加载热更代码后的逻辑
        /// </summary>
        /// <param name="dll"></param>
        /// <param name="pdb"></param>
        protected virtual void StartHotfix(byte[] dll, byte[] pdb = default)
        {
            int statu = 0;
            bool hasDll = dll != default;

            if (ILRuntimeIniter.HasLoadAnyAssembly)
            {
                if (hasDll)
                {
                    mILRuntimeIniter.Build(dll, pdb);
                }
                else
                {
                    if (!ILRuntimeIniter.ApplySingleHotFixMode)
                    {
                        statu = 2;//多热更端的模式下，必须指定热更文件
                    }
                }
            }
            else
            {
                if (hasDll)
                {
                    mILRuntimeIniter.Build(dll, pdb);
                }
                else
                {
                    statu = 1;//没有任何热更端的文件被加载
                }
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
                    Debug.Log(m_StartUpInfo.ClassName + "- dll loaded");
                    break;
            }
#endif

            InitILRuntime();
            ILRuntimeLoaded();
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
            if (string.IsNullOrEmpty(m_StartUpInfo.ClassName))
            {
#if LOG_INITER
                Debug.Log(GetType().Name + "'s ClassName is null.");
#endif
                return;
            }

            mShellBridge = InstantiateFromIL(m_StartUpInfo.ClassName);

            string method = "GetUpdateMethods";
            string className = m_StartUpInfo.ClassName;
            if (m_StartUpInfo.ApplyFixedUpdate)
            {
                InvokeMethodILR(mShellBridge, className, method, 1, OnGetFixedUpdateMethod, m_StartUpInfo.FixedUpdateMethodName);
            }
            if (m_StartUpInfo.ApplyUpdate)
            {
                InvokeMethodILR(mShellBridge, className, method, 1, OnGetUpdateMethod, m_StartUpInfo.UpdateMethodName);
            }
            if (m_StartUpInfo.ApplyLateUpdate)
            {
                InvokeMethodILR(mShellBridge, className, method, 1, OnGetLateUpdateMethod, m_StartUpInfo.LateUpdateMethodName);
            }
            InvokeMethodILR(mShellBridge, className, method, 1, OnGetDestroyMethod, "OnDestroy");
            InvokeMethodILR(mShellBridge, className, m_StartUpInfo.IniterMethodName, 1, this);
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

        /// <summary>
        /// 实例化热更里的类
        /// </summary>
        /// <typeparam name="T">泛型参数</typeparam>
        /// <param name="typeName">类名</param>
        /// <param name="args">实例化时传入的参数</param>
        /// <returns></returns>
        public object InstantiateFromIL(string typeName, params object[] args)
        {
            object result = ILAppDomain().Instantiate(typeName, args);
            return result;
        }

        /// <summary>
        /// 实例化热更里的类
        /// </summary>
        /// <typeparam name="T">泛型参数</typeparam>
        /// <param name="typeName">类名</param>
        /// <returns></returns>
        public object InstantiateFromIL(string typeName)
        {
            IType type = MethodCacher.GetClassCache(typeName, ILAppDomain());
            object result = ((ILType)type).Instantiate();
            return result;
        }

        /// <summary>
        /// 通过实例调用成员方法
        /// </summary>
        /// <param name="instance">实例</param>
        /// <param name="typeName">类名</param>
        /// <param name="methodName">方法名</param>
        /// <param name="paramCount">方法的参数个数</param>
        /// <param name="resultCallback">获取方法值的回调</param>
        public void InvokeMethodILR(object instance, string typeName, string methodName, int paramCount, System.Action<InvocationContext> resultCallback, params object[] args)
        {
            ILRuntimeInvokeCacher methodCacher = MethodCacher.GetMethodCacher(typeName);
            IMethod method = methodCacher.GetMethodFromCache(ILAppDomain(), typeName, methodName, paramCount);
            using (InvocationContext ctx = ILAppDomain().BeginInvoke(method))
            {
                ctx.PushObject(instance);
                int max = args.Length;
                for (int i = 0; i < max; i++)
                {
                    ctx.PushObject(args[i]);
                }
                ctx.Invoke();
                resultCallback?.Invoke(ctx);
            }
        }

        public void InvokeMethodILR(object instance, string typeName, string methodName, int paramCount, params object[] args)
        {
            ILRuntimeInvokeCacher methodCacher = MethodCacher.GetMethodCacher(typeName);
            IMethod method = methodCacher.GetMethodFromCache(ILAppDomain(), typeName, methodName, paramCount);
            using (InvocationContext ctx = ILAppDomain().BeginInvoke(method))
            {
                ctx.PushObject(instance);
                int max = args.Length;
                for (int i = 0; i < max; i++)
                {
                    ctx.PushObject(args[i]);
                }
#if LOG_INOVKED_METHOD_BY_ARGS_COUNT
                Debug.Log(string.Format("HOTFIX invoke: {0}.{1}", typeName, methodName));
#endif
                ctx.Invoke();
            }
        }
    }
}