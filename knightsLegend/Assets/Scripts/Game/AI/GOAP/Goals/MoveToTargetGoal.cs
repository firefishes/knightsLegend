namespace KLGame
{
    public class MoveToTargetGoal : AIGoal
    {
        private IKLRole mRole;

        public override void Init(IGoalIssuer issuer)
        {
            base.Init(issuer);

            mRole = issuer as IKLRole;
        }

        public override bool IsFinished()
        {
            return false;
        }

        public override bool ShouldPlanByReceiver(IGoalIssuer target)
        {
            return GoalIssuer != target;
        }
    }
}