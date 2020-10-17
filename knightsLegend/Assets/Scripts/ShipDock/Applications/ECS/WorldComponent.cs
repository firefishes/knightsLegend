using ShipDock.ECS;

namespace ShipDock.Applications
{
    public abstract class WorldComponent : StaticWorldComponent
    {
        public override void Init(IShipDockComponentContext context)
        {
            base.Init(context);

            ClusteringComp = context.RefComponentByName(WorldGroupComponentName) as ClusteringComponent;
            BehaviourIDsComp = context.RefComponentByName(BehaviaourIDsComponentName) as BehaviourIDsComponent;

            ShouldWorldGroupable = ClusteringComp != default;
        }

        public ClusteringData GetWorldGroupData(ref IShipDockEntitas target)
        {
            return ShouldWorldGroupable ? ClusteringComp.GetEntitasData(ref target) : default;
        }

        protected ClusteringComponent ClusteringComp { get; private set; }
        protected BehaviourIDsComponent BehaviourIDsComp { get; private set; }

        public virtual int WorldGroupComponentName { get; } = int.MaxValue;
        public abstract int BehaviaourIDsComponentName { get; }
        public bool ShouldWorldGroupable { get; private set; }
    }
}