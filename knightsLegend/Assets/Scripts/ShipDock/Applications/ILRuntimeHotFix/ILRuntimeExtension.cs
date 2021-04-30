using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ShipDock.Applications;
using System.Collections.Generic;
using UnityEngine;
using ParamBuilderAction = System.Action<ILRuntime.Runtime.Enviorment.InvocationContext, object[]>;

public static class ILRuntimeUtils
{
    private static IAppILRuntime frameworkApp;
    private static AppDomain appDomain;
    private static ILMethodCacher methodCacher;

    public static void InitFromApp(this ILRuntimeHotFix target, IAppILRuntime app)
    {
        frameworkApp = app;
    }

    public static IAppILRuntime GetAppILRuntime(this ILRuntimeHotFix target)
    {
        return frameworkApp;
    }

    public static ILRuntimeHotFix GetILRuntimeHotFix()
    {
        return frameworkApp.ILRuntimeHotFix;
    }

    private static AppDomain ILAppDomain()
    {
        if (appDomain == default)
        {
            appDomain = GetILRuntimeHotFix().ILAppDomain;
        }
        else { }

        return appDomain;
    }

    private static ILMethodCacher MethodCacher()
    {
        if (methodCacher == default)
        {
            methodCacher = GetILRuntimeHotFix().MethodCacher;
        }
        else { }

        return methodCacher;
    }

    public static void ClearExtension(this ILRuntimeHotFix target)
    {
        appDomain = default;
        methodCacher = default;
        frameworkApp = default;
    }

    /// <summary>
    /// 调用无参数静态方法
    /// </summary>
    /// <param name="typeName">类名（含命名空间）</param>
    /// <param name="method">方法名</param>
    public static void StaticInvokeILR(this string typeName, string method)
    {
        ILAppDomain().Invoke(typeName, method, default, default);
    }

    /// <summary>
    /// 调用带参数的静态方法
    /// </summary>
    /// <param name="typeName">类名（含命名空间）</param>
    /// <param name="method">方法名</param>
    /// <param name="args">参数列表</param>
    public static void StaticInvokeILR(this string typeName, string method, params object[] args)
    {
        ILAppDomain().Invoke(typeName, method, default, args);
    }

    /// <summary>
    /// 根据方法名称和参数个数获取方法
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="typeName">类名（含命名空间）</param>
    /// <param name="methodName">方法名</param>
    /// <param name="applyNoGCMode">是否应用无GC模式调用函数</param>
    /// <param name="args">参数列表</param>
    public static void StaticInvokeILR(this string typeName, string methodName, int paramCount, ParamBuilderAction paramBuilderCallback, params object[] args)
    {
        AppDomain appDomain = ILAppDomain();
        ILRuntimeInvokeCacher cacher = MethodCacher().GetMethodCacher(typeName);
        IMethod method = cacher.GetMethodFromCache(appDomain, typeName, methodName, paramCount);//预先获得IMethod，可以减低每次调用查找方法耗用的时间
        if (paramBuilderCallback != default)
        {
            using (InvocationContext ctx = appDomain.BeginInvoke(method))//无GC的调用模式
            {
                paramBuilderCallback?.Invoke(ctx, args);//构建需要传入的参数
                ctx.Invoke();
            }
        }
        else
        {
            appDomain.Invoke(method, default, args);//普通模式
        }
    }

    /// <summary>
    /// 指定参数类型来获得IMethod
    /// </summary>
    /// <param name="types">参数类型列表</param>
    /// <param name="typeName">类名（含命名空间）</param>
    /// <param name="methodName">方法名</param>
    /// <param name="applyNoGCMode">是否应用无GC模式调用函数</param>
    /// <param name="args">参数列表</param>
    public static void StaticInvokeILR(this string typeName, string methodName, System.Type[] types, ParamBuilderAction paramBuilderCallback, params object[] args)
    {
        AppDomain appDomain = ILAppDomain();

        IType typeTemp;
        List<IType> paramList = new List<IType>();//参数类型列表
        int max = types.Length;
        for (int i = 0; i < max; i++)
        {
            typeTemp = appDomain.GetType(types[i]);
            paramList.Add(typeTemp);
        }

        IType type = MethodCacher().GetClassCache(typeName, appDomain);
        IMethod method = type.GetMethod(methodName, paramList, default);//根据方法名称和参数类型列表获取方法
        if (paramBuilderCallback != default)
        {
            using (InvocationContext ctx = appDomain.BeginInvoke(method))//无GC的调用模式
            {
                paramBuilderCallback?.Invoke(ctx, args);//构建需要传入的参数
                ctx.Invoke();
            }
        }
        else
        {
            appDomain.Invoke(method, default, args);//普通模式
        }
    }

    /// <summary>
    /// 调用静态泛型方法
    /// </summary>
    /// <param name="typeName"></param>
    /// <param name="methodName"></param>
    /// <param name="generics"></param>
    /// <param name="args"></param>
    public static void StaticGenericILR(this string typeName, string methodName, System.Type[] generics, params object[] args)
    {
        AppDomain appDomain = ILAppDomain();
        int max = generics.Length;
        IType type;
        IType[] genericArgs = new IType[max];
        for (int i = 0; i < max; i++)
        {
            type = appDomain.GetType(generics[i]);
            genericArgs[i] = type;
        }
        appDomain.InvokeGenericMethod(typeName, methodName, genericArgs, default, args);
    }


    /// <summary>
    /// 实例化热更里的类
    /// </summary>
    /// <typeparam name="T">泛型参数</typeparam>
    /// <param name="typeName">类名</param>
    /// <param name="args">实例化时传入的参数</param>
    /// <returns></returns>
    public static object InstantiateFromIL(string typeName, params object[] args)
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
    public static object InstantiateFromIL(string typeName)
    {
        IType type = MethodCacher().GetClassCache(typeName, ILAppDomain());
        object result = ((ILType)type).Instantiate();
        return result;
    }

    public static object InstantiateMonoFromIL(GameObject target, string monoCompName)
    {
        ILType type = appDomain.LoadedTypes[monoCompName] as ILType;//获取Mono组件在ILRuntime中的类型定义
        
        if (type == default)
        {
            Debug.Log("Error: ILType is null when call InstantiateMonoFromIL");
            return default;
        }
        else { }

        ILTypeInstance result = new ILTypeInstance(type as ILType, false);//以适配器的方式新建组件
        MonoBehaviourAdapter.Adaptor component = target.AddComponent<MonoBehaviourAdapter.Adaptor>();//添加组件

        component.ILInstance = result;//手动构建ILRuntime热更端实例的关联
        component.AppDomain = ILAppDomain();
        result.CLRInstance = component;//手动替换组件的实例（AddComponent不是引擎中原来的API）
        component.Awake();//补调 Awake

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
    public static void InvokeMethodILR(object instance, string typeName, string methodName, int paramCount, System.Action<InvocationContext> resultCallback, params object[] args)
    {
        ILRuntimeInvokeCacher methodCacher = MethodCacher().GetMethodCacher(typeName);
        IMethod method = methodCacher.GetMethodFromCache(ILAppDomain(), typeName, methodName, paramCount);//检测是否存在方法缓存，没有则将使用反射获取
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

    public static void InvokeMethodILR(object instance, string typeName, string methodName, int paramCount, params object[] args)
    {
        ILRuntimeInvokeCacher methodCacher = MethodCacher().GetMethodCacher(typeName);
        IMethod method = methodCacher.GetMethodFromCache(ILAppDomain(), typeName, methodName, paramCount);//检测是否存在方法缓存，没有则将使用反射获取
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
