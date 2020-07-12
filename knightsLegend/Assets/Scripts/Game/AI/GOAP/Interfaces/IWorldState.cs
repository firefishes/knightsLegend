using System.Collections.Generic;

namespace KLGame
{
    public interface IWorldState
    {
        void ApplyEffect(IWorldEffect effect);
        void SetStateID(int id);
        int StateID { get; }
        bool Available { get; set; }
        bool WillRemoveFromWorld { get; set; }
        void RevertEffect();
        int OrientedType { get; }
        Queue<IWorldEffect> Effecteds { get; }
    }
}