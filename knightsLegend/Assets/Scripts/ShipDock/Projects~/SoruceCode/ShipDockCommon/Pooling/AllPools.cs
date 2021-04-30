using System;
using System.Collections.Generic;

namespace ShipDock.Pooling
{
    /// <summary>
    /// 
    /// 对象池工具静态类
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public static class AllPools
    {
#if UNITY_EDITOR
        /// <summary>使用中的实例</summary>
        public static List<IPoolable> used = new List<IPoolable>();
#endif
        private static Action onResetAllPooling;

        public static void ResetAllPooling()
        {
            onResetAllPooling?.Invoke();
        }

        public static void AddReset(Action onClearPool)
        {
            onResetAllPooling += onClearPool;
        }
    }
}