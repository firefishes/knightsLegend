namespace KLGame
{
    public class KLEnemyRoleComponent : KLRoleComponent
    {
        protected override void SetRoleEntitas()
        {
            mRole = new EnmeyRole();
        }
        
        protected override void UpdateAnimatorParams()
        {
            base.UpdateAnimatorParams();

        }
    }
}
