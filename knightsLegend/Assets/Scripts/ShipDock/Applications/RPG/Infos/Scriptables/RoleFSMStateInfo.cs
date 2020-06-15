using ShipDock.Infos;
using ShipDock.Interfaces;
using ShipDock.Tools;
using System;

namespace ShipDock.Applications
{
    [Serializable]
    public class RoleFSMStateInfo : IDispose
    {
        public int stateName;
        public NotificationInfo[] enterStateNotice;
        public NotificationInfo[] stateComboNotice;
        public NotificationInfo[] willFinishStateNotice;
        
        public void Dispose()
        {
            Utils.Reclaim(ref enterStateNotice);
            Utils.Reclaim(ref stateComboNotice);
            Utils.Reclaim(ref willFinishStateNotice);
        }
    }

}