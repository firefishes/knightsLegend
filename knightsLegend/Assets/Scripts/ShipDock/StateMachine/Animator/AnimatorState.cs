
using UnityEngine;

namespace ShipDock.FSM
{
    public class AnimatorState : FState, IAnimatorState
    {
        protected Animator mAnimator;
        protected Transform mAnimatorTF;
        protected string mAnimationName;

        private bool mCanPlay = true;

        public AnimatorState(int name) : base(name)
        {
        }

        public override void Dispose()
        {
            base.Dispose();

            mAnimator = null;
            mAnimatorTF = null;
            mAnimationName = string.Empty;
        }

        public override void InitState(IStateParam param = null)
        {
            base.InitState(param);
            
        }

        protected virtual void PlayAnimation()
        {
            if (mCanPlay && mAnimator != null)
            {
                mAnimator.Play(mAnimationName);
            }
        }

        public virtual void SetAnimator(ref Animator target)
        {
            mAnimator = target;
            mAnimatorTF = mAnimator.transform;
        }

        public void SetTransform(ref Transform target)
        {
            mAnimatorTF = target;
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
    }
}