using System;

namespace ShipDock.Applications
{
    public interface IRoleInputPhase
    {
        void SetRoleInput(IRoleInput target);
        void SetRoleEntitas(ref ICommonRole target);
        void ExecuteBySceneComponent(Action sceneCompCallback = default);
        void ExecuteByEntitasComponent();
        int AdvancedInputPhase();
        void DefaultAdvance();
        int[] PhasesMapper { get; }
        int PhaseTransitions { get; set; }
    }
}
