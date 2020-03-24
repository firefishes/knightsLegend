using ShipDock.ECS;
using UnityEngine;

namespace ShipDock.Applications
{

    public class RoleMoveComponent : RoleInputPhasesComponent
    {
        public override void Init()
        {
            base.Init();

            AddAllowCalled(UserInputPhases.ROLE_INPUT_PHASE_MOVE_READY);
            AddAllowCalled(UserInputPhases.ROLE_INPUT_PHASE_AMOUT_EXTRAN_TURN);
            AddAllowCalled(UserInputPhases.ROLE_INPUT_PHASE_SCALE_CAPSULE);

        }

        public override void Execute(int time, ref IShipDockEntitas target)
        {
            base.Execute(time, ref target);
        }

        protected override void InitRolePhases(IRoleInput roleInput)
        {
            base.InitRolePhases(roleInput);
            
            roleInput.AddEntitasCallback(UserInputPhases.ROLE_INPUT_PHASE_MOVE_READY, RoleMove);
            roleInput.AddEntitasCallback(UserInputPhases.ROLE_INPUT_PHASE_AMOUT_EXTRAN_TURN, RoleAmoutExtranTurn);
            roleInput.AddEntitasCallback(UserInputPhases.ROLE_INPUT_PHASE_SCALE_CAPSULE, RoleScaleCapsule);

            roleInput.SetInputPhase(UserInputPhases.ROLE_INPUT_PHASE_MOVE_READY);

        }

        private void RoleMove()
        {
            if (mRoleInput != default && mRoleInput.GetMoveValue().magnitude > 1f)
            {
                mRoleInput.MoveValueNormalize();
            }
        }

        private void RoleAmoutExtranTurn()
        {
            if (mRole == default || mRoleInput == default)
            {
                return;
            }
            mRoleData = mRole.RoleDataSource;

            Vector3 move = Vector3.ProjectOnPlane(mRoleInput.GetMoveValue(), mRole.GroundNormal);
            mRoleInput.SetMoveValue(move);
            mRoleInput.UpdateAmout(ref mRole);
            mRoleInput.UpdateRoleExtraTurnRotation(ref mRoleData);
            mRoleInput.NextPhase();
        }

        private void RoleScaleCapsule()
        {
            mRoleInput?.ScaleCapsuleForCrouching(ref mRole, ref mRoleInput);
            mRoleInput.NextPhase();
        }
    }

}