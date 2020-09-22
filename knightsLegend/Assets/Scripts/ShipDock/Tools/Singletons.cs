using System;

namespace ShipDock.Tools
{

    /// <summary>
    /// 
    /// 范型单例基类
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public abstract class Singletons<T> where T : Singletons<T>, new()
    {
        #region 静态成员
        private string SINGLETON_ERROR = "'s singleton is exsited.";
        private static T instance;

        private static void CheckInstanceNull()
        {
            if (instance == null)
            {
                instance = new T();
            }
        }

        public static T Instance
        {
            get
            {
                CheckInstanceNull();
                return instance;
            }
        }
        #endregion

        public Singletons()
        {
            if (instance != null)
            {
                throw new Exception(typeof(T).Name.Append(SINGLETON_ERROR));
            }
        }

        public virtual void Dispose()
        {
            instance = default;
        }
	}
}