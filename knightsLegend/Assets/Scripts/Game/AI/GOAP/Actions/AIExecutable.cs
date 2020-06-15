using System.Collections.Generic;

namespace KLGame
{
    public abstract class AIExecutable : IAIExecutable
    {
        protected IGoal mGoal;
        protected List<IWorldState> mWorldStates;

        public void Calculation()
        {

        }

        public abstract bool CheckFeasible(IWorldState worldStates);

        public void ApplyPlan(ref IGoal goal, ref List<IWorldState> worldStates)
        {
            mGoal = goal;
            mWorldStates = worldStates;
        }

        public abstract IWorldEffect[] Effects { get; }
        public abstract int Cost { get; protected set; }
        public abstract int OrientedType { get; }
    }

}