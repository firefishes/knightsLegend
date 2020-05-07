using ShipDock.Applications;
using ShipDock.ECS;

namespace KLGame
{
    public class KLRoleTimesComponent : ShipDockComponent
    {
        private float mTimeScale = 0.001f;
        private bool mTimeEnd;
        private TimingTasker mTimeItem;
        private TimingTaskEntitas mEntitas;

        public override void Execute(int time, ref IShipDockEntitas target)
        {
            base.Execute(time, ref target);

            mEntitas = target as TimingTaskEntitas;
            mEntitas?.UpdateAllTimes(time * mTimeScale, ref mTimeItem);
        }
    }

}