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

        public override void CheckFeasible(IWorldState worldStates, out int feasibleStatu)
        {
            feasibleStatu = 1;
            IPosState posState = worldStates as IPosState;
            if (posState == default)
            {
                feasibleStatu = 0;
                return;
            }

            float distance = Vector3.Distance(mRole.Position, posState.GetPosition());
            Cost = (int)distance;
            feasibleStatu = distance > mRole.GetStopDistance() ? 1 : 2;//TODO 同时需要被看到
        }

        public override IWorldEffect[] Effects { get; } = new IWorldEffect[] {
            new TrackTargetWorldEffect(),
        };

        public override int Cost { get; protected set; } = 1;

        public override int OrientedType { get; } = KLConsts.WORLD_STATE_ORIENTED_OBJECTIVE;
    }

    public abstract class WorldEffect : IWorldEffect
    {
        public virtual void CommitEffect(ref IWorldState worldState)
        {
            worldState.ApplyEffect(this);
        }

        public bool Available { get; set; }
    }

    public class TrackTargetWorldEffect : WorldEffect
    {
    }
}