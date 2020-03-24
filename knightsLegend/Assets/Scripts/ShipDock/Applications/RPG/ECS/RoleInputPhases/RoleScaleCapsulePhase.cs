using System;

namespace ShipDock.Applications
{
    public class RoleScaleCapsulePhase : RoleInputDefaultPhase, IRoleInputOnlyOnce
    {
        private IRoleInput mRoleInput;
        
        public override void ExecuteByEntitasComponent()
        {
            if (IsExecuted)
            {
                return;
            }
            IsExecuted = true;

            base.ExecuteByEntitasComponent();

            if (mRoleInput == default)
            {
                mRoleInput = RoleInput;
            }
            if (RoleInput == default)
            {
                IsExecuted = false;
                return;
            }

            RoleInput?.ScaleCapsuleForCrouching(ref mRoleItem, ref mRoleInput);

            DefaultAdvance();
        }

        public override void ExecuteBySceneComponent(Action sceneCompCallback = null)
        {
            DefaultAdvance();

            base.ExecuteBySceneComponent(sceneCompCallback);

            IsExecuted = false;
        }

        public override int[] PhasesMapper { get; } = new int[]
        {
            UserInputPhases.ROLE_INPUT_PHASE_SCALE_CAPSULE,
            UserInputPhases.ROLE_INPUT_PHASE_CHECK_CROUCH
        };

        public bool IsExecuted { get; set; }
    }
}