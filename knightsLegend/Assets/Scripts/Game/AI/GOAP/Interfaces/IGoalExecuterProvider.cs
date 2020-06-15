namespace KLGame
{
    public interface IGoalExecuter
    {
        void SetGoal(IGoal goal);
        bool HasPlanned { get; }
        int GoalExecuterID { get; }
        IGoal CurrentGoal { get; }
        IAIExecutable[] AIExecutables { get; }
        IGoalPlanner GoalPlanner { get; }
    }
}
