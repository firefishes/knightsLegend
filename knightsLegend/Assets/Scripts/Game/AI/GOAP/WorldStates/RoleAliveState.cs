using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public interface IPosState : IWorldState
    {
        Vector3 GetPosition();
    }

    public abstract class WorldState : IWorldState
    {
        public WorldState()
        {
            Effecteds = new Queue<IWorldEffect>();
        }

        public virtual void ApplyEffect(IWorldEffect effect)
        {
            Effecteds.Enqueue(effect);
        }

        public void RevertEffect()
        {
        }

        public virtual void SetStateID(int id)
        {
            StateID = id;
        }

        public int StateID { get; protected set; }
        public bool Available { get; set; }
        public bool WillRemoveFromWorld { get; set; }
        public abstract int OrientedType { get; }
        public abstract Queue<IWorldEffect> Effecteds { get; protected set; }
    }

    public class RoleAliveState : WorldState, IPosState
    {
        public RoleAliveState(IKLRole target)
        {
            KillTarget = target;
            SetStateID(KillTarget.ID);
        }

        public Vector3 GetPosition()
        {
            return KillTarget.Position;
        }

        public IKLRole KillTarget { get; private set; }
        
        public override int OrientedType { get; } = KLConsts.WORLD_STATE_ORIENTED_OBJECTIVE;
        public override Queue<IWorldEffect> Effecteds { get; protected set; }

    }

}