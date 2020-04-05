using ShipDock.ECS;

namespace ShipDock.Applications
{
    public interface IProcessingComponent : IShipDockComponent
    {
        RoleColliderComponent RoleCollisionComp { get; }
    }

}