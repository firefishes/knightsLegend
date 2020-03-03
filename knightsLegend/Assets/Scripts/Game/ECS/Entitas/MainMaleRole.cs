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
        }
    }
}
