using ShipDock.Applications;
using ShipDock.ECS;
using ShipDock.Pooling;
using ShipDock.Tools;

namespace KLGame
{
    public class RoleAIAtkComponent : ShipDockComponent//RoleInputPhasesComponent
    {
        private IAIRole mAIRole;
        private IAIBrain mBrain;
        private ICommonRole mRole;
        private PositionComponent mPositionComp;
        private TimingTasker mAtkTimingTask;
        private KeyValueList<int, IAIBrain> mAIBrains;

        public override void Init()
        {
            base.Init();

            mAIBrains = new KeyValueList<int, IAIBrain>();
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
            if (mAIRole.WillDestroy)
            {
                mAIBrains.Remove(mAIRole.ID);
            }
            else if (!mAIBrains.ContainsKey(mAIRole.ID))
            {
                mAIBrains.Put(mAIRole.ID, mAIRole.AIBrain);
            }

            mBrain = mAIRole.AIBrain;

            if (mAIRole.AfterGetStopDistChecked)
            {
                TimingTaskNotice notice = Pooling<TimingTaskNotice>.From();
                notice.ReinitForStart(KLConsts.T_AI_THINKING, KLConsts.T_AI_THINKING_TIME_TASK_AI_WAITED, mAIRole.AISensor.GetDecisionTime());
            }

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
            if(mAIRole.ConductTimingTask == int.MaxValue)
            {
                return;
            }
            mAtkTimingTask = mAIRole.TimesEntitas.GetTimingTasker(KLConsts.T_AI_THINKING, mAIRole.ConductTimingTask);// KLConsts.T_AI_THINKING_TIME_TASK_ATK);
            if (!mAtkTimingTask.ShouldRun())
            {
                return;
            }

            bool hasTrackedEnemy = mAIRole.EnemyTracking != default;
            bool isRoleStoped = mPositionComp.IsEntitasStoped(ref mRole);
            if (hasTrackedEnemy && isRoleStoped && mAIRole.ShouldAIThinking())
            {
                float time = mAIRole.AISensor.GetAtkThinkingTime();
                mAtkTimingTask.Start(time);
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