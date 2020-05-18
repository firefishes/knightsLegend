using ShipDock.Applications;
using ShipDock.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class KLPositionComponent : PositionComponent
    {
        public override void Execute(int time, ref IShipDockEntitas target)
        {
            base.Execute(time, ref target);
        }
    }

}