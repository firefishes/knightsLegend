using ShipDock.Applications;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class KLInputComponent : HostGameInputComponent<KLServer>
    {

        protected override void GetUserInput()
        {
            base.GetUserInput();

            //mInputV = Quaternion.Euler(mInputV) * mRoleItem.CameraForward;
        }

        protected override string[] RelateServerNames { get; } = new string[]
        {
            KLConsts.S_KL
        };

        protected override string MainServerName { get; } = KLConsts.S_KL;
    }

}