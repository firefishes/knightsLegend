using ShipDock.Pooling;
using ShipDock.Tools;

namespace KLGame
{

    public class PlanDetail : IPoolable
    {
        public static PlanDetail Obtain()
        {
            PlanDetail result = Pooling<PlanDetail>.From();
            return result;
        }

        public IGoal goal;
        public IGoalPlanner planner;
        public IAIExecutable[] executables;

        public void Reinit(IGoal goalParam, IGoalPlanner planerParam, ref IAIExecutable[] executablesParam)
        {
            goal = goalParam;
            planner = planerParam;
            executables = executablesParam;
        }

        public void Revert()
        {
            Utils.Reclaim(ref executables);
            planner = default;
        }

        public void ToPool()
        {
            Pooling<PlanDetail>.To(this);
        }

        public void DrawUp(ref WorldStatesMapper states)
        {
            PlanGraphic planGraphic = new PlanGraphic();
            planner.Planning(0, ref goal, ref states, ref executables, ref planGraphic);
        }
    }
}
