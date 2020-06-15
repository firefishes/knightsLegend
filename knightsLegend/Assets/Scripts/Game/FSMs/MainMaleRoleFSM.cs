using ShipDock.Applications;
using ShipDock.FSM;

namespace KLGame
{
    public class MainMaleRoleFSM : CommonRoleFSM, IAssailableCommiter
    {
        public MainMaleRoleFSM(int name) : base(name)
        {
            //IsApplyFastChange = true;
        }

        public bool HitCommit(int hitCollidID) 
        {
            if(Current is IAssailableCommiter target)
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
            new NormalATKState(NormalRoleStateName.NORMAL_ATK),
            new NormalDefenceState(NormalRoleStateName.NORMAL_DEF),
        };
    }

}