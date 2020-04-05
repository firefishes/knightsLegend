using ShipDock.Pooling;

namespace KLGame
{

    public class NormalEnemyATKState : NormalATKState
    {
        public NormalEnemyATKState(int name) : base(name)
        {
        }

        public override bool HitCommit()
        {
            if (mStateParam == default)
            {
                return false;
            }
            mStateParam.FillValues();

            ProcessHit hit = Pooling<ProcessHit>.From();
            hit.Reinit(mRole);

            hit.AfterProcessing = OnATKHit;
            hit.HitInfoScope.validAngle = 120f;
            hit.HitInfoScope.minDistance = 2.5f;
            hit.HitInfoScope.startPos = mStateParam.StartPos;
            hit.HitInfoScope.startRotation = mStateParam.StartRotation;

            return mRole.Processing.AddProcess(hit);
        }
    }

}