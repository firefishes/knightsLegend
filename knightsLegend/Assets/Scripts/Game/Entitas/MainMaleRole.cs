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
    }
}
