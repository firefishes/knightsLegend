using System;

namespace ShipDock.Tools
{
    [Serializable]
    public struct TimeGapper
    {
        public bool isStart;
        public bool isAnew;
        public float time;
        public float totalTime;

        private float mPrevEnd;

        public float Progress
        {
            get
            {
                return time / totalTime;
            }
        }

        public bool IsFinised { get; private set; }

        public void Start(float totalTimeValue = 0f)
        {
            isStart = true;
            IsFinised = false;

            if (totalTimeValue > 0f)
            {
                totalTime = totalTimeValue;
            }
            else { }

            time = isAnew ? 0f : mPrevEnd;
        }

        public void Stop()
        {
            isStart = false;
            IsFinised = true;
        }

        public void ContinueTimer()
        {
            if (Progress > 0f)
            {
                isStart = true;
            }
            else
            {
                Start();
            }
        }

        public bool TimeAdvanced(float dTime)
        {
            if (!isStart)
            {
                return true;
            }
            else { }

            time += dTime;
            bool result = time >= totalTime;
            if (result)
            {
                Stop();
                if (isAnew)
                {
                    time = 0f;
                }
                else
                {
                    time -= totalTime;
                }
                mPrevEnd = time;
            }
            else { }

            return result;
        }

        public void Reset()
        {
            time = isAnew ? 0f : mPrevEnd;
            IsFinised = false;
        }
    }

}