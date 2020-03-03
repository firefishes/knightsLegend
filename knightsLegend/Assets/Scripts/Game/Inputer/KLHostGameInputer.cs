using ShipDock.Applications;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class KLHostGameInputer : HostGameInputer
    {
        protected override int[] RelatedComponentNames { get; } = new int[]
        {
            KLConsts.C_ROLE_INPUT
        };
    }
}
