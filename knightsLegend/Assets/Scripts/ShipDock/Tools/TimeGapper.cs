namespace ShipDock.Tools
{
    public struct TimeGapper
    {
        public bool isStart;
        public float time;
        public float totalTime;
        
        public void Start()
        {
            isStart = true;
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
    }

}