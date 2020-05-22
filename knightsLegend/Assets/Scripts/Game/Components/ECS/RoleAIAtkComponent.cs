﻿using ShipDock.Applications;
using ShipDock.ECS;

namespace KLGame
{
    public class RoleAIAtkComponent : ShipDockComponent//RoleInputPhasesComponent
    {
        private IAIRole mAIRole;
        private ICommonRole mRole;
        private PositionComponent mPositionComp;
        private TimingTasker mAtkTimingTask;

        public override void Init()
        {
            base.Init();

            //AddAllowCalled(KLConsts.ENEMY_INPUT_PHASE_AFTER_NROMAL_ATK, 1);
        }

        public override void Execute(int time, ref IShipDockEntitas target)
        {
            base.Execute(time, ref target);

            mRole = target as ICommonRole;
            mAIRole = target as IAIRole;
            //if (mRoleInput != default)// && !mAIRole.IsInitNormalATKPhases)
            //{
            //    InitRolePhases(mRoleInput);
            //}
            CheckAttackAI();
        }

        protected void InitRolePhases(IRoleInput roleInput)
        {
            //roleInput.AddEntitasCallback(KLConsts.ENEMY_INPUT_PHASE_UPDATE_NROMAL_ATK_TRIGGER_TIME, CheckAttackAI);
            roleInput.AddEntitasCallback(KLConsts.ENEMY_INPUT_PHASE_AFTER_NROMAL_ATK, AfterNormalATK);
        }

        private void AfterNormalATK()
        {
            if (mPositionComp.IsEntitasStoped(ref mRole))
            {
                //mRoleInput.SetInputPhase(KLConsts.ENEMY_INPUT_PHASE_ATTACK_AI);
                //mAIRole.RoleFSM.ChangeState(NormalRoleStateName.GROUNDED);
            }
            else
            {
                //mAIRole.ResetAIRoleATK();
            }
        }

        private void CheckAttackAI()
        {
            mAtkTimingTask = mAIRole.TimesEntitas.GetTimingTasker(KLConsts.T_AI_THINKING, KLConsts.T_AI_THINKING_TIME_TASK_ATK);
            if (mAtkTimingTask.IsStart)
            {
                return;
            }

            bool hasTrackedEnemy = mAIRole.EnemyTracking != default;
            bool isRoleStoped = mPositionComp.IsEntitasStoped(ref mRole);
            if (hasTrackedEnemy && isRoleStoped && mAIRole.ShouldAIThinking())
            {
                float time = mAIRole.AISensor.GetAtkThinkingTime();
                mAtkTimingTask.Start(time);
            //    if (mAIRole.ShouldAtkAIWork)
            //    {
            //        if (mAIRole.TimesEntitas.GetTimingTasker(KLConsts.T_AI_ATK_TIME, KLConsts.T_AI_ATK_TIME_TASK_THIKING).IsFinish)
            //        {
            //            mRoleInput.SetInputPhase(KLConsts.ENEMY_INPUT_PHASE_NROMAL_ATK);
            //        }
            //    }
            //    else
            //    {
            //        mRoleInput.SetInputPhase(UserInputPhases.ROLE_INPUT_PHASE_AFTER_MOVE);
            //    }
            //}
            //else
            //{
            //    mAIRole.ResetAIRoleATK();
            }
        }

        protected override void ReFillRelateComponents(int name, IShipDockComponent target, IShipDockComponentManager manager)
        {
            base.ReFillRelateComponents(name, target, manager);

            if (mPositionComp == default)
            {
                mPositionComp = GetRelatedComponent<PositionComponent>(KLConsts.C_POSITION);
            }
        }
    }

}