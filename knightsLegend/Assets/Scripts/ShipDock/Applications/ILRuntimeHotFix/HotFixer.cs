#define LOG_INOVKED_METHOD_BY_ARGS_COUNT

using System;
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
        protected HotFixerStartUpInfo m_StartUpInfo = new HotFixerStartUpInfo();

        protected object mShellBridge;

        private ILRuntimeIniter mILRuntimeIniter;

        protected ILMethodCacher MethodCacher { get; set; }

        /// <summary>
        /// ILRuntime热更应用域引用
        /// </summary>
        /// <returns></returns>
        public virtual ILRuntime.Runtime.Enviorment.AppDomain Enviorment()
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
            Enviorment().UnityMainThreadID = System.Threading.Thread.CurrentThread.ManagedThreadId;
            Enviorment().DebugService.StartDebugService(56000);
#endif
        }

        #region Unity周期函数回调
        private Action mILRuntimeUpdate;
        private Action mILRuntimeFixedUpdate;
        private Action mILRuntimeLateUpdate;
        private Action mILRuntimeDestroy;
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
            mILRuntimeDestroy = context.ReadObject<Action>();
        }

        private void OnGetLateUpdateMethod(InvocationContext context)
        {
            mILRuntimeLateUpdate = context.ReadObject<Action>();
        }

        private void OnGetUpdateMethod(InvocationContext context)
        {
            mILRuntimeUpdate = context.ReadObject<Action>();
        }

        private void OnGetFixedUpdateMethod(InvocationContext context)
        {
            mILRuntimeFixedUpdate = context.ReadObject<Action>();
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
#if LOG_INOVKED_METHOD_BY_ARGS_COUNT
                Debug.Log(string.Format("HOTFIX invoke: {0}.{1}", typeName, methodName));
#endif
                ctx.Invoke();
            }
        }
    }
}