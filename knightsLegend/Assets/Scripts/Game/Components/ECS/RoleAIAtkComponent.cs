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

            AddAllowCalled(KLConsts.ENEMY_INPUT_PHASE_AFTER_NROMAL_ATK, 1);
        }

        protected override void InitRolePhases(IRoleInput roleInput)
        {
            mAIRole.IsInitNormalATKPhases = true;
            roleInput.AddEntitasCallback(KLConsts.ENEMY_INPUT_PHASE_UPDATE_NROMAL_ATK_TRIGGER_TIME, UpdateNormalATKTriggerTime);
            roleInput.AddEntitasCallback(KLConsts.ENEMY_INPUT_PHASE_AFTER_NROMAL_ATK, AfterNormalATK);
        }

        private void AfterNormalATK()
        {
            if (mPositionComp.IsEntitasStoped(ref mRole))
            {
                mRoleInput.SetInputPhase(KLConsts.ENEMY_INPUT_PHASE_ATTACK_AI);
                //mAIRole.RoleFSM.ChangeState(NormalRoleStateName.GROUNDED);
            }
            else
            {
                mAIRole.ResetAIRoleATK();
            }
        }

        private void UpdateNormalATKTriggerTime()
        {
            //if (mPositionComp.IsEntitasStoped(ref mRole))
            //{
            //    if (mAIRole.ShouldAtkAIWork)
            //    {
            //        if (mAIRole.TimesEntitas.GetTimingTasker(KLConsts.T_AI_ATK_TIME, 0).IsFinish)
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
            //}
        }

        public override void Execute(int time, ref IShipDockEntitas target)
        {
            if (mPositionComp == default)
            {
                mPositionComp = ShipDockApp.Instance.Components.GetComponentByAID(KLConsts.C_POSITION) as PositionComponent;
            }
            
            mAIRole = target as IAIRole;
            
            base.Execute(time, ref target);

            if(mRoleInput != default && !mAIRole.IsInitNormalATKPhases)
            {
                InitRolePhases(mRoleInput);
            }

        }
    }

}