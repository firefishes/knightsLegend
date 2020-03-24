using System;
using ShipDock.Applications;

namespace KLGame
{
    public class MainMaleRole : KLRole
    {
        public MainMaleRole()
        {
            IRoleData data = KLRoleData.GetRoleDataByRandom();
            data.Speed = 18f;
            data.ConfigID = 0;
            SetRoleData(data);
            
            IsUserControlling = true;
            PositionEnabled = false;

            Camp = 0;
        }

        public override void InitComponents()
        {
            base.InitComponents();

            TimesEntitas.AddTimingTask(RoleTimingTaskNames.ENMEY_SCAN_TIME);

            StartTimingTask(RoleTimingTaskNames.ENMEY_SCAN_TIME, 0.3f, OnScanEnemy);
        }

        private void OnScanEnemy()
        {

        }
    }
}
