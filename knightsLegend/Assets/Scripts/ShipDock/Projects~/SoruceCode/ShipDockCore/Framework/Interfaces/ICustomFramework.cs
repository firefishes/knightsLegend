using System;

namespace ShipDock
{
    public interface ICustomFramework
    {
        bool IsStarted { get; }
        IFrameworkUnit [] FrameworkUnits { get; }
        IUpdatesComponent UpdatesComponent { get; }
        Action MergeCallOnMainThread { get; set; }

        void Start(int ticks);
        void SetUpdatesComp(IUpdatesComponent component);
        void SetStarted(bool value);
        void AddStart(Action method);
        void Clean();
    }
}
