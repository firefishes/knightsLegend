using ShipDock.Applications;
using ShipDock.FSM;
using UnityEngine;

namespace KLGame
{
    public class NormalEnemyRoleFSM : CommonRoleFSM, IAssailableCommiter
    {
        public NormalEnemyRoleFSM(int name) : base(name)
        {
            IsApplyFastChange = true;
        }

        public bool HitCommit(int hitCollidID)
        {
            if (Current is IAssailableCommiter target)
            {
                return target.HitCommit(hitCollidID);//判定攻击是否有效
            }
            return false;
        }

        public override IState[] StateInfos { get; } = new IState[]
        {
            new FState(DEFAULT_STATE),
            new UnderAttackState(NormalRoleStateName.UNDER_ATK),
            new GroundedState(NormalRoleStateName.GROUNDED),
            //new NormalEnemyATKState(NormalRoleStateName.NORMAL_ATK),
            //new NormalAttackAIState(NormalRoleStateName.NORMAL_ATTACK_AI),
            new NormalDefenceState(NormalRoleStateName.NORMAL_DEF),
        };
    }
}