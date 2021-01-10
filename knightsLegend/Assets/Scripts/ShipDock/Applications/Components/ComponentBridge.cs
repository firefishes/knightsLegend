using ShipDock.Interfaces;
using ShipDock.Notices;
using System;

namespace ShipDock.Applications
{
    public class ComponentBridge : IDispose
    {
        private Action mOnStarted;

        public void Dispose()
        {
            mOnStarted = default;
        }

        public ComponentBridge(Action callback = null)
        {
            mOnStarted = callback;
        }

        public void Start()
        {
            Framework.Instance.AddStart(OnAppStart);
        }

        private void OnAppStart()
        {
            Server.Servers ioc = Framework.Instance.GetUnit<Server.Servers>(Framework.UNIT_IOC);
            ioc.AddOnServerFinished(mOnStarted);
        }
    }

}
