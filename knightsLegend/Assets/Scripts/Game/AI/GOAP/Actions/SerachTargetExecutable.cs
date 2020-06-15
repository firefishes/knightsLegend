using UnityEngine;

namespace KLGame
{
    public class SerachTargetExecutable : AIExecutable
    {
        private readonly IAIRole mRole;

        public SerachTargetExecutable(IAIRole target)
        {
            mRole = target;
        }

        public override bool CheckFeasible(IWorldState worldStates)
        {
            IPosState posState = worldStates as IPosState;
            if (posState == default)
            {
                return false;
            }

            float distance = Vector3.Distance(mRole.Position, posState.GetPosition());
            Cost = (int)distance;
            return distance > mRole.GetStopDistance();//TODO 同时需要被看到
        }

        public override IWorldEffect[] Effects
        {
            get;
        } = default;

        public override int Cost { get; protected set; } = 1;

        public override int OrientedType { get; } = KLConsts.WORLD_STATE_ORIENTED_OBJECTIVE;
    }

}