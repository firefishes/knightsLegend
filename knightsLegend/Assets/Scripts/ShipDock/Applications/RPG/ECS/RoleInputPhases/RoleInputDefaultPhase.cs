using ShipDock.Interfaces;
using System;

namespace ShipDock.Applications
{
    public class RoleInputDefaultPhase : IUserInputPhase, IDispose
    {
        public virtual void Dispose()
        {
            RoleInput = default;
        }

        public virtual void ExecuteByEntitasComponent()
        {
        }

        public virtual void ExecuteBySceneComponent(Action sceneCompCallback = default)
        {
            sceneCompCallback?.Invoke();
        }

        public void SetRoleInput(IRoleInput target)
        {
            RoleInput = target;
        }

        protected IRoleInput RoleInput { get; private set; }
    }
}