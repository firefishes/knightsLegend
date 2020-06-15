using System.Collections.Generic;

namespace KLGame
{
    public interface IAIExecutable
    {
        void Calculation();
        void ApplyPlan(ref IGoal goal, ref List<IWorldState> worldStates);
        bool CheckFeasible(IWorldState worldState);
        IWorldEffect[] Effects { get; }
        int Cost { get; }
        int OrientedType { get; }
    }

}