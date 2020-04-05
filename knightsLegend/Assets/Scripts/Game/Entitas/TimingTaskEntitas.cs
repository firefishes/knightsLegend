using ShipDock.Applications;
using ShipDock.Pooling;
using ShipDock.Tools;
using System.Collections.Generic;

namespace KLGame
{
    public class TimingTaskEntitas : EntitasComponentable, IPoolable
    {
        private List<TimingTasker> mTaskers;
        private KeyValueList<int, TimingTasker> mTaskMapper;
        private KeyValueList<int, TimingTasker> mForceMoveTaskMapper;

        public TimingTaskEntitas() : base()
        {
            InitComponents();

            mTaskMapper = new KeyValueList<int, TimingTasker>();
            mForceMoveTaskMapper = new KeyValueList<int, TimingTasker>();

            AddRoleTiming(RoleTimingTaskNames.NORMAL_ATK_TIME);
            AddRoleTiming(RoleTimingTaskNames.NORMAL_ATK_HIT_TIME);
        }

        public override void Dispose()
        {
            base.Dispose();

            mTaskers = default;
            Utils.Reclaim(ref mTaskMapper, true, true);
            Utils.Reclaim(ref mForceMoveTaskMapper, true, true);
        }
        
        public void Revert()
        {
            Utils.Reclaim(ref mTaskMapper, false, true);
            Utils.Reclaim(ref mForceMoveTaskMapper, false, true);
        }

        public TimingTasker AddRoleTiming(int name)
        {
            return new TimingTasker(name, ref mTaskMapper);
        }

        public TimingTasker AddForceMoveTiming(int name)
        {
            return new TimingTasker(name, ref mForceMoveTaskMapper);
        }

        public void RemoveRoleTiming(int name, bool isFinish = false, bool isDispose = false)
        {
            RemvoeTiming(ref mTaskMapper, name, isFinish, isDispose);
        }

        public void RemoveForceMoveTiming(int name, bool isFinish = false, bool isDispose = false)
        {
            RemvoeTiming(ref mForceMoveTaskMapper, name, isFinish, isDispose);
        }

        private void RemvoeTiming(ref KeyValueList<int, TimingTasker> mapper, int name, bool isFinish = false, bool isDispose = false)
        {
            TimingTasker target = mapper.Remove(name);
            if (target == default)
            {
                return;
            }
            if (isFinish)
            {
                target.completion?.Invoke();
            }
            if (isDispose)
            {
                target?.Dispose();
            }
        }

        public TimingTasker GetRoleTiming(int name)
        {
            return mTaskMapper[name];
        }

        public void UpdateAllTimes(float dTime, ref TimingTasker item)
        {
            if(WillDestroy || TaskersUpdating)
            {
                return;
            }
            TaskersUpdating = true;

            mTaskers = (mTaskMapper != default) ? mTaskMapper.Values : default;
            UpdateTaskers(ref mTaskers, dTime, ref item);

            mTaskers = (mForceMoveTaskMapper != default) ? mForceMoveTaskMapper.Values : default;
            UpdateTaskers(ref mTaskers, dTime, ref item);

            TaskersUpdating = false;
        }

        private void UpdateTaskers(ref List<TimingTasker> taskers, float dTime, ref TimingTasker item)
        {
            if(taskers == default)
            {
                return;
            }
            bool flag;
            int max = taskers.Count;
            for (int i = 0; i < max; i++)
            {
                item = taskers[i];
                if (item != default && item.IsStart)
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

        private bool TaskersUpdating { get; set; }
    }
}