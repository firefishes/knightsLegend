namespace KLGame
{
    public interface IGoal
    {
        void Clean();
        void Init(IGoalIssuer issuer);
        bool ShouldPlanByReceiver(IGoalIssuer target);
        bool IsFinished();
        bool Available { get; set; }
        bool WillRemoveFromWorld { get; set; }
    }

}
