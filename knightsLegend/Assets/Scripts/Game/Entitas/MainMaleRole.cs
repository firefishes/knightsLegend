using ShipDock.Applications;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class MainMaleRole : KLRole
    {
        public MainMaleRole()
        {
            IRoleData data = KLRoleData.GetRoleDataByRandom();
            data.ConfigID = 0;
            SetRoleData(data);
            
            IsUserControlling = true;
            PositionEnabled = false;

            Camp = 0;
        }
    }

    public class EnmeyRole : KLRole
    {
        public EnmeyRole()
        {
            IRoleData data = KLRoleData.GetRoleDataByRandom();
            data.ConfigID = 1;
            SetRoleData(data);

            IsUserControlling = false;
            PositionEnabled = true;

            Camp = 1;
        }
    }
}
