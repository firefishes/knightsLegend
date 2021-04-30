using ShipDock.Tools;

namespace ShipDock.Ticks
{
    public class ShipDockTicks : Singletons<ShipDockTicks>
    {
        public TicksUpdater TicksUpdater { get; private set; }

        public ShipDockTicks()
        {
            TicksUpdater = new TicksUpdater(60);
        }
    }
}
