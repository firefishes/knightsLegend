using ShipDock.Pooling;
using ShipDock.Tools;
using System;
using System.Reflection;

namespace ShipDock.Server
{
    public class Resolvable : IResolvable
    {
        /// <summary>用于构造对象的解析器类型标识，如对应的类没有构造函数则不会存在此解析器</summary>
        public const string RESOLVER_CRT = "resolverCrt";
        /// <summary>初始化对象的解析器类型标识</summary>
        public const string RESOLVER_INIT = "resolverInit";

        private static readonly Type[] defaultGenericType = new Type[0];
        private static readonly object[] defaultGenericParam = new object[0];

        private KeyValueList<int, IResolverHandler> mResolvers;
        private ConstructorInfo mDefaultConstructorInfo;
        private Func<Type[], ConstructorInfo> mConstructor;
        private IntegerMapper<string> mResolverIDMapper;

        public void Dispose()
        {
            Binder = default;
            InstanceFactory = default;
            mConstructor = default;
            mDefaultConstructorInfo = default;
            Utils.Reclaim(ref mResolvers);
            Utils.Reclaim(mResolverIDMapper);
        }

        public void InitResolver<InterfaceT>(IServersHolder serverHolder, IPoolBase pool = default)
        {
            InstanceFactory = pool;
            ResolveType = Binder.GetInstanceType(serverHolder);
            mConstructor = ResolveType.GetConstructor;
            mDefaultConstructorInfo = mConstructor(defaultGenericType);

            if(mResolverIDMapper == default)
            {
                mResolverIDMapper = new IntegerMapper<string>();
            }
            SetResolver<InterfaceT>(RESOLVER_CRT, OnResolverDefault, out _);
        }

        private void OnResolverDefault<T>(ref T param)
        {
            if(mDefaultConstructorInfo == default)
            {
                return;
            }
            param = (InstanceFactory == default) ? 
                        (T)mDefaultConstructorInfo.Invoke(defaultGenericParam) : 
                        (T)InstanceFactory.Create();
        }

        public void Binding(ref ResolvableBinder target)
        {
            if (default == Binder)
            {
                Binder = target;
                mResolvers = new KeyValueList<int, IResolverHandler>();
            }
        }

        public void SetResolver<InterfaceT>(string resolverName, ResolveDelegate<InterfaceT> resolveDelgate, out int statu, bool onlyOnce = false, bool isMakeResolver = false)
        {
            statu = 0;
            ResolverHandler<InterfaceT> handler;
            bool hasRef = mResolverIDMapper.ContainsKey(ref resolverName, out int id);
            if(hasRef)
            {
                statu = 1;
                if (isMakeResolver)
                {
                    handler = mResolvers[id] as ResolverHandler<InterfaceT>;
                    handler.AddDelegate(resolveDelgate);
                }
            }
            else
            {
                if (isMakeResolver)
                {
                    statu = 2;
                    return;
                }
                id = mResolverIDMapper.Add(resolverName, out _);
                handler = new ResolverHandler<InterfaceT>();
                handler.SetDelegate(resolveDelgate);
                handler.OnlyOnce = onlyOnce;
                handler.SetID(id);
                mResolvers[id] = handler;
            }
        }
        
        public void RevokeResolver<InterfaceT>(string resolverName, ResolveDelegate<InterfaceT> resolveDelgate)
        {
            ResolverHandler<InterfaceT> handler;
            bool hasRef = mResolverIDMapper.ContainsKey(ref resolverName, out int id);
            if (hasRef)
            {
                handler = mResolvers[id] as ResolverHandler<InterfaceT>;
                handler.RemoveDelegate(resolveDelgate);
            }
        }

        public void RemoveResolver<InterfaceT>(string resolverName, out int statu)
        {
            statu = 0;
            bool hasRef = mResolverIDMapper.ContainsKey(ref resolverName, out int id);
            if (hasRef)
            {
                IResolverHandler handler = mResolvers.Remove(id);
                handler.Dispose();
                mResolverIDMapper.Remove(resolverName, out statu);
            }
            else
            {
                statu = 1;
            }
        }

        public IResolverHandler GetResolver<InterfaceT>(string resolverName, out int statu)
        {
            statu = 0;
            IResolverHandler handler = default;
            bool hasRef = mResolverIDMapper.ContainsKey(ref resolverName, out int id);
            if (hasRef)
            {
                handler = mResolvers[id];
            }
            else
            {
                statu = 1;
            }
            return handler;
        }

        public int ID
        {
            get
            {
                return (default != Binder) ? Binder.BinderID : int.MaxValue;
            }
        }

        public Type ResolveType { get; private set; }
        public ResolvableBinder Binder { get; private set; }
        public IPoolBase InstanceFactory { get; private set; }
    }
}