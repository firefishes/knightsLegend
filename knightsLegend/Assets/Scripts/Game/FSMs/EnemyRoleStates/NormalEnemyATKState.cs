using System;
using ShipDock.Pooling;

namespace KLGame
{

    public class NormalEnemyATKState : NormalATKState
    {
        public NormalEnemyATKState(int name) : base(name)
        {
        }

        protected override bool ShouldEnter(ref NormalATKStateParam param)
        {
            bool result = param.KLRole.FindingPath;
            if(result)
            {
                param.ToPool();
                GetFSM().ChangeState(NormalRoleStateName.GROUNDED);
            }
            return !result && base.ShouldEnter(ref param);
        }

        public override bool HitCommit(int hitCollidID)
        {
            if (mStateParam == default)
            {
                return false;
            }
            
            mStateParam.FillValues();

            if(!ShouldCheckHit())
            {
                return false;
            }

            mHit = Pooling<ProcessHit>.From();
            ProcessHit hit = mHit as ProcessHit;
            hit.Reinit(mRole);

            hit.HitColliderID = hitCollidID;
            hit.AfterProcessing = OnATKHit;
            hit.HitInfoScope.validAngle = 120f;
            hit.HitInfoScope.minDistance = 2.5f;
            hit.HitInfoScope.startPos = mStateParam.StartPos;
            hit.HitInfoScope.startRotation = mStateParam.StartRotation;

            return mRole.Processing.AddRoleProcess(hit);
        }

        protected override bool CheckBeforeFinish()
        {
            //TODO add config notice 1003
            mRole.RoleInput.SetInputPhase(KLConsts.ENEMY_INPUT_PHASE_AFTER_NROMAL_ATK);//TODO 此处不需要了
            StopAtkTiming();
            mRole.AfterGetStopDistChecked = false;

            return true;
        }

        private void StopAtkTiming()
        {
            TimingTaskNotice notice = Pooling<TimingTaskNotice>.From();
            notice.ReinitForReset(KLConsts.T_AI_THINKING, KLConsts.T_AI_THINKING_TIME_TASK_ATK);
            mRole.Dispatch(KLConsts.N_ROLE_TIMING, notice);
            notice.ToPool();
        }
    }

}