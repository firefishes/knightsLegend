using ShipDock.Applications;
using ShipDock.ECS;
using ShipDock.Tools;
using System;
using System.Collections.Generic;

namespace KLGame
{
    public class KLRoleTimesComponent : ShipDockComponent
    {
        private float mTimeScale = 0.001f;
        private bool mTimeEnd;
        private TimingTasker mTimeItem;
        private List<TimingTasker> mRoleTimes;
        private TimingTaskEntitas mEntitas;

        public override void Execute(int time, ref IShipDockEntitas target)
        {
            base.Execute(time, ref target);

            mEntitas = target as TimingTaskEntitas;
            mEntitas.UpdateAllTimes(time * mTimeScale, ref mRoleTimes, ref mTimeItem);
        }
    }

}