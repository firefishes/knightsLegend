
using ShipDock.Interfaces;
using System;

namespace ShipDock.FSM
{
    /// <summary>
    /// 
    /// 状态接口
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public interface IState : IDispose
    {
        
        void SetFSMName(int FSMName);
        void InitState(IStateParam param = null);
        void AddSubState(int name, IState sub);
        void RemoveSubState(int name);
        void DeinitState();
        void UpdateState(int dTime);
        void ChangeSubState(int name, IStateParam param = null);
        void ChangeSubStateToDefault(IStateParam param = null);
        void ChangeToState(int name, IStateParam param = null);
        void ChangeToNextState(IStateParam param = null);
        void ChangeToPreviousState(IStateParam param = null);
        void ChangeToDefaultState(IStateParam param = null);
        void SetStateParam(IStateParam param);
        Action<IState, bool> StateFrameUpdater { get; set; }
        void SetFSM(IStateMachine fsm);
        IStateMachine GetFSM();
        IState[] SubStateInfos { get; }
        IState SubState { get; }
        bool IsApplyAutoNext { get; }
        bool IsValid { get; }
        bool IsDeinitLater { get; }
        int StateName { get; set; }
        int SubStateName { get; }
        int DefaultState { get; }
        int NextState { get; }
        int PreviousState { get; }
        int SubStateCount { get; }
    }
}
