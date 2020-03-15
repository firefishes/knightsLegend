using ShipDock.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class RoleAIAtkComponent : ShipDockComponent
    {
        private IAIRole mRole;
        private float mNormalATKTime;

        public override void Execute(int time, ref IShipDockEntitas target)
        {
            base.Execute(time, ref target);

            mRole = target as IAIRole;
            
            if(mRole.ShouldAtkAIWork && !mRole.InATKCycle)
            {
                if(mRole.NormalATKTriggerTime > 0f)
                {
                    mNormalATKTime = mRole.NormalATKTriggerTime;
                    mNormalATKTime -= time * 0.001f;
                    if(mNormalATKTime <= 0f && mRole.ATKID != 3)
                    {
                        mNormalATKTime = 0f;
                        mRole.SetATKID(2);
                    }
                    mRole.SetNormalATKTriggerTime(mNormalATKTime);
                }
                else
                {
                    mRole.SetATKID(1);
                }
            }
        }
    }

}