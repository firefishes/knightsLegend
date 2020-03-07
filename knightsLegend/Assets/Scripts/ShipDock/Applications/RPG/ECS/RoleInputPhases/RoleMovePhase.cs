namespace ShipDock.Applications
{
    public class RoleMovePhase : RoleInputDefaultPhase
    {
        public override void ExecuteByEntitasComponent()
        {
            if (RoleInput.GetMoveValue().magnitude > 1f)
            {
                RoleInput.MoveValueNormalize();
            }
        }
    }

}
