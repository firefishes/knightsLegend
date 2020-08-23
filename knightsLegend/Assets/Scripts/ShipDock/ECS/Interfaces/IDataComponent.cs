namespace ShipDock.ECS
{
    public interface IDataComponent<T> : IShipDockComponent, IDataValidable
    {
        void FillEntitasData<E>(ref E target, T data) where E : IShipDockEntitas;
        T GetEntitasData<E>(ref E target) where E : IShipDockEntitas;
        bool IsDataValid<E>(ref E target) where E : IShipDockEntitas;
    }

    public interface IDataValidable
    {
        void SetDataValidable<E>(bool value, ref E target) where E : IShipDockEntitas;
    }
}