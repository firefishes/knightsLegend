﻿using System;

namespace ShipDock.Tools
{
    [Serializable]
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

        public void ContinueTimer()
        {
            if (TimeProgress() > 0f)
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