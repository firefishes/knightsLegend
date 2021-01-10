using System;

namespace ShipDock
{
    public interface ICustomFramework
    {
        bool IsStarted { get; }
        IFrameworkUnit [] FrameworkUnits { get; }

        void SetStarted(bool value);
        void AddStart(Action method);
    }
}
