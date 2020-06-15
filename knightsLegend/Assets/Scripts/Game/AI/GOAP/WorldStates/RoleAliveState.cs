using UnityEngine;

namespace KLGame
{
    public interface IPosState : IWorldState
    {
        Vector3 GetPosition();
    }

    public class RoleAliveState : IPosState
    {
        public RoleAliveState(IKLRole target)
        {
            KillTarget = target;
            SetStateID(KillTarget.ID);
        }

        public void SetStateID(int id)
        {
            StateID = id;
        }

        public Vector3 GetPosition()
        {
            return KillTarget.Position;
        }

        public IKLRole KillTarget { get; private set; }

        public bool Available { get; set; }
        public bool WillRemoveFromWorld { get; set; }
        public int OrientedType { get; } = KLConsts.WORLD_STATE_ORIENTED_OBJECTIVE;
        public int StateID { get; private set; }

    }

}