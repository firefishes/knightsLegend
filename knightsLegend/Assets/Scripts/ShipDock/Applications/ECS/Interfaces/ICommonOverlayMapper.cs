using ShipDock.ECS;

namespace ShipDock.Applications
{
    public interface ICommonOverlayMapper : IDataComponent<BehaviourIDs>
    {
        void OverlayChecked(int subgroupID, int id, bool isTrigger, bool isCollided);
    }
}