using ILRuntime.CLR.Method;
using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using System.Collections.Generic;

namespace ShipDock.Applications
{
    /// <summary>
    /// ILRuntime方法缓存器
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class ILRuntimeInvokeCacher
    {
        private List<Dictionary<string, IMethod>> mCacheMethodList;
        private Dictionary<string, IType> mCacheType = new Dictionary<string, IType>();
        private Dictionary<int, int> mCacherByParamCount = new Dictionary<int, int>();

        /// <summary>
        /// 添加方法缓存
        /// </summary>
        /// <param name="methodName">方法名</param>
        /// <param name="paramCountKey">使用参数个数转换的键名</param>
        /// <param name="target">ILRuntime方法</param>
        private void AddMethodToCache(string methodName, int paramCountKey, IMethod target)
        {
            if (target != default)
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
                    else { }

                    mapper = new Dictionary<string, IMethod>();
                    mCacheMethodList.Add(mapper);

                    int cacherIndex = mCacheMethodList.Count - 1;
                    mCacherByParamCount[paramCountKey] = cacherIndex;
                }
                mapper[methodName] = target;
            }
            else { }
        }

        /// <summary>
        /// 从缓存获取方法
        /// </summary>
        /// <param name="appDomain">应用域</param>
        /// <param name="type">类型名</param>
        /// <param name="methodName">方法名</param>
        /// <param name="paramCountKey">使用参数个数转换的键名</param>
        /// <returns>ILRuntime方法</returns>
        public IMethod GetMethodFromCache(AppDomain appDomain, string type, string methodName, int paramCountKey)
        {
            IMethod result;
            if (mCacherByParamCount.ContainsKey(paramCountKey))
            {
                int index = mCacherByParamCount[paramCountKey];
                Dictionary<string, IMethod> mapper = mCacheMethodList[index];
                if (mapper.ContainsKey(methodName))
                {
                    result = mapper[methodName];
                }
                else
                {
                    CreateClassAndMethodCache(ref appDomain, ref type, ref methodName, paramCountKey, out result);
                }
            }
            else
            {
                CreateClassAndMethodCache(ref appDomain, ref type, ref methodName, paramCountKey, out result);
            }
            return result;
        }

        /// <summary>
        /// 创建类与方法的缓存
        /// </summary>
        /// <param name="appDomain"></param>
        /// <param name="type"></param>
        /// <param name="methodName"></param>
        /// <param name="paramCountKey"></param>
        /// <param name="result"></param>
        private void CreateClassAndMethodCache(ref AppDomain appDomain, ref string type, ref string methodName, int paramCountKey, out IMethod result)
        {
            IType cls = GetClassCache(ref appDomain, ref type);
            result = cls.GetMethod(methodName, paramCountKey);
#if !RELEASE
            if (result == default)
            {
                UnityEngine.Debug.Log("ILRuntimeInvokeCacher error: Method is null, name is ".Append(methodName, ", param count need ", paramCountKey.ToString()));
            }
            else { }
#endif
            AddMethodToCache(methodName, paramCountKey, result);
        }

        /// <summary>
        /// 从缓存中互殴ILRuntime类
        /// </summary>
        /// <param name="appDomain"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public IType GetClassCache(ref AppDomain appDomain, ref string type)
        {
            IType cls;
            if (mCacheType.ContainsKey(type))
            {
                cls = mCacheType[type];
            }
            else
            {
                bool isExist = appDomain.LoadedTypes.ContainsKey(type);
                cls = appDomain.LoadedTypes[type];
#if !RELEASE
                if (!isExist)
                {
                    UnityEngine.Debug.Log("ILRuntimeInvokeCacher warning: Class do not exist in ILRuntime domain, class name is ".Append(type));
                }
                else { }

                if (cls == default)
                {
                    UnityEngine.Debug.Log("ILRuntimeInvokeCacher error: Class do not exist in app domain, class name is ".Append(type));
                    return default;
                }
                else { }
#endif
                mCacheType[type] = cls;
            }
            return cls;
        }

        /// <summary>
        /// 清除所有缓存
        /// </summary>
        public void Clear()
        {
            mCacheType.Clear();
            mCacherByParamCount.Clear();

            int max = mCacheMethodList != default ? mCacheMethodList.Count : 0;
            Dictionary<string, IMethod> mapper;
            for (int i = 0; i < max; i++)
            {
                mapper = mCacheMethodList[i];
                mapper.Clear();
            }
            mCacheMethodList?.Clear();
        }
    }
}
