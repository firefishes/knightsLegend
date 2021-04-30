namespace ShipDock
{
    /// <summary>
    /// 
    /// 定制框架桥接单元
    /// 
    /// 在不实现 IFrameworkUnit 接口的情况下往定制框架中增加框架单元
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class FrameworkUnitBrige<T> : IFrameworkUnit
    {
        /// <summary>单元名</summary>
        public int Name { get; private set; }

        /// <summary>单元的对象引用</summary>
        public T Unit { get; private set; }

        public FrameworkUnitBrige(int name, T unit)
        {
            Name = name;
            Unit = unit;
        }
    }
}
