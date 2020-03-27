using ShipDock.Applications;
using ShipDock.Tools;
using System.Collections.Generic;

namespace KLGame
{
    public class TimingTaskEntitas : EntitasComponentable
    {
        private KeyValueList<int, TimingTasker> mTaskMapper;

        public TimingTaskEntitas() : base()
        {
            InitComponents();

            mTaskMapper = new KeyValueList<int, TimingTasker>();

            AddTimingTask(RoleTimingTaskNames.NORMAL_ATK_TIME);
            AddTimingTask(RoleTimingTaskNames.NORMAL_ATK_HIT_TIME);
        }

        public void AddTimingTask(int name)
        {
            new TimingTasker(name, ref mTaskMapper);
        }

        public TimingTasker GetRoleTime(int name)
        {
            return mTaskMapper[name];
        }

        public void UpdateAllTimes(float dTime, ref List<TimingTasker> allRoleTimes, ref TimingTasker item)
        {
            if (mTaskMapper == default)
            {
                return;
            }
            bool flag;
            allRoleTimes = mTaskMapper.Values;
            int max = allRoleTimes.Count;
            for (int i = 0; i < max; i++)
            {
                item = allRoleTimes[i];
                if (item.IsStart)
                {
                    flag = item.timeGapper.TimeAdvanced(dTime);
                    if (flag)
                    {
                        item.completion?.Invoke();
                    }
                }
            }
        }

        protected override int[] ComponentIDs { get; } = new int[]
        {
            KLConsts.C_ROLE_TIMES
        };
    }
}