using System;

namespace ShipDock.Server
{
    public interface IServersHolder
    {
        int GetAliasID(ref string alias);
        void CheckAndCacheType(ref Type target, out int id);
        Type GetCachedTypeByID(int id, out int statu);
        IResolvable[] SetResolvable<InterfaceT>(ResolveDelegate<InterfaceT> target, out int statu);
        IResolvable GetResolvable(ref string alias, out int errorResult);
        IResolvable GetResolvable(int id, out int errorResult);
        T GetServer<T>(string name) where T : IServer;
        IServer GlobalServer();
    }
}