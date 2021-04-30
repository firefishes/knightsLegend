using System;

namespace ShipDock.Server
{
    public class ResolvableConfigItem<InterfaceT, T> : IResolvableConfig
    {
        private static Type typeTemp;
        private static string aliasTemp;

        public ResolvableConfigItem(string alias)
        {
            Alias = alias;
            InterfaceType = typeof(InterfaceT);
            Type = typeof(T);
        }

        public void Dispose()
        {
            Alias = string.Empty;
            Type = default;
            InterfaceType = default;
        }

        public void Create(IServersHolder servers)
        {
            aliasTemp = Alias;
            AliasID = servers.GetAliasID(ref aliasTemp);
            Alias = string.Empty;

            typeTemp = Type;
            servers.GetAliasID(ref aliasTemp);
            servers.CheckAndCacheType(ref typeTemp, out int id);
            Type = default;

            TypeID = id;

            typeTemp = InterfaceType;
            servers.CheckAndCacheType(ref typeTemp, out id);
            InterfaceType = default;

            InterfaceID = id;

            aliasTemp = string.Empty;
            typeTemp = default;
        }

        public int TypeID { get; private set; }
        public int InterfaceID { get; private set; }
        public int AliasID { get; private set; }
        public Type Type { get; private set; }
        public Type InterfaceType { get; private set; }
        public string Alias { get; private set; }
    }
}
