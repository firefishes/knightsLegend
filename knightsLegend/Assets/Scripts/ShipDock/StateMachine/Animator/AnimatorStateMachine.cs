
using System;
using UnityEngine;

namespace ShipDock.FSM
{
    public class AnimatorStateMachine : StateMachine
    {

        public AnimatorStateMachine(Animator target, int name, Action<IStateMachine> fsmRegister = default) : base(name, fsmRegister)
        {
            FSMRegister = fsmRegister;
            Init(ref target);
        }

        private void Init(ref Animator target)
        {
            InitAnimatorStateMachine(ref target);
            
            FSMRegister?.Invoke(this);
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
            }
        }

        public void SetAnimator(ref Animator animator)
        {
            InitAnimatorStateMachine(ref animator);            
        }
    }
}
