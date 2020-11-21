#define _G_LOG

using ShipDock.Pooling;
using ShipDock.Testers;
using System;
using System.Reflection;

namespace ShipDock.Server
{
    /// <summary>
    /// 解析器函数处理器委托类型
    /// </summary>
    public delegate void ResolveDelegate<T>(ref T target);

    /// <summary>
    /// 
    /// 服务容器基类
    /// 
    /// 用于基于接口开发功能交互的IoC模式的实现
    /// 
    /// </summary>
    public class Server : IServer
    {
        private static readonly Type callableAttrType = typeof(CallableAttribute);

        public Server(string serverName = "")
        {
            if (string.IsNullOrEmpty(ServerName))
            {
                ServerName = serverName;
            }
        }

        /// <summary>
        /// 销毁
        /// </summary>
        public virtual void Dispose()
        {
            Purge();
        }

        /// <summary>
        /// 覆盖此方法以定义服务容器子类的销毁过程
        /// </summary>
        protected virtual void Purge()
        {

        }

        /// <summary>
        /// 覆盖此方法以定义服务容器的初始化过程，用于注册容器中的对象解析器
        /// </summary>
        public virtual void InitServer()
        {
        }

        /// <summary>
        /// 覆盖此方法以定义服务容器的初始化结束后的功能，用于增加容器中暴露于外界的外派方法
        /// </summary>
        public virtual void ServerReady()
        {
        }

        /// <summary>
        /// 以指定的接口为参数调用容器暴露于外界的外派方法
        /// </summary>
        public InterfaceT Delive<InterfaceT>(string resolverName, string alias, ResolveDelegate<InterfaceT> customResolver = default, bool isMakeResolver = false)
        {
            if (isMakeResolver)
            {
                MakeResolver(alias, resolverName, customResolver);
                return Resolve<InterfaceT>(alias, resolverName);
            }
            else
            {
                return Resolve(alias, resolverName, customResolver);
            }
        }

        /// <summary>
        /// 新增容器中的外派方法，外派方法可被容器外部通过 Delive 方法调用
        /// </summary>
        public void Add<InterfaceT>(ResolveDelegate<InterfaceT> target, bool onlyOnce = false)
        {
            int statu = 0;

            MethodInfo method = target.Method;
            object[] attributes = method.GetCustomAttributes(callableAttrType, false);

            int max = attributes.Length;
            string resolverName, alias;
            CallableAttribute attribute;
            IResolvable resolvable;
            for (int i = 0; i < max; i++)
            {
                attribute = attributes[i] as CallableAttribute;
                resolverName = attribute.ResolverName;
                resolverName = string.IsNullOrEmpty(resolverName) ? method.Name : resolverName;
                alias = attribute.Alias;
                resolvable = ServersHolder.GetResolvable(ref alias, out statu);
                if (statu == 0)
                {
                    "error".Log(resolvable == default, "Resolvable is null, alias is " + alias);
                    resolvable.SetResolver(resolverName, target, out statu, onlyOnce);
                }
            }
        }

        /// <summary>
        /// 为解析器新定义一个解析器函数，与之前定义的解析器函数共同生效
        /// </summary>
        public int MakeResolver<InterfaceT>(string alias, string resolverName, ResolveDelegate<InterfaceT> target)
        {
            int statu = 0;
            IResolvable resolvable = ServersHolder.GetResolvable(ref alias, out statu);
            if (statu == 0)
            {
                "error".Log(resolvable == default, "Resolvable is null when MakeResolver, alias is " + alias);
                resolvable.SetResolver(resolverName, target, out statu, false, true);
            }
            return statu;
        }

        /// <summary>
        /// 撤销已定义在解析器中的一个解析器函数
        /// </summary>
        public int RevokeResolver<InterfaceT>(string alias, string resolverName, ResolveDelegate<InterfaceT> target)
        {
            int statu = 0;
            IResolvable resolvable = ServersHolder.GetResolvable(ref alias, out statu);
            if (statu == 0)
            {
                "error".Log(resolvable == default, "Resolvable is null when MakeResolver, alias is " + alias);
                resolvable.RevokeResolver(resolverName, target);
            }
            return statu;
        }

        /// <summary>
        /// 重新注册解析器的解析器函数，并返回旧的解析器函数
        /// </summary>
        public ResolveDelegate<InterfaceT> Reregister<InterfaceT>(ResolveDelegate<InterfaceT> target, string alias)
        {
            ResolveDelegate<InterfaceT> raw = default;
            IResolvable resolvable = ServersHolder.GetResolvable(ref alias, out int resultError);
            if (resultError == 0)
            {
                IResolverCacher<InterfaceT> resolverHandler = resolvable.GetResolver<InterfaceT>(Resolvable.RESOLVER_INIT, out _) as IResolverCacher<InterfaceT>;
                raw = resolverHandler.DelegateTarget;
                resolverHandler.SetDelegate(target);
            }
            return raw;
        }

        /// <summary>
        /// 注册一个新的解析器
        /// </summary>
        /// <typeparam name="InterfaceT">要解析对象的接口</typeparam>
        /// <param name="target">解析器函数</param>
        /// <param name="factory">解析对象的对象池或工厂对象</param>
        /// <returns></returns>
        public int Register<InterfaceT>(ResolveDelegate<InterfaceT> target, params IPoolBase[] factory)
        {
            IResolvable[] list = ServersHolder.SetResolvable(target, out int statu);
            int max = list != default ? list.Length : 0;
            if (max > 0)
            {
                IResolvable resolvable;
                IPoolBase factoryItem;
                int factoryCount = factory != default ? factory.Length : 0;
                for (int i = 0; i < max; i++)
                {
                    factoryItem = factoryCount > i ? factory[i] : default;
                    resolvable = list[i];
                    resolvable.InitResolver<InterfaceT>(ServersHolder, factoryItem);
                }
            }
            return statu;
        }

        /// <summary>
        /// 注销一个解析器
        /// </summary>
        public void Unregister<InterfaceT>(string alias)
        {
            //TODO 注销容器方法
        }

        /// <summary>
        /// 通过接口及别名解析出一个对象的实例并根据解析器名调用解析器函数
        /// </summary>
        /// <typeparam name="InterfaceT">接口</typeparam>
        /// <param name="alias">别名</param>
        /// <param name="resolverName">解析器名，用于将解析得到的对象传入对应的加工函数，不传此参表示仅获取实例</param>
        /// <param name="customResolver">定制的解析器函数，在传入加工函数前预处理解析得到的对象</param>
        /// <returns>需要获得的实例对象</returns>
        public InterfaceT Resolve<InterfaceT>(string alias, string resolverName = "", ResolveDelegate<InterfaceT> customResolver = default)
        {
            int resultError;
            InterfaceT result = default;
            IResolvable resolvable = ServersHolder.GetResolvable(ref alias, out resultError);
            if (resolvable == default)
            {
                UnityEngine.Debug.Log("Resolvable is null, alias is " + alias);
            }
            if (resultError == 0)
            {
                IResolverHandler resolverHandler;

                resolverHandler = resolvable.GetResolver<InterfaceT>(Resolvable.RESOLVER_CRT, out _);
                resolverHandler.InvokeResolver();
                result = (InterfaceT)resolverHandler.ResolverParam;

                resolverHandler = resolvable.GetResolver<InterfaceT>(Resolvable.RESOLVER_INIT, out _);
                resolverHandler.SetParam(ref result);
                resolverHandler.InvokeResolver();
                result = (InterfaceT)resolverHandler.ResolverParam;

                if (result != default)
                {
                    if (customResolver != default)
                    {
                        customResolver.Invoke(ref result);
                    }
                    bool isDelive = !string.IsNullOrEmpty(resolverName);
                    if (isDelive)
                    {

                        resolverHandler = resolvable.GetResolver<InterfaceT>(resolverName, out _);
                        if (resolverHandler == default)
                        {
                            return result;
                        }
                        resolverHandler.SetParam(ref result);
                        resolverHandler.InvokeResolver();

                        if (resolverHandler.OnlyOnce)
                        {
                            resolvable.RemoveResolver<InterfaceT>(resolverName, out _);
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 还原一个已解析的对象，重置回对应的对象池或工厂
        /// </summary>
        /// <param name="target">已解析的对象</param>
        /// <param name="alias">已解析对象的别名</param>
        public void Revert(IPoolable target, string alias)
        {
            IResolvable resolvable = ServersHolder.GetResolvable(ref alias, out int resultError);
            if (resultError == 0)
            {
                if (resolvable.InstanceFactory == default)
                {
                    target.Revert();
                }
                else
                {
                    resolvable.InstanceFactory.Reserve(ref target);
                }
            }
        }

        /// <summary>
        /// 设置服务容器管理器
        /// </summary>
        public void SetServerHolder(IServersHolder servers)
        {
            ServersHolder = servers;
        }

        /// <summary>服务容器初始化的优先级</summary>
        public int Prioriity { get; set; }
        /// <summary>服务容器管理器引用</summary>
        public IServersHolder ServersHolder { get; private set; }
        /// <summary>服务容器名</summary>
        public virtual string ServerName { get; protected set; }
    }

}
