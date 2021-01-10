using ShipDock.Interfaces;
using ShipDock.Pooling;
using System;

namespace ShipDock.Server
{
    public interface IResolvable : IDispose
    {
        void Binding(ref ResolvableBinder target);
        void InitResolver<InterfaceT>(IServersHolder serverHolder, IPoolBase factory = default);
        void SetResolver<InterfaceT>(string resolverName, ResolveDelegate<InterfaceT> resolveDelgate, out int statu, bool onlyOnce = false, bool isMakeResolver = false);
        IResolverHandler GetResolver<InterfaceT>(string resolverName, out int statu);
        void RemoveResolver<InterfaceT>(string resolverName, out int statu);
        void RevokeResolver<InterfaceT>(string resolverName, ResolveDelegate<InterfaceT> resolveDelgate);
        Type ResolveType { get; }
        ResolvableBinder Binder { get; }
        IPoolBase InstanceFactory { get; }
        int ID { get; }
    }
}