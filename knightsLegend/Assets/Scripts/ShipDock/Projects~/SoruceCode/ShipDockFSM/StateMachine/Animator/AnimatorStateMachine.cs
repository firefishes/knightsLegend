
using System;
using UnityEngine;

namespace ShipDock.FSM
{
    public class AnimatorStateMachine : StateMachine
    {
        private IAnimatorState mAniState;

        public AnimatorParamer AniParamer { get; private set; } = default;

        public AnimatorStateMachine(Animator target, int name, Action<IStateMachine> fsmRegister = default) : base(name, fsmRegister)
        {
            AniParamer = new AnimatorParamer();

            Init(ref target);

            AniParamer.SetAnimator(ref target);
        }

        public override void Dispose()
        {
            base.Dispose();

            AniParamer?.Clear();
            AniParamer = default;
        }

        private void Init(ref Animator target)
        {
            InitAnimatorStateMachine(ref target);
        }

        protected virtual void InitAnimatorStateMachine(ref Animator target)
        {
            IState state;
            IAnimatorState animatorState;
            int max = StateCount;
            for (int i = 0; i < max; i++)
            {
                state = GetStateByIndex(i);
                if (state is IAnimatorState)
                {
                    animatorState = state as IAnimatorState;
                    animatorState.SetAnimator(ref target);
                }
                else { }

                InitAniState(ref state);
            }
        }

        protected virtual void InitAniState(ref IState state)
        {
        }

        public void SetAnimator(ref Animator animator)
        {
            InitAnimatorStateMachine(ref animator);
        }

        public override void UpdateState(int dTime)
        {
            base.UpdateState(dTime);

            if (mAniState != default && AniParamer != default)
            {
                if (string.IsNullOrEmpty(AniParamer.MotionName))
                {
                    AniParamer?.CommitParamToAnimator();
                }
                else
                {
                    mAniState.CheckMotionComplete(AniParamer);
                }
            }
            else { }
        }

        protected override void StateChanging()
        {
            bool isParamerNotEmpty = AniParamer != default;
            if (isParamerNotEmpty)
            {
                AniParamer.ResetMotionState();
                AniParamer.ResetMotionWillPlay();
            }

            base.StateChanging();

            if (isParamerNotEmpty)
            {
                if (Current is IAnimatorState state)
                {
                    state.CheckStateAnimation(AniParamer);
                    mAniState = state;
                }
                else { }
            }
            else { }
        }
    }
}
