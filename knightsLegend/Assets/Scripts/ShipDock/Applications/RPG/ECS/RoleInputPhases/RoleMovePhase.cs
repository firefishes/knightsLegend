using System;

namespace ShipDock.Applications
{
    public class RoleMovePhase : RoleInputDefaultPhase, IRoleInputOnlyOnce
    {

        public override void ExecuteByEntitasComponent()
        {
            if(IsExecuted)
            {
                return;
            }
            IsExecuted = true;

            base.ExecuteByEntitasComponent();

            if (RoleInput != default && RoleInput.GetMoveValue().magnitude > 1f)
            {
                RoleInput.MoveValueNormalize();
            }
        }

        public override void ExecuteBySceneComponent(Action sceneCompCallback = null)
        {
            DefaultAdvance();
            base.ExecuteBySceneComponent(sceneCompCallback);
            
            IsExecuted = false;
        }

        public override int[] PhasesMapper { get; } = new int[]
        {
            UserInputPhases.ROLE_INPUT_PHASE_MOVE_READY,
            UserInputPhases.ROLE_INPUT_PHASE_AMOUT_EXTRAN_TURN
        };

        public bool IsExecuted { get; set; }
    }

}
