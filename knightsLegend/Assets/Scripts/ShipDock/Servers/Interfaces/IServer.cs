
using ShipDock.Interfaces;
using ShipDock.Pooling;

namespace ShipDock.Server
{
    public interface IServer : IDispose
    {
        void InitServer();
        void ServerReady();
        void SetServerHolder(IServersHolder servers);
        int Register<InterfaceT>(ResolveDelegate<InterfaceT> target, params IPoolBase[] factory);
        ResolveDelegate<InterfaceT> Reregister<InterfaceT>(ResolveDelegate<InterfaceT> target, string alias);
        void Unregister<InterfaceT>(string alias);
        InterfaceT Resolve<InterfaceT>(string alias,  string resolverName = "");
        void Add<InterfaceT>(ResolveDelegate<InterfaceT> target, bool onlyOnce = false);
        InterfaceT Delive<InterfaceT>(string resolverName, string alias);
        void Revert(IPoolable target, string alias);
        IServersHolder ServersHolder { get; }
        int Prioriity { get; set; }
        string ServerName { get; }
    }
}