using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Server;

public static class ServerExtension
{
    public static I Delive<S, I>(this string serverName, string resolverName, string alias) where S : IServer
    {
        S server = serverName.GetServer<S>();
        return server.Delive<I>(resolverName, alias);
    }

    public static P DeliveParam<S, P>(this string serverName, string resolverName, string alias, ResolveDelegate<IParamNotice<P>> newResolver = default, bool isReregister = false) where S : IServer
    {
        S server = serverName.GetServer<S>();
        IParamNotice<P> notice;
        if (newResolver != default)
        {
            if(isReregister)
            {
                ResolveDelegate<IParamNotice<P>> raw = server.Reregister(newResolver, alias);
                notice = server.Delive<IParamNotice<P>>(resolverName, alias);
                server.Reregister(raw, alias);
            }
            else
            {
                notice = server.Delive<IParamNotice<P>>(resolverName, alias);
                newResolver.Invoke(ref notice);
            }
        }
        else
        {
            notice = server.Delive<IParamNotice<P>>(resolverName, alias);
        }
        return notice.ParamValue;
    }

    public static P DeliveParam<S, P>(this string serverName, string alias, ResolveDelegate<IParamNotice<P>> newResolver = default) where S : IServer
    {
        return serverName.DeliveParam<S, P>(string.Empty, alias, newResolver);
    }

    public static void Revert<S>(this string serverName, string alias, IPoolable target) where S : IServer
    {
        S server = serverName.GetServer<S>();
        server?.Revert(target, alias);
    }
}
