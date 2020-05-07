using ShipDock.Interfaces;

namespace ShipDock.ECS
{
    public interface IShipDockComponent : IDispose
    {
        void Init();
        void FillRelateComponents(IShipDockComponentManager manager);
        IShipDockEntitas GetEntitas(int id);
        int SetEntitas(IShipDockEntitas target);
        int DropEntitas(IShipDockEntitas target, int entitasID);
        void Execute(int time, ref IShipDockEntitas target);
        void UpdateComponent(int time);
        void FreeComponent(int time);
        void SetComponentID(int id);
        void SetSceneUpdate(bool value);
        bool IsVariableFrame { get; set; }
        int ID { get; }
        bool IsSceneUpdate { get; }
        int[] RelateComponents { get; set; }
    }
}