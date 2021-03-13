using UnityEngine;

namespace ShipDock.FSM
{
    public interface IAnimatorState : IState
    {
        string AnimationName { get; set; }
        bool ShouldPlay { get; }
        bool IsAniPlaying { get; }
        int LayerIndex { get; set; }
        Animator Animator { get; }
        void SetAnimator(ref Animator target);
        void CheckStateAnimation(AnimatorParamer paramer);
        void CheckMotionComplete(AnimatorParamer paramer);
    }
}
