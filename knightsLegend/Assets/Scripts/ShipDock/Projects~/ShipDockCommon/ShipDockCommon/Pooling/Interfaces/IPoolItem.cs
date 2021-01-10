
namespace ShipDock.Pooling
{
    /// <summary>
    /// 
    /// 可池化的对象接口
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public interface IPoolable
    {
        void Revert();
        void ToPool();
    }
}