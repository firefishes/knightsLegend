using System;
using System.Collections.Generic;

namespace ShipDock.ECS
{
    public interface IShipDockComponentContext
    {
        T GetEntitasWithComponents<T>(params int[] aidArgs) where T : IShipDockEntitas, new();
        void RefComponentByIndex(int value, bool isSceneUpdate, ref IShipDockComponent comp);
        void UpdateComponentUnit(int time, Action<Action<int>> method);
        void FreeComponentUnit(int time, Action<Action<int>> method);
        void UpdateAndFreeComponents(int time, Action<Action<int>> method);
        void UpdateComponentUnitInScene(int time, Action<Action<int>> method);
        void FreeComponentUnitInScene(int time, Action<Action<int>> method);
        void UpdateAndFreeComponentsInScene(int time, Action<Action<int>> method);
        int Create<T>(int aid, bool isUpdateByScene = false, params int[] willRelateComponents) where T : IShipDockComponent, new();
        IShipDockComponent RefComponentByName(int name);
        void RemoveComponent(IShipDockComponent target);
        Action<int, IShipDockComponent, IShipDockComponentContext> RelateComponentsReFiller { get; set; }
        Action<List<int>, bool> PreUpdate { get; set; }
        int CountTime { get; }
        int FrameTimeInScene { get; set; }
    }
}