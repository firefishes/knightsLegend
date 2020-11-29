using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using System.Collections.Generic;

namespace ShipDock.Applications
{
    public class ILRuntimeInvokeCacher
    {
        private List<Dictionary<string, IMethod>> mCacheMethodList;
        private Dictionary<string, IType> mCacheType = new Dictionary<string, IType>();
        private Dictionary<int, int> mCacherByParamCount = new Dictionary<int, int>();

        private void Add(string methodName, int paramCountKey, IMethod target)
        {
            Dictionary<string, IMethod> mapper;
            if (mCacherByParamCount.ContainsKey(paramCountKey))
            {
                int cacherIndex = mCacherByParamCount[paramCountKey];
                mapper = mCacheMethodList[cacherIndex];
            }
            else
            {
                if (mCacheMethodList == default)
                {
                    mCacheMethodList = new List<Dictionary<string, IMethod>>();
                }

                mapper = new Dictionary<string, IMethod>();
                mCacheMethodList.Add(mapper);

                int cacherIndex = mCacheMethodList.Count - 1;
                mCacherByParamCount[paramCountKey] = cacherIndex;
            }
            mapper[methodName] = target;
        }

        public IMethod GetMethodFromCache(AppDomain appDomain, string type, string methodName, int paramCountKey)
        {
            IMethod result;
            if (mCacherByParamCount.ContainsKey(paramCountKey))
            {
                int index = mCacherByParamCount[paramCountKey];
                Dictionary<string, IMethod> mapper = mCacheMethodList[index];
                result = mapper[methodName];
            }
            else
            {
                IType cls = GetClsCache(ref appDomain, ref type);
                result = cls.GetMethod(methodName, paramCountKey);

                Add(methodName, paramCountKey, result);
            }
            return result;
        }

        public IType GetClsCache(ref AppDomain appDomain, ref string type)
        {
            IType cls;
            if (mCacheType.ContainsKey(type))
            {
                cls = mCacheType[type];
            }
            else
            {
                cls = appDomain.LoadedTypes[type];
                mCacheType[type] = cls;
            }
            return cls;
        }

        public void Clear()
        {
            mCacheType.Clear();
            mCacherByParamCount.Clear();

            int max = mCacheMethodList.Count;
            Dictionary<string, IMethod> mapper;
            for (int i = 0; i < max; i++)
            {
                mapper = mCacheMethodList[i];
                mapper.Clear();
            }
            mCacheMethodList.Clear();
        }
    }
}
