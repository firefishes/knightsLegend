using ShipDock.ECS;

namespace ShipDock.Applications
{
    public class PositionComponent : ShipDockComponent
    {
        protected ICommonRole mRole;

        private float mDistance;

        public override void Dispose()
        {
            base.Dispose();

            mRole = default;
        }

        private void CheckEnemyLockDown()
        {
            if (mRole.TargetTracking != default)
            {
                mDistance = mRole.GetDistFromMainLockDown();
                if (ShouldMove())
                {
                    if (!mRole.FindingPath)
                    {
                        StartMoving();
                    }
                }
                else
                {
                    PositionStoped();
                }
            }
            else
            {
                mRole.FindingPath = false;
            }
        }

        protected virtual void StartMoving()
        {
            mRole.FindingPath = true;
            mRole.SpeedCurrent = mRole.Speed;
            mRole.AfterGetStopDistChecked = false;
        }

        private bool PositionStoped()
        {
            bool result = mRole.FindingPath || !mRole.AfterGetStopDistChecked;
            if (result)
            {
                mRole.FindingPath = false;
                mRole.SpeedCurrent = 0;
                mRole.AfterGetStopDistChecked = mRole.AfterGetStopDistance(mDistance, mRole.Position);
            }
            return result;
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

        private bool ShouldMove()
        {
            return mDistance > mRole.GetStopDistance();
        }

        public bool IsEntitasStoped(ref ICommonRole target)
        {
            float stopDistance = target.GetStopDistance();
            float lockDownDistance = target.GetDistFromMainLockDown();

            return lockDownDistance <= stopDistance;
        }
    }

}
