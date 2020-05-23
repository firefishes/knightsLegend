using ShipDock.Notices;
using ShipDock.Pooling;

namespace KLGame
{
    public class TimingTaskNotice : Notice
    {
        public void ReinitForStart(int timingName, int mapperIndex, float time)
        {
            TimingName = timingName;
            MapperIndex = mapperIndex;
            Time = time;
            IsStart = true;
        }

        public void ReinitForStop(int timingName, int mapperIndex, bool onlyChangeState = false)
        {
            TimingName = timingName;
            MapperIndex = mapperIndex;
            OnlyChangeState = onlyChangeState;
            IsStop = true;
        }

        public void ReinitForReset(int timingName, int mapperIndex)
        {
            TimingName = timingName;
            MapperIndex = mapperIndex;
            IsReset = true;
        }

        public override void Revert()
        {
            base.Revert();

            Time = 0f;
            OnlyChangeState = false;
            IsStart = false;
            IsStop = false;
            IsReset = false;
        }

        public override void ToPool()
        {
            Pooling<TimingTaskNotice>.To(this);
        }

        public int TimingName { get; private set; }
        public int MapperIndex { get; private set; }
        public float Time { get; private set; }
        public bool OnlyChangeState { get; private set; } = false;
        public bool IsStart { get; private set; }
        public bool IsReset { get; private set; }
        public bool IsStop { get; private set; }
    }
}