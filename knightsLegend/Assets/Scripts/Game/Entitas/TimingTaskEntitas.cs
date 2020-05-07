using ShipDock.Applications;
using ShipDock.Pooling;
using ShipDock.Tools;
using System.Collections.Generic;
using UnityEngine;
using TimingMapper = ShipDock.Tools.KeyValueList<int, ShipDock.Applications.TimingTasker>;

namespace KLGame
{
    public class TimingTaskEntitas : EntitasComponentable, IPoolable
    {
        public static TimingTaskEntitas Create()
        {
            return Pooling<TimingTaskEntitas>.From();
        }

        private List<TimingTasker> mTaskers;
        private TimingMapper mMapperItem;
        private List<TimingMapper> mMappers;

        public TimingTaskEntitas() : base()
        {
            InitComponents();

            mMappers = new List<TimingMapper>();
            CreateMapper();
        }

        public override void Dispose()
        {
            base.Dispose();

            mTaskers = default;
            Utils.Reclaim(ref mMappers, true, true);
        }
        
        public void Revert()
        {
            Utils.Reclaim(ref mMappers, false, true);
        }

        public void ToPool()
        {
            Pooling<TimingTaskEntitas>.To(this);
        }

        public int CreateMapper()
        {
            mMappers.Add(new KeyValueList<int, TimingTasker>());
            return mMappers.Count;
        }

        public TimingTasker AddTiming(int name, int mapperIndex)
        {
            TimingMapper mapper = mMappers[mapperIndex];
            return new TimingTasker(name, ref mapper);
        }

        public void RemoveTiming(int name, int mapperIndex, bool isFinish = false, bool isDispose = false)
        {
            TimingMapper mapper = mMappers[mapperIndex];
            RemvoeTiming(ref mapper, name, isFinish, isDispose);
        }

        private void RemvoeTiming(ref TimingMapper mapper, int name, bool isFinish = false, bool isDispose = false)
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

        public TimingTasker GetTimingTasker(int name, int mapperIndex)
        {
            TimingMapper mapper = mMappers[mapperIndex];
            return mapper[name];
        }

        public void UpdateAllTimes(float dTime, ref TimingTasker item)
        {
            if(WillDestroy || TaskersUpdating)
            {
                return;
            }
            TaskersUpdating = true;

            int max = mMappers.Count;
            for (int i = 0; i < max; i++)
            {
                mMapperItem = mMappers[i];
                mTaskers = mMapperItem.Values;
                UpdateTaskers(ref mTaskers, dTime, ref item);
            }

            TaskersUpdating = false;
        }

        private void UpdateTaskers(ref List<TimingTasker> taskers, float dTime, ref TimingTasker item)
        {
            if(taskers == default)
            {
                return;
            }
            bool flag;
            TimeGapper timer;
            int max = taskers.Count;
            for (int i = 0; i < max; i++)
            {
                item = taskers[i];
                if (item != default && item.IsStart)
                {
                    timer = item.timeGapper;
                    flag = timer.TimeAdvanced(dTime);
                    item.timeGapper = timer;
                    if (!item.IsStart)
                    {
                        item.Stop(true);
                    }
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