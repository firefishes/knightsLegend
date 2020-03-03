using System;

namespace ShipDock.Server
{
    public struct ResolvableInfo
    {
        public static void FillResolvableInfo(int id, ref IResolvableConfig item, out ResolvableInfo info)
        {
            info = new ResolvableInfo()
            {
                configID = id,
                aliasID = item.AliasID,
                interfaceID = item.InterfaceID,
                typeID = item.TypeID
            };
        }

        public int configID;
        public int aliasID;
        public int interfaceID;
        public int typeID;
    }
}