using ShipDock.Applications;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class KLRole : RoleEntitas
    {

        protected override int[] ComponentIDs { get; } = new int[]
        {
            KLConsts.C_ROLE_INPUT,
            KLConsts.C_POSITION,
            KLConsts.C_ROLE_COLLIDER,
            KLConsts.C_ROLE_MUST
        };
    }
}
