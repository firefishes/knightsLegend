using ShipDock.Applications;
using ShipDock.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class KLRoleCameraComponent : ShipDockComponent
    {
        private float mTime;
        private ICommonRole mRole;

        //public override void Execute(int time, ref IShipDockEntitas target)
        //{
        //    base.Execute(time, ref target);

        //    mTime += time / 1000f;
        //    if(mTime >= Time.deltaTime)
        //    {
        //        mTime -= Time.deltaTime;

        //        mRole = target as ICommonRole;
        //        mRole.CameraForward = 
        //    }
        //}
    }
}
