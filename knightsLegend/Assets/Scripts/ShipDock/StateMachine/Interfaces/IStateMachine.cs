using ShipDock.Interfaces;
using System;

namespace ShipDock.FSM
{
    /// <summary>
    /// 状态机接口
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public interface IStateMachine : IDispose {

        void Run(IStateParam param = null, int initState = int.MaxValue);
        void StopStateMachine();
        void ChangeToDefaultState(IStateParam param = null);
        void ChangeToNextState(IStateParam param = null);
        void ChangeToPreviousState(IStateParam param = null);
        void UpdateState(int dTime);
        void ChangeState(int name, IStateParam param = null);
        void ChangeStateByIndex(int index, IStateParam param = null);
        Action<IStateMachine, int> OnFSMChanged { get; set; }
        Action<IStateMachine, bool> FSMFrameUpdater { get; set; }
        Action<IStateMachine> FSMRegister { get; set; }
        Action<IState, bool> StateFrameUpdater { get; set; }
        IStateParam SubStateParam { set; get; }
        IState[] StateInfos { get; }
        IState Current { get; }
        IState Previous { get; }
        IState Next { get; }
        int SubStateWillChange { set; get; }
        int DefaultState { get; }
        int StateCount { get; }
        int Name { get; }
    }
}
