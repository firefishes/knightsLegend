
namespace ShipDock.Pooling
{
    public interface IPoolBase
    {
        IPoolable GetInstance();
        void Reserve(ref IPoolable item);
    }

}