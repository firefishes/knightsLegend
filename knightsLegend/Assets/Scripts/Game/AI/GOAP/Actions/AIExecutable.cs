using System;
using System.Collections.Generic;

namespace KLGame
{
    public abstract class AIExecutable : IAIExecutable
    {
        protected IGoal mGoal;
        protected List<IWorldState> mWorldStates;

        public void Clean()
        {
        }

        public abstract void CheckFeasible(IWorldState worldStates, out int feasibleStatu);

        public void ApplyPlan(ref IGoal goal, ref List<IWorldState> worldStates)
        {
            mGoal = goal;
            mWorldStates = worldStates;
        }

        public void CommitEffect(ref IWorldState worldState)
        {
            IWorldEffect effect;
            int max = Effects.Length;
            for (int i = 0; i < max; i++)
            {
                effect = Effects[i];
                effect.CommitEffect(ref worldState);
            }
        }

        public abstract IWorldEffect[] Effects { get; }
        public abstract int Cost { get; protected set; }
        public abstract int OrientedType { get; }
    }

}