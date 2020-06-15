using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class WillKillGoal : AIGoal
    {
        private IKLRole mKillTarget;

        public override void Init(IGoalIssuer issuer)
        {
            base.Init(issuer);

            mKillTarget = issuer as IKLRole;
        }

        public override bool IsFinished()
        {
            return mKillTarget.IsDead;
        }

        public override bool ShouldPlanByReceiver(IGoalIssuer receiver)
        {
            if (receiver is IKLRole role)
            {
                int camp = role.Camp;
                return (receiver != mKillTarget) && (camp != mKillTarget.Camp);//TODO 增加中立阵营，用于不可被攻击的角色
            }
            return false;
        }
    }
}