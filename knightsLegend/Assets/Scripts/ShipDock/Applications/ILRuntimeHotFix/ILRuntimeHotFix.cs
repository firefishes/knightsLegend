
using ILRuntime.Runtime.Enviorment;

namespace ShipDock.Applications
{
#if UNITY_EDITOR
    /// <summary>
    /// 
    /// 运行时热更编辑器应用单例，用于实现编辑器工具下可访问的 AppDomain
    /// 
    /// </summary>
    public class ILRuntimeAppEditor : IAppILRuntime
    {
        private static ILRuntimeAppEditor instance;

        public static ILRuntimeAppEditor GetInstance()
        {
            if (instance == default)
            {
                instance = new ILRuntimeAppEditor();
            }
            else { }

            return instance;
        }

        private IHotFixConfig Config { get; set; }

        public ILRuntimeHotFix ILRuntimeHotFix { get; private set; }

        public void SetHotFixSetting(ILRuntimeHotFix value, IHotFixConfig config)
        {
            ILRuntimeHotFix = value;
            Config = config;
        }

        public IHotFixConfig GetHotFixConfig()
        {
            return Config;
        }
    }
#endif

    /// <summary>
    /// 
    /// 运行时热更管理器，用于实现全局可访问的 AppDomain
    /// 
    /// </summary>
    public class ILRuntimeHotFix
    {
#if UNITY_EDITOR
        public static void InitInEditor()
        {
            string className = "AppHotFixConfig";
            try
            {
                ILRuntimeAppEditor app = ILRuntimeAppEditor.GetInstance();

                System.Type type = System.Type.GetType(className);
                IHotFixConfig config = type.Assembly.CreateInstance(className) as IHotFixConfig;
                config.ToString();

                ILRuntimeHotFix hotFix = new ILRuntimeHotFix(app);
                app.SetHotFixSetting(hotFix, config);
            }
            catch(System.Exception _)
            {
                UnityEngine.Debug.LogError("未定义必需的热更配置 AppHotFixConfig，或热更配置对象为实现 IILuntimeHotFixConfig 接口");
            }
        }
#endif

        public AppDomain ILAppDomain { get; private set; }
        public ILMethodCacher MethodCacher { get; private set; }
        public bool IsStart { get; private set; }

        public ILRuntimeHotFix(IAppILRuntime app)
        {
            this.InitFromApp(app);

            ILAppDomain = new AppDomain();
            MethodCacher = new ILMethodCacher();
        }

        /// <summary>
        /// 启动热更实例，包括跨域函数注册、跨域类装配器注册、重定向方法注册
        /// </summary>
        public void Start()
        {
            if (IsStart)
            {
                return;
            }
            else { }

            IsStart = true;
            IHotFixConfig config = this.GetAppILRuntime().GetHotFixConfig();
            config.RegisterMethods?.Invoke(ILAppDomain);//注册函数适配器

            CrossBindingAdaptor[] list = config.Adapters;
            int max = list == default ? 0 : list.Length;
            for (int i = 0; i < max; i++)
            {
                ILAppDomain.RegisterCrossBindingAdaptor(list[i]);//注册跨域类适配器
            }

            LitJson.JsonMapper.RegisterILRuntimeCLRRedirection(ILAppDomain);//注册热更端JSON解析器
            ILRuntime.Runtime.Generated.CLRBindings.Initialize(ILAppDomain);//注册重定向的方法
        }

        /// <summary>
        /// 重新读取热更脚本时调用此方法（未完成，需要在ILRuntime中增加统一释放的代码）
        /// </summary>
        public void Reset()
        {
            IsStart = false;
            ILRuntimeIniter.HasLoadAnyAssembly = false;

            MethodCacher?.Clear();
        }

        /// <summary>
        /// 自有框架或应用退出时调用此方法
        /// </summary>
        public void Clear()
        {
            UnityEngine.Debug.Log("ILRuntime hot fix 对象以清除，请重新创建");

            Reset();

            this.ClearExtension();

            MethodCacher = default;
            ILAppDomain = default;
        }
    }
}

namespace ILRuntime.Runtime.Generated
{
    partial class CLRBindings
    {
        /// <summary>
        /// 解决热更模式下未生成绑定文件的报错，便于正常模式与热更模式的平顺切换
        /// </summary>
        public static void Initialize(ILRuntime.Runtime.Enviorment.AppDomain app, bool defaultFlag = true) { }
    }
}
