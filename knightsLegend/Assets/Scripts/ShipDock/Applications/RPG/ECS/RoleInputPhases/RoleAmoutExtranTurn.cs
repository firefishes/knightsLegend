using System;
using UnityEngine;

namespace ShipDock.Applications
{
    public class RoleAmoutExtranTurn : RoleInputDefaultPhase, IRoleInputOnlyOnce
    {
        private IRoleData mRoleData;
        
        public override void ExecuteByEntitasComponent()
        {
            if (IsExecuted)
            {
                return;
            }
            IsExecuted = true;

            base.ExecuteByEntitasComponent();

            if (mRoleItem == default || RoleInput == default)
            {
                return;
            }

            Vector3 move = Vector3.ProjectOnPlane(RoleInput.GetMoveValue(), mRoleItem.GroundNormal);
            RoleInput.SetMoveValue(move);
            RoleInput.UpdateAmout(ref mRoleItem);
            RoleInput.UpdateRoleExtraTurnRotation(ref mRoleData);

            DefaultAdvance();
        }

        public override void ExecuteBySceneComponent(Action sceneCompCallback = null)
        {
            DefaultAdvance();

            base.ExecuteBySceneComponent(sceneCompCallback);

            IsExecuted = false;
        }

        public override void SetRoleEntitas(ref ICommonRole target)
        {
            base.SetRoleEntitas(ref target);

            mRoleData = mRoleItem.RoleDataSource;
        }

        public override int[] PhasesMapper { get; } = new int[]
        {
            UserInputPhases.ROLE_INPUT_PHASE_AMOUT_EXTRAN_TURN,
            UserInputPhases.ROLE_INPUT_PHASE_CHECK_GROUND
        };

        public bool IsExecuted { get; set; }
    }
}