using ShipDock.Applications;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class KLInputComponent : HostGameInputComponent<KLServer>
    {

        protected override string[] RelateServerNames { get; } = new string[]
        {
            KLConsts.S_KL
        };

        protected override string MainServerName { get; } = KLConsts.S_KL;
    }

}