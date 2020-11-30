using ILRuntime.CLR.TypeSystem;
using ILRuntime.Runtime.Enviorment;
using System.Collections.Generic;

namespace ShipDock.Applications
{
    /// <summary>
    /// 方法缓存器
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class ILMethodCacher
    {
        private Dictionary<string, ILRuntimeInvokeCacher> mILMethodCachers;

        public ILMethodCacher()
        {
            mILMethodCachers = new Dictionary<string, ILRuntimeInvokeCacher>();
        }

        public void Clear()
        {
            KeyValuePair<string, ILRuntimeInvokeCacher> item;
            int max = mILMethodCachers.Count;
            var enumtor = mILMethodCachers.GetEnumerator();
            for (int i = 0; i < max; i++)
            {
                enumtor.MoveNext();
                item = enumtor.Current;
                item.Value.Clear();
            }

            mILMethodCachers.Clear();
            mILMethodCachers = default;
        }

        /// <summary>
        /// 获取缓存器中的类
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public ILRuntimeInvokeCacher GetMethodCacher(string type)
        {
            ILRuntimeInvokeCacher cacher;
            if (mILMethodCachers.ContainsKey(type))
            {
                cacher = mILMethodCachers[type];
            }
            else
            {
                cacher = new ILRuntimeInvokeCacher();
                mILMethodCachers[type] = cacher;
            }
            return cacher;
        }

        /// <summary>
        /// 获取缓存器中的方法
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public IType GetClassCache(string typeName, AppDomain appDomain)
        {
            ILRuntimeInvokeCacher cacher = GetMethodCacher(typeName);
            IType type = cacher.GetClassCache(ref appDomain, ref typeName);
            return type;
        }
    }
}