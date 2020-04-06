using ShipDock.Applications;
using ShipDock.ECS;

namespace KLGame
{
    public class RoleAIAtkComponent : RoleInputPhasesComponent
    {
        private IAIRole mAIRole;
        private PositionComponent mPositionComp;

        public override void Init()
        {
            base.Init();

            AddAllowCalled(EnemyInputPhases.ENEMY_INPUT_PHASE_AFTER_NROMAL_ATK, 0);
        }

        protected override void InitRolePhases(IRoleInput roleInput)
        {
            mAIRole.IsInitNormalATKPhases = true;
            roleInput.AddEntitasCallback(EnemyInputPhases.ENEMY_INPUT_PHASE_UPDATE_NROMAL_ATK_TRIGGER_TIME, UpdateNormalATKTriggerTime);
            roleInput.AddEntitasCallback(EnemyInputPhases.ENEMY_INPUT_PHASE_AFTER_NROMAL_ATK, AfterNormalATK);
        }

        private void AfterNormalATK()
        {
            if (mPositionComp.IsEntitasStoped(ref mRole))
            {
                mRoleInput.SetInputPhase(EnemyInputPhases.ENEMY_INPUT_PHASE_ATTACK_AI);
            }
            else
            {
                mAIRole.ResetAIRoleATK();
            }
        }

        private void UpdateNormalATKTriggerTime()
        {
            if (mPositionComp.IsEntitasStoped(ref mRole))
            {
                if (mAIRole.ShouldAtkAIWork)
                {
                    if (!mAIRole.TimesEntitas.GetRoleTiming(RoleTimingTaskNames.NORMAL_ATK_TIME).IsStart)
                    {
                        mRoleInput.SetInputPhase(EnemyInputPhases.ENEMY_INPUT_PHASE_NROMAL_ATK);
                    }
                }
                else
                {
                    mRoleInput.SetInputPhase(UserInputPhases.ROLE_INPUT_PHASE_AFTER_MOVE);
                }
            }
            else
            {
                mAIRole.ResetAIRoleATK();
            }
        }

        public override void Execute(int time, ref IShipDockEntitas target)
        {
            if (mPositionComp == default)
            {
                mPositionComp = ShipDockApp.Instance.Components.GetComponentByAID(KLConsts.C_POSITION) as PositionComponent;
            }
            
            mAIRole = target as IAIRole;
            
            base.Execute(time, ref target);

            if(!mAIRole.IsInitNormalATKPhases)
            {
                InitRolePhases(mRoleInput);
            }

        }
    }

}