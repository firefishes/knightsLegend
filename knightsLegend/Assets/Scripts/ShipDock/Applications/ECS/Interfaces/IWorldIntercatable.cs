using ShipDock.Notices;

namespace ShipDock.Applications
{
    public interface IWorldIntercatable
    {
        void WorldItemHandler(INoticeBase<int> param);
        void WorldItemDispose();
    }
}