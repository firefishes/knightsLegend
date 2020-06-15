namespace KLGame
{
    public abstract class AIGoal : IGoal
    {

        public virtual void Init(IGoalIssuer issuer)
        {
            GoalIssuer = issuer;
        }

        public virtual void Clean()
        {
            GoalIssuer = default;
        }

        public abstract bool IsFinished();
        public abstract bool ShouldPlanByReceiver(IGoalIssuer target);

        protected IGoalIssuer GoalIssuer { get; private set; }

        public bool Available { get; set; }
        public bool WillRemoveFromWorld { get; set; }
    }
}