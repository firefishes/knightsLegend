using ShipDock.Applications;
using ShipDock.ECS;
using ShipDock.Pooling;

namespace KLGame
{
    public class KLEnemyRoleComponent : KLRoleComponent
    {
        private IAIRole mRoleATkAI;

        protected override void OnInited()
        {
            base.OnInited();

            RoleFSM = (mRole.RoleInput as KLRoleInputInfo).AnimatorFSM;
            (RoleFSM as CommonRoleFSM).SetAnimator(ref m_RoleAnimator);
            RoleFSM.Run(default, NormalRoleStateName.GROUNDED);

        }

        protected override void SetRoleEntitas()
        {
            mRole = new EnmeyRole();
            mRoleATkAI = mRole as IAIRole;
        }

        protected override void InitRoleInputCallbacks()
        {
            base.InitRoleInputCallbacks();

            SetRoleInputCallback(EnemyInputPhases.ENEMY_INPUT_PHASE_SET_NROMAL_ATK_TRIGGER_TIME, OnSetNormalATKTriggerTime);
            SetRoleInputCallback(EnemyInputPhases.ENEMY_INPUT_PHASE_NROMAL_ATK, OnEnemyNormalATK);
        }

        private void OnEnemyNormalATK()
        {
            if (!mRoleATkAI.InATKCycle)
            {
                mRoleATkAI.InATKCycle = true;

                CurrentSkillID = 1;
                MoveBlock = true;

                NormalATKStateParam param = Pooling<NormalATKStateParam>.From();
                param.Reinit(this, 1);

                if (RoleFSM.Current.StateName == NormalRoleStateName.NORMAL_ATK)
                {
                    RoleFSM.Current.SetStateParam(param);
                }
                else
                {
                    RoleFSM.ChangeState(NormalRoleStateName.NORMAL_ATK, param);
                }
            }
        }

        private void OnSetNormalATKTriggerTime()
        {
            TimingTasker target = mRoleATkAI.TimesEntitas.GetRoleTiming(RoleTimingTaskNames.NORMAL_ATK_TIME);
            if (target.RunCounts > 0)
            {
                mRoleATkAI.StartTimingTask(RoleTimingTaskNames.NORMAL_ATK_TIME, UnityEngine.Random.Range(1f, 5f));
                mRoleInput.NextPhase();
            }
            else
            {
                target.DirectCompletion();
                OnEnemyNormalATK();
            }
        }

        public override void OnATKCompleted()
        {
            switch(CurrentSkillID)
            {
                case 1:
                    mRoleATkAI.InATKCycle = false;
                    mRoleInput.SetInputPhase(EnemyInputPhases.ENEMY_INPUT_PHASE_NROMAL_ATKED);
                    break;
            }

            base.OnATKCompleted();
        }

        protected override bool CheckUnableToMove()
        {
            bool flag = base.CheckUnableToMove();
            if (flag)
            {
                mRoleATkAI.ResetAIRoleATK();
            }
            return flag;
        }
    }
}
