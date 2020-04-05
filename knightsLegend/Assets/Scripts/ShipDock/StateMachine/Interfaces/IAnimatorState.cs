using UnityEngine;

namespace ShipDock.FSM
{
    public interface IAnimatorState
    {
        void SetAnimator(ref Animator target);
        string AnimationName { get; set; }
        bool CanPlay { get; set; }
        Animator Animator { get; }
    }
}
