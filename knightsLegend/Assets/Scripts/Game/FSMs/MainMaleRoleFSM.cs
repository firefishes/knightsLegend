using ShipDock.Applications;
using ShipDock.FSM;

namespace KLGame
{
    public class MainMaleRoleFSM : CommonRoleFSM, IAssailableCommiter
    {
        public MainMaleRoleFSM(int name) : base(name)
        {
            IsApplyFastChange = true;
        }

        public bool HitCommit() 
        {
            if(Current is IAssailableCommiter target)
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
            new NormalATKState(NormalRoleStateName.NORMAL_ATK),
        };
    }

}