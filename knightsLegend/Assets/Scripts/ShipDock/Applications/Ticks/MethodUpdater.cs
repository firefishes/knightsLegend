using System;
using ShipDock.Interfaces;

namespace ShipDock.Applications
{
    public class MethodUpdater : IUpdate, IDispose
    {

        public virtual void Dispose()
        {
            Asynced = false;
            Update = default;
            FixedUpdate = default;
            LateUpdate = default;
        }

        public void AddUpdate()
        {
        }

        public void OnFixedUpdate(int dTime)
        {
            Asynced = false;
            FixedUpdate?.Invoke(dTime);
        }

        public void OnLateUpdate()
        {
            Asynced = true;
            LateUpdate?.Invoke();
        }

        public virtual void OnUpdate(int dTime)
        {
            Update?.Invoke(dTime);
        }

        public void RemoveUpdate()
        {
        }

        public Action<int> Update { get; set; }
        public Action<int> FixedUpdate { get; set; }
        public Action LateUpdate { get; set; }
        public bool IsUpdate { get; set; } = true;
        public bool IsFixedUpdate { get; set; } = true;
        public bool IsLateUpdate { get; set; } = true;
        public bool Asynced { get; set; }
    }

}
