using ShipDock.Tools;

namespace ShipDock.ECS
{
    public class ShipDockECS : Singletons<ShipDockECS>
    {
        public IShipDockComponentContext Context { get; private set; } = Framework.Instance.GetUnit<IShipDockComponentContext>(Framework.UNIT_ECS);
    }
}
