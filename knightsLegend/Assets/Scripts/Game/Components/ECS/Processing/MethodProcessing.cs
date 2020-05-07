using ShipDock.Applications;
using ShipDock.Pooling;
using System;

namespace KLGame
{
    public class MethodProcessing : IProcessing
    {

        public void Reinit(Action<IProcessing> method)
        {
            Method = method;
        }

        public void Dispose()
        {
        }

        public void Revert()
        {
            Method = default;
            Finished = false;
        }

        public void ToPool()
        {
            Pooling<MethodProcessing>.To(this);
        }

        public void OnProcessing()
        {
            ProcessingReady();
            Method?.Invoke(this);
        }

        public void ProcessingReady()
        {
        }

        private Action<IProcessing> Method { get; set; }

        public bool Finished { get; set; }
        public int Type { get; }
        public Action AfterProcessing { get; set; }

    }
}
