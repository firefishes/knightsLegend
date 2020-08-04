using ShipDock.Interfaces;
using System;

namespace ShipDock.ECS
{
    public interface IShipDockComponent : IDispose
    {
        void Init(IShipDockComponentManager manager);
        IShipDockEntitas GetEntitas(int id);
        void GetEntitasRef(int id, out IShipDockEntitas entitas);
        int SetEntitas(IShipDockEntitas target);
        int DropEntitas(IShipDockEntitas target, int entitasID);
        void Execute(int time, ref IShipDockEntitas target);
        void UpdateComponent(int time);
        void FreeComponent(int time);
        void SetComponentID(int id);
        void SetSceneUpdate(bool value);

        #region TODO 系统特性，需要迁移
        void SystemChecked();
        bool IsSystemChanged { get; set; }
        bool IsSystem { get; }
        #endregion

        Action<int> OnFinalUpdateForTime { set; }
        Action<IShipDockEntitas> OnFinalUpdateForEntitas { set; }
        Action<Action<int, IShipDockEntitas>> OnFinalUpdateForExecute { set; }
        Action<IShipDockEntitas, bool> OnEntitasStretch { get; set; }
        bool IsSceneUpdate { get; }
        int ID { get; }
    }
}