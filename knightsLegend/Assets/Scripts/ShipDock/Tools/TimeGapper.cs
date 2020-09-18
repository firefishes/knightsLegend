namespace ShipDock.Tools
{
    public struct TimeGapper
    {
        public bool isStart;
        public float time;
        public float totalTime;
        
        public void Start(float totalTimeValue = 0f)
        {
            isStart = true;
            if(totalTimeValue > 0f)
            {
                totalTime = totalTimeValue;
            }
            time = totalTime;
        }

        public void Stop()
        {
            isStart = false;
        }

        public bool TimeAdvanced(float dTime)
        {
            time -= dTime;
            bool result = time <= 0f;
            if (result)
            {
                Stop();
                time += totalTime;
            }
            return result;
        }

        public float TimeProgress()
        {
            return 1f - (time / totalTime);
        }
    }

}