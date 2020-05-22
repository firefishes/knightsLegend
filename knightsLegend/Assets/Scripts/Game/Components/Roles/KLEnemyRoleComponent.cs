using System;
using System.Collections.Generic;
using ShipDock.Applications;
using ShipDock.Notices;
using ShipDock.Pooling;

namespace KLGame
{
    public class KLEnemyRoleComponent : KLRoleComponent
    {

        //private MethodUpdater mKeepStandingUpdater;
        private IAIRole mRoleATkAI;

        protected override void Init()
        {
            base.Init();

            //mKeepStandingUpdater = new MethodUpdater
            //{
            //    Update = KeepStanding
            //};
        }

        //private void KeepStanding(int dTime)
        //{
        //    m_RoleAnimator.SetFloat(m_BlendTreeInfo.MoveMotionName, 0f);
        //    m_RoleAnimator.SetFloat(m_BlendTreeInfo.TurnMotionName, 0f);
        //}

        protected override void SetRoleEntitas()
        {
            mRole = new EnmeyRole();
            mRoleATkAI = mRole as IAIRole;
        }

        protected override void UpdateAnimatorParams()
        {
            base.UpdateAnimatorParams();
            
            if (mRoleATkAI != default)
            {
                IAnticipathioner anticipathioner = mRoleATkAI.Anticipathioner;
                AIStateWill stateWill = anticipathioner.AIStateWillChange;
                if (stateWill != default)
                {
                    CurrentSkillID = stateWill.SkillID;
                    stateWill.RoleFSMParam?.Reinit(this, stateWill.Inputs);
                    RoleFSM.ChangeState(stateWill.StateWill, stateWill.RoleFSMParam);
                    stateWill.ToPool();
                    anticipathioner.AIStateWillChange = default;
                }
                anticipathioner.IsExecuted = false;
            }
        }

        protected override void InitRoleInputCallbacks()
        {
            base.InitRoleInputCallbacks();
            
            SetRoleInputCallback(KLConsts.ENEMY_INPUT_PHASE_ATTACK_AI, OnAttackIAStart);
            //SetRoleInputCallback(KLConsts.ENEMY_INPUT_PHASE_UPDATE_NROMAL_ATK_TRIGGER_TIME, OnUpdateNormalAtkTriggerTime);
            //SetRoleInputCallback(KLConsts.ENEMY_INPUT_PHASE_NROMAL_ATK, OnEnemyNormalATK);

        }

        //private void OnUpdateNormalAtkTriggerTime()
        //{
        //    mRoleATkAI.RoleInput.SetInputPhase(KLConsts.ENEMY_INPUT_PHASE_NROMAL_ATK);
        //}

        //private void OnEnemyNormalATK()
        //{
            //ActiveRoleInputPhase(KLConsts.ENEMY_INPUT_PHASE_NROMAL_ATK, false);

            //CurrentSkillID = 1;
            //MoveBlock = true;

            //NormalATKStateParam param = Pooling<NormalATKStateParam>.From();
            //param.Reinit(this, 1);
            //RoleFSM.ChangeState(NormalRoleStateName.NORMAL_ATK, param);

            //UpdaterNotice.AddSceneUpdater(mKeepStandingUpdater);
        //}

        private void OnAttackIAStart()
        {
            if(BlockingToAIStates.IndexOf(RoleFSM.Current.StateName) >= 0)
            {
                return;
            }

            ActiveRoleInputPhase(KLConsts.ENEMY_INPUT_PHASE_ATTACK_AI, false);

            KLRoleFSMAIStateParam param = Pooling<KLRoleFSMAIStateParam>.From();
            param.Reinit(this);
            RoleFSM.ChangeState(NormalRoleStateName.NORMAL_ATTACK_AI, param);
        }

        public override void OnATKCompleted()
        {
            switch(CurrentSkillID)
            {
                case 1:
                    mRoleInput.SetInputPhase(KLConsts.ENEMY_INPUT_PHASE_AFTER_NROMAL_ATK);
                    break;
            }

            base.OnATKCompleted();
        }

        protected override bool CheckUnableToMove()
        {
            bool flag = base.CheckUnableToMove();
            if (flag)
            {
                //mRoleATkAI.ResetAIRoleATK();
            }
            return flag;
        }

        protected override void OnRoleNotices(INoticeBase<int> obj)
        {
            base.OnRoleNotices(obj);

            switch (obj.Name)
            {
                case KLConsts.N_AI_RESET:
                    //UpdaterNotice.RemoveSceneUpdater(mKeepStandingUpdater);
                    ActiveRoleInputPhase(KLConsts.ENEMY_INPUT_PHASE_ATTACK_AI, true);
                    //ActiveRoleInputPhase(KLConsts.ENEMY_INPUT_PHASE_NROMAL_ATK, true);
                    break;
            }
        }

        protected virtual List<int> BlockingToAIStates { get; } = new List<int>
        {
            NormalRoleStateName.NORMAL_ATTACK_AI,
            NormalRoleStateName.NORMAL_ATK
        };
    }
}
