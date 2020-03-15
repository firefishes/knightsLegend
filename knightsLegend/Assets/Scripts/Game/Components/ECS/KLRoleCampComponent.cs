using ShipDock.Applications;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{

    public class KLRoleCampComponent : RoleCampComponent
    {
        public override string DataServerName { get; } = KLConsts.S_DATAS;
        public override string AddCampRoleResovlerName { get; } = "AddCampRole";
        public override string CampRoleCreatedAlias { get; } = "CampRoleCreated";
    }

}