using ShipDock.ECS;
using System;

namespace ShipDock.Applications
{
    public interface IProcessingComponent : IShipDockComponent
    {
        bool AddProcess(Action<IProcessing> method);
    }

}