namespace ShipDock.Applications
{
    public class RoleScaleCapsulePhase : RoleInputDefaultPhase
    {
        private ICommonRole mRoleItem;
        private IRoleInput mRoleInput;

        public RoleScaleCapsulePhase(ICommonRole item)
        {
            mRoleItem = item;
        }

        public override void ExecuteByEntitasComponent()
        {
            if (mRoleInput == default)
            {
                mRoleInput = RoleInput;
            }
            RoleInput.ScaleCapsuleForCrouching(ref mRoleItem, ref mRoleInput);
            RoleInput.AdvancedInputPhase();
        }
    }
}