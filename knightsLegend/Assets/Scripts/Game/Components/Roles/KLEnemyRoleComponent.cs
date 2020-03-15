namespace KLGame
{
    public class KLEnemyRoleComponent : KLRoleComponent
    {
        private IAIRole mRoleATkAI;

        protected override void SetRoleEntitas()
        {
            mRole = new EnmeyRole();
            mRoleATkAI = mRole as IAIRole;
        }
        
        protected override void UpdateAnimatorParams()
        {
            base.UpdateAnimatorParams();

            if(mRoleATkAI != default)
            {
                if(mRoleATkAI.ATKID == 1)
                {
                    mNormalAtkMotionCreater.AddComboMotion(ref m_RoleAnimator);
                }
                switch(mRoleATkAI.ATKID)
                {
                    case 1:
                        mRoleATkAI.SetNormalATKTriggerTime(UnityEngine.Random.Range(1f, 6f));
                        break;
                    case 2:
                        if(!mRoleATkAI.InATKCycle)
                        {
                            mNormalAtkMotionCreater.AddComboMotion(ref m_RoleAnimator);
                            mRoleATkAI.InATKCycle = true;
                        }
                        break;
                }
            }
        }

        protected override void OnAtk1Completed()
        {
            base.OnAtk1Completed();

            mRoleATkAI.SetATKID(0);
            mRoleATkAI.InATKCycle = false;
        }
    }
}
