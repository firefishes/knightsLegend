using ShipDock.ECS;

namespace ShipDock.Applications
{
    public class PositionComponent : ShipDockComponent
    {
        private float mDistance;
        private ICommonRole mRole;

        public override void Dispose()
        {
            base.Dispose();

            mRole = default;
        }

        private void CheckEnemyLockDown()
        {
            if (mRole.EnemyTracking != default)
            {
                CheckPosistionInLockDown();
            }
            else
            {
                mRole.FindingPath = false;
            }
        }

        private void CheckPosistionInLockDown()
        {
            mDistance = mRole.GetDistFromMainLockDown();
            if(ShouldMove())
            {
                mRole.FindingPath = true;
                mRole.SpeedCurrent = mRole.Speed;
            }
            else
            {
                PositionStoped();
            }
        }

        private void PositionStoped()
        {
            if(mRole.FindingPath || !mRole.AfterGetStopDistChecked)
            {
                mRole.FindingPath = false;
                mRole.SpeedCurrent = 0;
                mRole.AfterGetStopDistChecked = mRole.AfterGetStopDistance(mDistance, mRole.Position);
            }
        }

        public override void Execute(int time, ref IShipDockEntitas target)
        {
            base.Execute(time, ref target);
            
            mRole = target as ICommonRole;

            if (mRole != default && mRole.PositionEnabled)
            {
                CheckEnemyLockDown();
            }
        }

        private float GetStopDistance(ref ICommonRole role)
        {
            return role.GetStopDistance();
        }

        private float GetTraceDistance()
        {
            return mRole.GetStopDistance();
        }

        private bool ShouldMove()
        {
            return mDistance > GetTraceDistance();
        }

        private bool ShouldStop(ref ICommonRole role)
        {
            return mDistance <= GetStopDistance(ref role);
        }

        public bool IsEntitasStoped(ref ICommonRole target)
        {
            float distance = target.GetDistFromMainLockDown();
            return distance <= GetStopDistance(ref target);
        }
    }

}
