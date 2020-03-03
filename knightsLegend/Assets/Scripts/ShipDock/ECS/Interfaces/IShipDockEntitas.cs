using ShipDock.Interfaces;
using System.Collections.Generic;

namespace ShipDock.ECS
{
    public interface IShipDockEntitas : IDispose
    {
        void InitComponents();
        bool HasComponent(int componentID);
        void SetEntitasID(int id);
        void AddComponent(IShipDockComponent component);
        void RemoveComponent(IShipDockComponent component);
        int FindEntitasInComponent(IShipDockComponent component);
        List<int> ComponentList { get; }
        bool WillDestroy { get; }
        int ID { get; }
    }
}
