
using System;
using UnityEngine;

namespace ShipDock.FSM
{
    public class AnimatorStateMachine : StateMachine
    {

        public AnimatorStateMachine(ref Animator target, int name, Transform animatorTF = default, Action<IStateMachine> fsmRegister = default) : base(name, fsmRegister)
        {
            FSMRegister = fsmRegister;
            Init(ref target, ref name, ref animatorTF);
        }

        private void Init(ref Animator target, ref int name, ref Transform animatorTF)
        {
            InitAnimatorStateMachine(ref target, ref name, ref animatorTF);
            
            FSMRegister?.Invoke(this);
        }

        protected virtual void InitAnimatorStateMachine(ref Animator target, ref int name, ref Transform animatorTF)
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
                    if (animatorTF != null)
                    {
                        animatorState.SetTransform(ref animatorTF);
                    }
                }
            }
        }
    }
}
