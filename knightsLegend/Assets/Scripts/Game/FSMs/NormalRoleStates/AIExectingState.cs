using ShipDock.FSM;

namespace KLGame
{
    public class AIExectingState : FState
    {
        public AIExectingState(int name) : base(name)
        {
        }
    }

    public class NormalEnemyAIExecutingState : AIExectingState
    {
        public NormalEnemyAIExecutingState(int name) : base(name)
        {
        }

        public override IState[] SubStateInfos { get; } = new IState[]
        {
            new UnderAttackState(NormalRoleStateName.UNDER_ATK),
            new GroundedState(NormalRoleStateName.GROUNDED),
            new NormalEnemyATKState(NormalRoleStateName.NORMAL_ATK),
            new NormalAIState(NormalRoleStateName.NORMAL_AI),
            new NormalDefenceState(NormalRoleStateName.NORMAL_DEF),
        };
    }
}