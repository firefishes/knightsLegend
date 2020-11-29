using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using UnityEngine;
using ResultAction = System.Action<ILRuntime.Runtime.Enviorment.InvocationContext>;

namespace ShipDock.Applications
{
    public abstract class HotFixer : MonoBehaviour
    {
        [SerializeField]
        protected bool m_ApplyRunStandalone;
        [SerializeField]
        protected string m_ClassName;
        [SerializeField]
        protected string m_IniterMethodName = "ShellInited";

        private object mShellBridge;
        private ILRuntimeIniter mILRuntimeIniter;

        protected ILMethodCacher MethodCacher { get; set; }

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

        protected virtual void Init()
        {
            mILRuntimeIniter = new ILRuntimeIniter(Enviorment());
            MethodCacher = ShipDockApp.Instance.ILRuntimeHotFix.MethodCacher;
        }

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

        protected virtual void ILRuntimeLoaded()
        {
            mShellBridge = InstantiateFromIL(m_ClassName);
            InvokeMethodILR(mShellBridge, m_ClassName, m_IniterMethodName, 1, gameObject);
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
        public void InvokeMethodILR(object instance, string typeName, string methodName, int paramCount, ResultAction resultCallback)
        {
            ILRuntimeInvokeCacher methodCacher = MethodCacher.GetMethodCacher(typeName);
            IMethod method = methodCacher.GetMethodFromCache(Enviorment(), typeName, methodName, paramCount);//type.GetMethod(methodName, paramCount);
            using (InvocationContext ctx = Enviorment().BeginInvoke(method))
            {
                ctx.PushObject(instance);
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