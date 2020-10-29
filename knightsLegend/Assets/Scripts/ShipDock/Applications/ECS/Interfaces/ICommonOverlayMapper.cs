using ShipDock.ECS;

namespace ShipDock.Applications
{
    /// <summary>
    /// 通用覆盖检测映射器接口
    /// </summary>
    public interface ICommonOverlayMapper : IDataComponent<BehaviourIDs>
    {
        bool GetPhysicsChecked(int gameItemID);
        void PhysicsChecked(int gameItemID, bool isInit = false);
        void PhysicsCheckReset(int gameItemID);
        void OverlayChecked(int subgroupID, int id, bool isTrigger, bool isCollided);
        void RemovePhysicsChecker(int subgroupID);
    }
}