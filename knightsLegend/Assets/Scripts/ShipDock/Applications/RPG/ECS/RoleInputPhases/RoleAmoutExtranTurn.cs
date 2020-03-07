using UnityEngine;

namespace ShipDock.Applications
{
    public class RoleAmoutExtranTurn : RoleInputDefaultPhase
    {
        private IRoleData mRoleData;
        private ICommonRole mRoleItem;

        public RoleAmoutExtranTurn(ICommonRole item)
        {
            mRoleItem = item;
            mRoleData = mRoleItem.RoleDataSource;
        }

        public override void ExecuteByEntitasComponent()
        {
            Vector3 move = Vector3.ProjectOnPlane(RoleInput.GetMoveValue(), mRoleItem.GroundNormal);
            RoleInput.SetMoveValue(move);
            RoleInput.UpdateAmout(ref mRoleItem);
            RoleInput.UpdateRoleExtraTurnRotation(ref mRoleData);
            RoleInput.AdvancedInputPhase();
        }
    }
}