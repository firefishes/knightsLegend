using UnityEngine;

namespace ShipDock.FSM
{
    public interface IAnimatorState
    {
        void SetAnimator(ref Animator target);
        void SetTransform(ref Transform target);
        string AnimationName { get; set; }
        bool CanPlay { get; set; }
    }
}
