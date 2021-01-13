namespace ShipDock
{
    public class FrameworkUnitBrige<T> : IFrameworkUnit
    {
        public int Name { get; private set; }

        public T Unit { get; private set; }

        public FrameworkUnitBrige(int name, T unit)
        {
            Name = name;
            Unit = unit;
        }
    }
}
