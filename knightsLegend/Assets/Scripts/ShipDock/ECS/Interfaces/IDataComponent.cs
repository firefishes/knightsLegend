namespace ShipDock.ECS
{
    public interface IDataComponent<T> : IShipDockComponent, IDataValidable
    {
        void FillEntitasData(ref IShipDockEntitas target, T data);
        T GetEntitasData<E>(ref E target) where E : IShipDockEntitas;
        bool IsDataValid(ref IShipDockEntitas target);
    }

    public interface IDataValidable
    {
        void SetDataValidable<E>(bool value, ref E target) where E : IShipDockEntitas;
    }
}