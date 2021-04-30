using ShipDock.Interfaces;
using System;

namespace ShipDock.Server
{
    public interface IResolvableConfig : IDispose
    {
        void Create(IServersHolder servers);
        int TypeID { get; }
        int InterfaceID { get; }
        int AliasID { get; }
        Type Type { get; }
        Type InterfaceType { get; }
        string Alias { get; }
    }
}