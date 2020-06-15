using System.Collections.Generic;

namespace KLGame
{
    public interface IGoalPlanner
    {
        void WillPlan(IGoalExecuter executerProvider);
        void StartPlan(ref List<IGoal> goals, ref List<IWorldState> worldStates, ref Queue<PlanDetail> planDetailQueue);
        void Planning(int index, ref IGoal goal, ref WorldStatesMapper worldStates, ref IAIExecutable[] executables, ref PlanGraphic planGraphic);
    }
}
