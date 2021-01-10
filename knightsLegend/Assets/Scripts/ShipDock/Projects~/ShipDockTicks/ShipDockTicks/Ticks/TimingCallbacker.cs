using ShipDock.Interfaces;
using System;

namespace ShipDock.Ticks
{
    public class TimingCallbacker : IDispose
    {
        public float timing;
        public Action callback;

        public void Dispose()
        {
            callback = default;
        }
    }
}