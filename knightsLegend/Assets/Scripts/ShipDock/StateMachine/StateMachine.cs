#define G_LOG

using System;
using System.Collections.Generic;
using ShipDock.Testers;
using ShipDock.Tools;

namespace ShipDock.FSM
{
    /// <summary>
    /// 
    /// 有限状态机类
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class StateMachine : IStateMachine
    {

        #region 静态属性
        public const int DEFAULT_STATE = -1;
        #endregion

        #region 私有属性
        /// <summary>当前状态的索引</summary>
        private int mStateIndex;
        /// <summary>状态机名</summary>
        private int mFSMName;
        /// <summary>下一个状态，以状态列表的顺序为依据</summary>
        private IState mNext;
        /// <summary>上一个状态</summary>
        private IState mPrevious;
        /// <summary>转换中的状态</summary>
        private IState mChangingState;
        /// <summary>状态切换使用的参数</summary>
        private IStateParam mStateParam;
        /// <summary>所有状态对象的映射</summary>
        private KeyValueList<int, IState> mStates;
        /// <summary>所有状态对象的列表</summary>
        private List<IState> mStateList;
        #endregion

        #region 构造函数
        public StateMachine(int name, Action<IStateMachine> fsmRegister = default)
        {
            FSMRegister = fsmRegister;
            Init(name);
        }

        private void Init(int name)
        {
            mFSMName = name;

            FSMRegister?.Invoke(this);

            InitStates(ref name);
        }
        #endregion

        #region 销毁
        /// <summary>销毁</summary>
        public virtual void Dispose()
        {
            StopStateMachine();

            if (mStateList != null)
            {
                int max = mStateList.Count;
                for (var i = 0; i < max; i++)
                {
                    Utils.Reclaim(mStateList[i]);
                }
            }

            Utils.Reclaim(ref mStates, true);
            Utils.Reclaim(ref mStateList, true);

            mNext = default;
            Current = default;
            mPrevious = default;
            mStateParam = default;
            mChangingState = default;
            FSMFrameUpdater = default;
            FSMRegister = default;
            OnFSMChanged = default;
        }
        #endregion

        #region 初始化
        /// <summary>初始化各状态</summary>
        protected virtual void InitStates(ref int name)
        {
            IsRun = false;
            mStateIndex = 0;
            mStates = new KeyValueList<int, IState>();
            SubStateWillChange = int.MaxValue;
            
            IState state;
            int max = StateInfos.Length;
            mStateList = new List<IState>(max);
            for (int i = 0; i < max; i++)
            {
                state = StateInfos[i];
                if(state.IsValid)
                {
                    state.SetFSMName(name);
                    state.SetFSM(this);
                    state.StateFrameUpdater = StateFrameUpdater;
                    AddState(i, ref state);
                    AfterStateInited(ref state);
                }
                else
                {
                    state.Dispose();
                }
            }
            Array.Clear(StateInfos, 0, StateInfos.Length);
        }

        /// <summary>子状态初始化之后</summary>
        protected virtual void AfterStateInited(ref IState state)
        {
        }
        #endregion

        #region 状态管理
        /// <summary>添加状态</summary>
        public void AddState(int index, ref IState info)
        {
            if (mStates == null)
            {
                return;
            }
            CreateStatesMap(info.StateName, info, index);
        }

        protected virtual IState CreateStatesMap(int name, IState state, int index = -1)
        {
            if (!mStates.IsContainsKey(name))
            {
                mStates.Put(name, state);
            }
            else
            {
                Tester.Instance.Log(TesterBaseApp.Instance, TesterBaseApp.LOG5, mFSMName.ToString());
                return null;
            }
            if ((index == -1) || (mStateList.Count <= index))
            {
                mStateList.Add(state);
            }
            else
            {
                if(mStateList[index] == null)
                {
                    mStateList[index] = state;
                }
                else
                {
                    mStateList.Insert(index, state);
                }
            }
            return state;
        }

        /// <summary>移除状态</summary>
        public void RemoveState(int name)
        {
            if (mStates == default)
            {
                return; 
            }

            IState state = mStates.Remove(name);
            int index = mStateList.IndexOf(state);
            mStateList.RemoveAt(index);

            if (state == Current)
            {
                ChangeToDefaultState();
            }
            Utils.Reclaim(state);
        }

        protected IState GetState(int name)
        {
            return mStates.GetValue(name);
        }

        protected IState GetStateByIndex(int index)
        {
            return mStates.GetValueByIndex(index);
        }

        #endregion

        #region 状态修改
        /// <summary>更改至下一个状态</summary>
        public void ChangeToNextState(IStateParam param = null)
        {
            if (Current != null)
            {
                if (Current.NextState == int.MaxValue)
                {
                    mNext = mStates.GetValue(Current.NextState);
                }
                if ((mNext == null) && Current.IsApplyAutoNext)
                {
                    int n = mStateIndex + 1;
                    mNext = (n <= (mStateList.Count - 1)) ? mStateList[n] : null;
                }
                if (mNext == null)
                {
                    return;
                }
            }
            else
            {
                return;
            }
            SetCurrentState(ref mNext, param);
        }

        /// <summary>更改至上一个状态</summary>
        public void ChangeToPreviousState(IStateParam param = null)
        {
            if (mPrevious == null)
            {
                if ((Current != null) && (Current.PreviousState == int.MaxValue))
                {
                    mPrevious = mStates.GetValue(Current.PreviousState);
                }
                else
                {
                    mPrevious = (mStateIndex - 1 >= 0) ? mStateList[mStateIndex - 1] : null;
                }
            }
            if (mPrevious == null)
            {
                return;
            }
            SetCurrentState(ref mPrevious, param);
        }

        /// <summary>更改至指定状态</summary>
        public virtual void ChangeState(int name, IStateParam param = null)
        {
            IState state = mStates.IsContainsKey(name) ? mStates[name] : null;
            SetCurrentState(ref state, param);
        }

        public virtual void ChangeStateByIndex(int index, IStateParam param = null)
        {
            IState state = GetState(index);
            SetCurrentState(ref state, param);
        }

        /// <summary>更改至默认状态</summary>
        public virtual void ChangeToDefaultState(IStateParam param = null)
        {
            IState defaultState = mStates.GetValue(DefaultState);
            if (defaultState != null)
            {
                SetCurrentState(ref defaultState, param);
            }
        }

        /// <summary>设置当前状态</summary>
        private void SetCurrentState(ref IState state, IStateParam param = null)
        {
            if (state == default)
            {
                ChangeToDefaultState(param);
                return;
            }
            if (mChangingState != state)
            {
                IsStateChanging = true;
                mStateParam = param;
                mChangingState = state;

                if (IsApplyFastChange)
                {
                    ChangedStateFinish();
                }
            }
            else
            {
                if (Current != null)
                {
                    Current.SetStateParam(param);
                }
            }
        }
        #endregion

        #region 启动、停止和更新
        /// <summary>运行状态机</summary>
        public virtual void Run(IStateParam param = null, int initState = int.MaxValue)
        {
            IsRun = true;

            FSMFrameUpdater?.Invoke(this, true);

            if (initState != int.MaxValue)
            {
                ChangeState(initState, param);
            }
            else if (DefaultState != int.MaxValue)
            {
                ChangeToDefaultState(param);
            }
        }

        /// <summary>停止状态机</summary>
        public virtual void StopStateMachine()
        {
            IsRun = false;
            FSMFrameUpdater?.Invoke(this, false);
        }

        /// <summary>更新当前状态</summary>
        public virtual void UpdateState(int dTime)
        {
            #region 完成状态修改
            if (!IsApplyFastChange)
            {
                ChangedStateFinish();
            }
            #endregion

            //if (Current != null)
            //{
            //    Current.UpdateState(dTime);
            //}
        }

        private void ChangedStateFinish()
        {
            if (IsStateChanging)
            {
                IsStateChanging = false;
                BeforeStateChange();
                if (IsStateChanging)
                {
                    return;//防止 DeinitState 的逻辑中存在跳转状态操作引起的参数错误
                }

                StateChanging();
                if (IsStateChanging)
                {
                    return;//防止多次跳转状态操作同时发生引起的参数错误
                }
                Tester.Instance.Log(TesterBaseApp.Instance,
                                    TesterBaseApp.LOG4,
                                    GetType().Name,
                                    (mPrevious != null) ? mPrevious.StateName.ToString() : DefaultState.ToString(),
                                    CurrentStateName.ToString());
                AfterStateChanged();
            }
        }

        private void BeforeStateChange()
        {
            IState state = Current;
            bool isDeinitLater = IsStateDeinitLater(ref state);
            if (!isDeinitLater)
            {
                if (Current != null)
                {
                    Current.DeinitState();
                }
            }
        }

        private void StateChanging()
        {
            mPrevious = Current;
            Current = mChangingState;
            CurrentStateName = Current.StateName;
            mStateIndex = mStateList.IndexOf(Current);
            Current.InitState(mStateParam);

            if (IsStateDeinitLater(ref mPrevious))
            {
                mPrevious.DeinitState();
            }
        }

        private void AfterStateChanged()
        {
            if ((Current.SubStateCount > 0) && (SubStateWillChange != int.MaxValue))
            {
                Current.ChangeSubState(SubStateWillChange, SubStateParam);
            }
            if (OnFSMChanged != null)
            {
                OnFSMChanged.Invoke(this, CurrentStateName);
            }

            mNext = null;
            mStateParam = null;
            SubStateParam = null;
            SubStateWillChange = int.MaxValue;
        }

        private bool IsStateDeinitLater(ref IState state)
        {
            return (state != null) && state.IsDeinitLater;
        }
        #endregion

        #region 属性
        /// <summary>状态机名</summary>
        public virtual int Name
        {
            get
            {
                return mFSMName;
            }
        }

        /// <summary>覆盖此方法，定义状态列表</summary>
        public virtual IState[] StateInfos
        {
            get
            {
                return new IState[0];
            }
        }

        /// <summary>覆盖此方法，定义默认状态名</summary>
        public virtual int DefaultState
        {
            get
            {
                return DEFAULT_STATE;
            }
        }

        public IState Previous
        {
            get
            {
                return mPrevious;
            }
        }

        public IState Next
        {
            get
            {
                return mNext;
            }
        }

        public int StateCount
        {
            get
            {
                return (mStates != null) ? mStates.Size : 0;
            }
        }

        public int SubStateWillChange { get; set; }
        public int CurrentStateName { get; private set; }
        public bool IsRun { get; private set; }
        public bool IsApplyFastChange { get; set; }
        public bool IsStateChanging { get; private set; }
        public Action<IStateMachine, bool> FSMFrameUpdater { get; set; }
        public Action<IStateMachine> FSMRegister { get; set; }
        public Action<IStateMachine, int> OnFSMChanged { get; set; }
        public Action<IState, bool> StateFrameUpdater { get; set; }
        public IStateParam SubStateParam { get; set; }
        public IState Current { get; private set; }
        #endregion

    }
}