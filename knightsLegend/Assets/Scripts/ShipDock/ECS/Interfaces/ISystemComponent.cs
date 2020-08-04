namespace ShipDock.ECS
{
    public interface ISystemComponent : IShipDockComponent
    {
        void FillRelateComponents(IShipDockComponentManager manager);
        void ComponentEntitasStretch(IShipDockEntitas entitas, bool isRemove);
        int[] RelateComponents { get; set; }
    }
}