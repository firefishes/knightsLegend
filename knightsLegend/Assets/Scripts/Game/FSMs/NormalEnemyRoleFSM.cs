using ShipDock.Applications;
using ShipDock.FSM;

namespace KLGame
{
    public class NormalEnemyRoleFSM : CommonRoleFSM, IAssailableCommiter
    {
        public NormalEnemyRoleFSM(int name) : base(name)
        {
            IsApplyFastChange = true;
        }

        public bool HitCommit()
        {
            if (Current is IAssailableCommiter target)
            {
                return target.HitCommit();
            }
            return false;
        }

        public override IState[] StateInfos { get; } = new IState[]
        {
            new FState(DEFAULT_STATE),
            new UnderAttackState(NormalRoleStateName.UNDER_ATK),
            new GroundedState(NormalRoleStateName.GROUNDED),
            new NormalEnemyATKState(NormalRoleStateName.NORMAL_ATK),
        };
    }
}