using ShipDock.Interfaces;
using ShipDock.Tools;
using System;

namespace ShipDock.Applications
{
    public class TicksLater : IDispose
    {
        private DoubleBuffers<Action<int>> mDoubleBuffer;

        public TicksLater() : base()
        {
            mDoubleBuffer = new DoubleBuffers<Action<int>>();
            mDoubleBuffer.OnDequeue += OnTicksLater;
        }

        public void Dispose()
        {
            mDoubleBuffer?.Dispose();
        }

        private void OnTicksLater(int time, Action<int> current)
        {
            current?.Invoke(time);
        }

        /// <summary>
        /// 添加在下一帧只需要执行一次的函数
        /// </summary>
        public void CallLater(Action<int> method)
        {
            mDoubleBuffer.Enqueue(method);
        }

        internal void Update(int time)
        {
            mDoubleBuffer.Update(time);
        }
    }

}
