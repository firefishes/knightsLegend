#define G_LOG

using ShipDock.Pooling;
using ShipDock.Testers;
using System;
using System.Reflection;

namespace ShipDock.Server
{
    public delegate void ResolveDelegate<T>(ref T target);

    public class Server : IServer
    {
        private static readonly Type callableAttrType = typeof(CallableAttribute);

        public Server(string serverName = "")
        {
            if(string.IsNullOrEmpty(ServerName))
            {
                ServerName = serverName;
            }
        }

        public virtual void Dispose()
        {
            Purge();
        }

        protected virtual void Purge()
        {

        }

        public virtual void InitServer()
        {
        }

        public virtual void ServerReady()
        {
        }

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
                    Tester.Instance.Log(TesterBaseApp.Instance, TesterBaseApp.LOG, resolvable == default, "error: Resolvable is null, alias is " + alias);
                    resolvable.SetResolver(resolverName, target, out statu, onlyOnce);
                }
            }
        }

        public int MakeResolver<InterfaceT>(string alias, string resolverName, ResolveDelegate<InterfaceT> target)
        {
            int statu = 0;
            IResolvable resolvable = ServersHolder.GetResolvable(ref alias, out statu);
            if (statu == 0)
            {
                Tester.Instance.Log(TesterBaseApp.Instance, TesterBaseApp.LOG, resolvable == default, "error: Resolvable is null when MakeResolver, alias is " + alias);
                resolvable.SetResolver(resolverName, target, out statu, false, true);
            }
            return statu;
        }

        public int RevokeResolver<InterfaceT>(string alias, string resolverName, ResolveDelegate<InterfaceT> target)
        {
            int statu = 0;
            IResolvable resolvable = ServersHolder.GetResolvable(ref alias, out statu);
            if (statu == 0)
            {
                Tester.Instance.Log(TesterBaseApp.Instance, TesterBaseApp.LOG, resolvable == default, "error: Resolvable is null when MakeResolver, alias is " + alias);
                resolvable.RevokeResolver(resolverName, target);
            }
            return statu;
        }

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

        public int Register<InterfaceT>(ResolveDelegate<InterfaceT> target, params IPoolBase[] factory)
        {
            IResolvable[] list = ServersHolder.SetResolvable(target, out int statu);
            int max = list != default ? list.Length : 0;
            if(max > 0)
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
                    bool isDelive = !string.IsNullOrEmpty(resolverName);
                    if (isDelive)
                    {
                        if (customResolver != default)
                        {
                            customResolver.Invoke(ref result);
                        }

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

        public void SetServerHolder(IServersHolder servers)
        {
            ServersHolder = servers;
        }

        public int Prioriity { get; set; }
        public IServersHolder ServersHolder { get; private set; }
        public virtual string ServerName { get; protected set; }
    }

}
