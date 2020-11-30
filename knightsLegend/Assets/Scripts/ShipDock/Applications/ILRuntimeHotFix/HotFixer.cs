using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using UnityEngine;
using ResultAction = System.Action<ILRuntime.Runtime.Enviorment.InvocationContext>;

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
        [SerializeField]
        [Tooltip("是否脱离框架单独运行")]
        protected bool m_ApplyRunStandalone;
        [SerializeField]
        [Tooltip("热更端的类名")]
        protected string m_ClassName;
        [SerializeField]
        [Tooltip("主项目启动热更端的入口方法")]
        protected string m_IniterMethodName = "ShellInited";

        private object mShellBridge;
        private ILRuntimeIniter mILRuntimeIniter;

        protected ILMethodCacher MethodCacher { get; set; }

        /// <summary>
        /// ILRuntime热更应用域引用
        /// </summary>
        /// <returns></returns>
        public abstract AppDomain Enviorment();

        protected virtual void Awake()
        {
            if (m_ApplyRunStandalone)
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

            mILRuntimeIniter?.Clear();
            mILRuntimeIniter = default;
            MethodCacher = default;
        }

        protected abstract void RunWithinFramework();
        protected abstract void Purge();

        /// <summary>
        /// 子类重写此方法，扩展初始化的逻辑
        /// </summary>
        protected virtual void Init()
        {
            mILRuntimeIniter = new ILRuntimeIniter(Enviorment());

            MethodCacher = ILRuntimeExtension.GetILRuntimeHotFix().MethodCacher;
        }

        /// <summary>
        /// 子类重写此方法，扩展加载热更代码后的逻辑
        /// </summary>
        /// <param name="dll"></param>
        /// <param name="pdb"></param>
        protected virtual void StartHotfix(byte[] dll, byte[] pdb)
        {
            mILRuntimeIniter.Build(dll, pdb);

            InitILRuntime();
            ILRuntimeLoaded();
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
            Enviorment().UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
#endif
        }

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

        #region Unity周期函数回调
        private System.Action mILRuntimeUpdate;
        private System.Action mILRuntimeFixedUpdate;
        private System.Action mILRuntimeLateUpdate;
        #endregion

        private void FixedUpdate()
        {
            mILRuntimeUpdate?.Invoke();
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
            mShellBridge = InstantiateFromIL(m_ClassName);
            InvokeMethodILR(mShellBridge, m_ClassName, m_IniterMethodName, 1, this);

            string method = "GetUpdateMethods";
            if (m_ApplyFixedUpdate)
            {
                InvokeMethodILR(mShellBridge, m_ClassName, method, 1, OnGetFixedUpdateMethod, m_FixedUpdateMethodName);
            }
            if (m_ApplyUpdate)
            {
                InvokeMethodILR(mShellBridge, m_ClassName, method, 1, OnGetFixedUpdateMethod, m_UpdateMethodName);
            }
            if (m_ApplyLateUpdate)
            {
                InvokeMethodILR(mShellBridge, m_ClassName, method, 1, OnGetFixedUpdateMethod, m_LateUpdateMethodName);
            }
        }

        private void OnGetFixedUpdateMethod(InvocationContext context)
        {
            mILRuntimeUpdate = context.ReadObject<System.Action>();
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
            object result = Enviorment().Instantiate(typeName, args);
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
            IType type = MethodCacher.GetClassCache(typeName, Enviorment());
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
        public void InvokeMethodILR(object instance, string typeName, string methodName, int paramCount, ResultAction resultCallback, params object[] args)
        {
            ILRuntimeInvokeCacher methodCacher = MethodCacher.GetMethodCacher(typeName);
            IMethod method = methodCacher.GetMethodFromCache(Enviorment(), typeName, methodName, paramCount);
            using (InvocationContext ctx = Enviorment().BeginInvoke(method))
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
            IMethod method = methodCacher.GetMethodFromCache(Enviorment(), typeName, methodName, paramCount);
            using (InvocationContext ctx = Enviorment().BeginInvoke(method))
            {
                ctx.PushObject(instance);
                int max = args.Length;
                for (int i = 0; i < max; i++)
                {
                    ctx.PushObject(args[i]);
                }
                ctx.Invoke();
            }
        }
    }
}