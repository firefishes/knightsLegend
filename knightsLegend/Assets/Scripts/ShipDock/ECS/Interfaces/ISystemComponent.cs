namespace ShipDock.ECS
{
    public interface ISystemComponent : IShipDockComponent
    {
        void FillRelateComponents(IShipDockComponentManager manager);
        void ComponentEntitasStretch(IShipDockEntitas entitas, bool isRemove);
        T GetRelatedComponent<T>(int aid) where T : IShipDockComponent;
        int[] RelateComponents { get; set; }
    }
}