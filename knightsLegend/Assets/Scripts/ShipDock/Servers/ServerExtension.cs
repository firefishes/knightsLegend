using ShipDock.Notices;
using ShipDock.Server;

public static class ServerExtension
{
    public static I Delive<S, I>(this string serverName, string resolverName, string alias) where S : IServer
    {
        S server = serverName.GetServer<S>();
        return server.Delive<I>(resolverName, alias);
    }

    public static P DeliveParam<S, P>(this string serverName, string resolverName, string alias, ResolveDelegate<IParamNotice<P>> newResolver = default) where S : IServer
    {
        S server = serverName.GetServer<S>();
        IParamNotice<P> notice;
        if (newResolver != default)
        {
            ResolveDelegate<IParamNotice<P>> raw = server.Reregister(newResolver, alias);
            notice = server.Delive<IParamNotice<P>>(resolverName, alias);
            server.Reregister(raw, alias);
        }
        else
        {
            notice = server.Delive<IParamNotice<P>>(resolverName, alias);
        }
        return notice.ParamValue;
    }
}
