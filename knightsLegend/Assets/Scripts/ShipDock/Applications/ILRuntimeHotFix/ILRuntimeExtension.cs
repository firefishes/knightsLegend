using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using ShipDock.Applications;
using System.Collections.Generic;
using ParamBuilderAction = System.Action<ILRuntime.Runtime.Enviorment.InvocationContext, object[]>;

public static class ILRuntimeExtension
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

    private static AppDomain Enviorment()
    {
        if (appDomain == default)
        {
            appDomain = GetILRuntimeHotFix().ILAppDomain;
        }
        return appDomain;
    }

    private static ILMethodCacher MethodCacher()
    {
        if (methodCacher == default)
        {
            methodCacher = GetILRuntimeHotFix().MethodCacher;
        }
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
        Enviorment().Invoke(typeName, method, default, default);
    }

    /// <summary>
    /// 调用带参数的静态方法
    /// </summary>
    /// <param name="typeName">类名（含命名空间）</param>
    /// <param name="method">方法名</param>
    /// <param name="args">参数列表</param>
    public static void StaticInvokeILR(this string typeName, string method, params object[] args)
    {
        Enviorment().Invoke(typeName, method, default, args);
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
        AppDomain appDomain = Enviorment();
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
        AppDomain appDomain = Enviorment();

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
        AppDomain appDomain = Enviorment();
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
}
