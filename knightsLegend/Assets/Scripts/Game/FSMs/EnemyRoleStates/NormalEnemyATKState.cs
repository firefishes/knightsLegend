using ShipDock.Applications;
using ShipDock.Pooling;

namespace KLGame
{

    public class NormalEnemyATKState : NormalATKState
    {
        public NormalEnemyATKState(int name) : base(name)
        {
        }

        public override bool HitCommit(int hitCollidID)
        {
            if (mStateParam == default)
            {
                return false;
            }
            
            mStateParam.FillValues();

            mHit = Pooling<ProcessHit>.From();
            ProcessHit hit = mHit as ProcessHit;
            hit.Reinit(mRole);

            hit.HitColliderID = hitCollidID;
            hit.AfterProcessing = OnATKHit;
            hit.HitInfoScope.validAngle = 120f;
            hit.HitInfoScope.minDistance = 2.5f;
            hit.HitInfoScope.startPos = mStateParam.StartPos;
            hit.HitInfoScope.startRotation = mStateParam.StartRotation;

            return mRole.Processing.AddProcess(hit);
        }

        protected override bool BeforeFinish(bool checkInputWhenFinish)
        {
            bool flag = base.BeforeFinish(checkInputWhenFinish);
            if (flag)
            {
                mRole.RoleInput.SetInputPhase(EnemyInputPhases.ENEMY_INPUT_PHASE_AFTER_NROMAL_ATK);
            }
            return flag;
        }
    }

}