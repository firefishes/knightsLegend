using System.Collections.Generic;

namespace KLGame
{
    public interface IAIExecutable
    {
        void Clean();
        void ApplyPlan(ref IGoal goal, ref List<IWorldState> worldStates);
        void CheckFeasible(IWorldState worldState, out int feasibleStatu);
        void CommitEffect(ref IWorldState worldState);
        IWorldEffect[] Effects { get; }
        int Cost { get; }
        int OrientedType { get; }
    }

}