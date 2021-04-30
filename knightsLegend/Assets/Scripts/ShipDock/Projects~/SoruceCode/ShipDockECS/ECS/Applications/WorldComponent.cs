using ShipDock.ECS;

namespace ShipDock.Applications
{
    public abstract class WorldComponent : StaticWorldComponent
    {
        public override void Init(IShipDockComponentContext context)
        {
            base.Init(context);
        }

        public virtual int WorldGroupComponentName { get; } = int.MaxValue;
        public abstract int BehaviaourIDsComponentName { get; }
    }
}