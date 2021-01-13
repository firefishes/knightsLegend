using ShipDock.ECS;

namespace ShipDock.Applications
{
    public abstract class StaticWorldComponent : DataComponent<WorldInteracter>
    {
        protected override WorldInteracter CreateData()
        {
            return new WorldInteracter();
        }
    }
}