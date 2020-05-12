using ShipDock.Infos;
using ShipDock.Tools;
using System;

namespace ShipDock.Applications
{
    [Serializable]
    public class RoleFSMStateInfo : SceneInfosMapper<int, RoleFSMStateExecuableInfo>
    {
        public int stateName;
        public NotificationInfo[] enterStateNotice;
        public NotificationInfo[] willFinishStateNotice;

        public override int GetInfoKey(ref RoleFSMStateExecuableInfo item)
        {
            return item.phaseName;
        }

        public override void Dispose()
        {
            m_DisposeInfos = true;

            base.Dispose();

            Utils.Reclaim(ref enterStateNotice);
        }
    }

}