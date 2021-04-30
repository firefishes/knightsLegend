using ShipDock.Interfaces;
using ShipDock.Tools;
using System;

namespace ShipDock.Server
{
    public class ResolvableBinder : IDispose
    {
        public static ResolvableBinder CreateResolvableRef(ref ResolvableInfo info, ref IntegerMapper<Type> typeIDMapper)
        {
            ResolvableBinder result = new ResolvableBinder(ref typeIDMapper)
            {
                alias = info.aliasID,
                interfaceType = info.interfaceID,
                insType = info.typeID
            };
            return result;
        }

        public int alias;
        public int insType;
        public int interfaceType;

        private IntegerMapper<Type> mTypeID;

        public ResolvableBinder(ref IntegerMapper<Type> types)
        {
            mTypeID = types;
        }

        public void Dispose()
        {
            alias = -1;
            interfaceType = -1;
            mTypeID = default;
        }

        public Type GetInstanceType(IServersHolder serverHolder)
        {
            return serverHolder.GetCachedTypeByID(insType, out int statu);
        }

        public void SetNextRef(ResolvableBinder next)
        {
            if (NextRef == default)
            {
                NextRef = next;
            }
        }

        public void RecursiveAndCheckRef(ref ResolvableInfo info, bool isCreateWhenEmpty, out ResolvableBinder result)
        {
            if (IsSameRef(ref info))
            {
                result = this;
            }
            else
            {
                if (NextRef == default)
                {
                    if (isCreateWhenEmpty)
                    {
                        result = CreateResolvableRef(ref info, ref mTypeID);
                        SetNextRef(result);
                    }
                    else
                    {
                        result = default;
                        return;
                    }
                }
                else
                {
                    NextRef.RecursiveAndCheckRef(ref info, isCreateWhenEmpty, out result);
                }
            }
        }

        public bool IsSameRef(ref ResolvableInfo info)
        {
            return alias.Equals(info.aliasID) && 
                    (info.interfaceID == interfaceType) && 
                    (info.typeID == insType);
        }

        public void SetID(int id)
        {
            BinderID = id;
        }

        public ResolvableBinder NextRef { get; private set; }
        public int BinderID { get; private set; }
    }
}
