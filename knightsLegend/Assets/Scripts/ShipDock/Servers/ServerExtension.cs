using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Server;

public static class ServerExtension
{
    public static void DeliveAndRevert<I>(this string serverName, string resolverName, string alias, ResolveDelegate<I> customResolver = default, bool isMakeResolver = false, bool isReregister = false) where I : IPoolable
    {
        I notice = serverName.Delive<I>(resolverName, alias);
        serverName.Revert(alias, notice);
    }

    public static I Delive<I>(this string serverName, string resolverName, string alias, ResolveDelegate<I> customResolver = default, bool isMakeResolver = false, bool isReregister = false)
    {
        I result;
        IServer server = serverName.GetServer();
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

    public static P DeliveParam<P>(this string serverName, string resolverName, string alias, ResolveDelegate<IParamNotice<P>> customResolver = default, bool isMakeResolver = false, bool isReregister = false)
    {
        IParamNotice<P> notice = Delive(serverName, resolverName, alias, customResolver, isMakeResolver, isReregister);

        P result = notice.ParamValue;

        serverName.Revert(alias, notice);
        return result;
    }

    public static P Resolve<P>(this string serverName, string alias, ResolveDelegate<P> customResolver = default, string resolverName = "")
    {
        IServer server = serverName.GetServer();
        P result = server.Resolve(alias, resolverName, customResolver);
        return result;
    }

    public static void Revert(this string serverName, string alias, IPoolable target)
    {
        IServer server = serverName.GetServer();
        server?.Revert(target, alias);
    }
}
