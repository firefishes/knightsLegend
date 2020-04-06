using System;
using System.Collections.Generic;
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
            
            SetRoleInputCallback(EnemyInputPhases.ENEMY_INPUT_PHASE_ATTACK_AI, OnAttackIAStart);
            SetRoleInputCallback(EnemyInputPhases.ENEMY_INPUT_PHASE_NROMAL_ATK, OnEnemyNormalATK);
            SetRoleInputCallback(EnemyInputPhases.ENEMY_INPUT_PHASE_UPDATE_NROMAL_ATK_TRIGGER_TIME, OnUpdateNormalAtkTriggerTime);
        }

        private void OnUpdateNormalAtkTriggerTime()
        {
            m_RoleAnimator.SetFloat(m_BlendTreeInfo.MoveMotionName, 0f);
            m_RoleAnimator.SetFloat(m_BlendTreeInfo.TurnMotionName, 0f);
        }

        private void OnEnemyNormalATK()
        {
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

        private void OnAttackIAStart()
        {
            if(BlockingToAIStates.IndexOf(RoleFSM.Current.StateName) >= 0)
            {
                return;
            }

            KLRoleFSMAIStateParam param = Pooling<KLRoleFSMAIStateParam>.From();
            param.Reinit(this);

            RoleFSM.ChangeState(NormalRoleStateName.NORMAL_ATTACK_AI, param);
        }

        public override void OnATKCompleted()
        {
            switch(CurrentSkillID)
            {
                case 1:
                    mRoleInput.SetInputPhase(EnemyInputPhases.ENEMY_INPUT_PHASE_AFTER_NROMAL_ATK);
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

        protected virtual List<int> BlockingToAIStates { get; } = new List<int>
        {
            NormalRoleStateName.NORMAL_ATTACK_AI,
            NormalRoleStateName.NORMAL_ATK
        };
    }
}
