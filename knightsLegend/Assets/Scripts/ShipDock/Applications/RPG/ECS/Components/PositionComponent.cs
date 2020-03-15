using ShipDock.ECS;

namespace ShipDock.Applications
{
    public class PositionComponent : ShipDockComponent
    {
        private float mDistance;
        private ICommonRole mRole;

        public override void Execute(int time, ref IShipDockEntitas target)
        {
            base.Execute(time, ref target);

            mRole = target as ICommonRole;

            if (mRole.PositionEnabled)
            {
                if (mRole.EnemyMainLockDown != default)
                {
                    mDistance = mRole.GetDistFromMainLockDown();
                    if (ShouldStop())
                    {
                        mRole.FindngPath = false;
                        mRole.SpeedCurrent = 0;
                        mRole.AfterGetStopDistance(mDistance, mRole.Position);
                    }
                    else if (ShouldMove())
                    {
                        mRole.FindngPath = true;
                        mRole.SpeedCurrent = mRole.Speed;
                    }
                    else
                    {
                        mRole.FindngPath = false;
                        mRole.SpeedCurrent = 0;
                    }
                }
                else
                {
                    mRole.FindngPath = false;
                }
            }
        }

        private float GetStopDistance()
        {
            return mRole.GetStopDistance();
        }

        private float GetTraceDistance()
        {
            return 3f;
        }

        private bool ShouldMove()
        {
            return mDistance > GetTraceDistance();
        }

        private bool ShouldStop()
        {
            return mDistance <= GetStopDistance();
        }

        public override void Dispose()
        {
            base.Dispose();

            mRole = default;
        }
    }

}
