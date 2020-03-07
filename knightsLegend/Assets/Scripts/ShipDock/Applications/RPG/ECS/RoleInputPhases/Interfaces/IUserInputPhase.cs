using System;

namespace ShipDock.Applications
{
    public interface IUserInputPhase
    {
        void SetRoleInput(IRoleInput target);
        void ExecuteBySceneComponent(Action sceneCompCallback = default);
        void ExecuteByEntitasComponent();
    }
}
