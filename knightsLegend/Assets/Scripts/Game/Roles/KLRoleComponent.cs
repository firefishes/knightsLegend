using ShipDock.Applications;
using ShipDock.Notices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class KLRoleComponent : RoleComponent
    {
        protected override void InitRoleData()
        {
        }

        protected override void SetRoleEntitas()
        {
            mRole = new MainMaleRole();
        }

        protected override void OnRoleNotices(INoticeBase<int> obj)
        {
        }

    }
}
