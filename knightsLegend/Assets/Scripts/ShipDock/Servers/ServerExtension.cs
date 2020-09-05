using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Server;

public static class ServerExtension
{
    public static I Delive<S, I>(this string serverName, string resolverName, string alias, ResolveDelegate<I> customResolver = default, bool isMakeResolver = false, bool isReregister = false) where S : IServer
    {
        I result;
        S server = serverName.GetServer<S>();
        if (customResolver != default)
        {
            if (isReregister)
            {
                ResolveDelegate<I> raw = server.Reregister(customResolver, alias);
                result = server.Delive<I>(resolverName, alias);
                server.Reregister(raw, alias);
            }
            else
            {
                result = server.Delive(resolverName, alias, customResolver, isMakeResolver);
            }
        }
        else
        {
            result = server.Delive<I>(resolverName, alias);
        }
        return result;
    }

    public static P DeliveParam<S, P>(this string serverName, string resolverName, string alias, ResolveDelegate<IParamNotice<P>> customResolver = default, bool isMakeResolver = false, bool isReregister = false) where S : IServer
    {
        IParamNotice<P> notice = Delive<S, IParamNotice<P>>(serverName, resolverName, alias, customResolver, isMakeResolver, isReregister);

        P result = notice.ParamValue;

        serverName.Revert<S>(alias, notice);
        return result;
    }

    public static void Revert<S>(this string serverName, string alias, IPoolable target) where S : IServer
    {
        S server = serverName.GetServer<S>();
        server?.Revert(target, alias);
    }
}
