using ShipDock.Interfaces;
using System;

namespace ShipDock.Applications
{
    public class RoleInputDefaultPhase : IRoleInputPhase, IDispose
    {
        protected ICommonRole mRoleItem;
        protected int mPhaseMax = -1;

        private bool mIsExecuting;

        public RoleInputDefaultPhase() { }

        public RoleInputDefaultPhase(params int[] newMapper)
        {
            PhasesMapper = newMapper;
            mPhaseMax = newMapper.Length - 1;
        }

        public virtual void Dispose()
        {
            RoleInput = default;
            mPhaseMax = -1;
        }

        protected bool PhaseTransitionValid(bool reset)
        {
            if(mPhaseMax == -1)
            {
                mPhaseMax = PhasesMapper.Length - 1;
            }
            bool result = PhaseTransitions <= mPhaseMax;
            if(reset && !result)
            {
                PhaseTransitions = mPhaseMax;
            }
            return result;
                
        }

        public virtual void ExecuteByEntitasComponent()
        {
        }

        public virtual void ExecuteBySceneComponent(Action sceneCompCallback = default)
        {
            if (mIsExecuting)
            {
                return;
            }
            mIsExecuting = true;
            sceneCompCallback?.Invoke();
            mIsExecuting = false;
        }

        public void SetRoleInput(IRoleInput target)
        {
            RoleInput = target;
        }

        public virtual int AdvancedInputPhase()
        {
            return PhasesMapper[PhaseTransitions];
        }

        public void DefaultAdvance()
        {
            PhaseTransitions++;
            PhaseTransitionValid(true);
        }
        
        public virtual void SetRoleEntitas(ref ICommonRole target)
        {
            mRoleItem = target;
        }

        protected IRoleInput RoleInput { get; private set; }

        public int PhaseTransitions { get; set; }
        public virtual int[] PhasesMapper { get; } = new int[] { 0 };
    }

    public interface IRoleInputOnlyOnce
    {
        bool IsExecuted { get; set; }
    }

    public class RoleInputOnlyExecuteByScene : RoleInputDefaultPhase, IRoleInputOnlyOnce
    {
        public RoleInputOnlyExecuteByScene(params int[] newMapper) : base(newMapper)
        {
        }
        
        public sealed override void ExecuteByEntitasComponent() { }

        public override void ExecuteBySceneComponent(Action sceneCompCallback = null)
        {
            IsExecuted = true;

            base.ExecuteBySceneComponent(sceneCompCallback);
        }

        public bool IsExecuted { get; set; }
    }

    public class RoleInputOnlyExecuteByEntitas : RoleInputDefaultPhase, IRoleInputOnlyOnce
    {
        public RoleInputOnlyExecuteByEntitas(params int[] newMapper) : base(newMapper)
        {
        }

        public sealed override void ExecuteBySceneComponent(Action sceneCompCallback = null) { }
        
        public override void ExecuteByEntitasComponent()
        {
            IsExecuted = true;

            ExecuteInEntitas();
        }

        protected virtual void ExecuteInEntitas()
        {
        }

        public bool IsExecuted { get; set; }

    }
}