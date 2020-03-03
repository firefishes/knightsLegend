using ShipDock.Interfaces;

namespace ShipDock.ECS
{
    public interface IShipDockComponent : IDispose
    {
        void Init();
        IShipDockEntitas GetEntitas(int id);
        int SetEntitas(IShipDockEntitas target);
        int DropEntitas(IShipDockEntitas target, int entitasID);
        void Execute(int time, ref IShipDockEntitas target);
        void UpdateComponent(int time);
        void FreeComponent(int time);
        void SetComponentID(int id);
        bool Asynced { get; }
        int ID { get; }
    }
}