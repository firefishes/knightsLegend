
using UnityEngine;

namespace ShipDock.FSM
{
    public class AnimatorState : FState, IAnimatorState
    {
        protected string mAnimationName;

        private bool mCanPlay = true;

        public AnimatorState(int name) : base(name)
        {
        }

        public override void Dispose()
        {
            base.Dispose();

            Animator = null;
            mAnimationName = string.Empty;
        }

        public override void InitState(IStateParam param = null)
        {
            base.InitState(param);
            
        }

        protected virtual void PlayAnimation()
        {
            if (mCanPlay && Animator != null)
            {
                Animator.Play(mAnimationName);
            }
        }

        public virtual void SetAnimator(ref Animator target)
        {
            Animator = target;
        }
        
        public virtual string AnimationName
        {
            get
            {
                return mAnimationName;
            }
            set
            {
                mAnimationName = value;
            }
        }

        public bool CanPlay
        {
            get
            {
                return mCanPlay && string.IsNullOrEmpty(mAnimationName);
            }

            set
            {
                mCanPlay = value;
            }
        }

        public Animator Animator { get; private set; }
    }
}