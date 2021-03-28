#define _G_LOG

using ShipDock.Tools;
using System;
using System.Collections.Generic;

namespace ShipDock.FSM
{
    public class FSMStateParam : IStateParam
    {
        public int name;
        public IStateMachine FSM;
    }

    /// <summary>
    /// 
    /// 状态类
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class FState : IState
    {
        #region 受保护的属性
        /// <summary>已更改的状态参数</summary>
        protected object mSelfParam;
        /// <summary>当前子状态</summary>
        protected IState mSubState;
        /// <summary>状态是否被激活</summary>
        protected bool mIsStateActived;
        #endregion

        #region 私有属性
        /// <summary>状态是否已初始化</summary>
        private bool mIsInited;
        /// <summary>状态名</summary>
        private int mStateName = int.MaxValue;
        /// <summary>状态是否被切换</summary>
        private bool mIsStateChanged;
        /// <summary>即将要切换到的状态</summary>
        private IState mChangingState;
        /// <summary>即将要切换到的状态使用的参数</summary>
        private IStateParam mChangingStateParam;
        /// <summary>状态所在状态机的名称</summary>
        private int mFSMName;
        /// <summary>状态所在状态机引用</summary>
        private IStateMachine mFSM;
        /// <summary>所有子状态</summary>
        private KeyValueList<int, IState> mSubStateList;
        #endregion

        #region 属性
        public virtual int StateName
        {
            get
            {
                return mStateName;
            }
            set
            {
                mStateName = value;
            }
        }

        public virtual int SubStateName
        {
            get
            {
                return (mSubState != null) ? mSubState.StateName : int.MaxValue;
            }
        }

        /// <summary>覆盖此方法，定义子状态列表</summary>
        public virtual IState[] SubStateInfos
        {
            get
            {
                return new IState[0];
            }
        }
        public virtual int DefaultState
        {
            get
            {
                return int.MaxValue;
            }
        }

        public virtual IState SubState
        {
            get
            {
                return mSubState;
            }
        }

        public virtual int NextState
        {
            get
            {
                return int.MaxValue;
            }
        }

        public virtual int PreviousState
        {
            get
            {
                return int.MaxValue;
            }
        }

        public int SubStateCount
        {
            get
            {
                return (mSubStateList != null) ? mSubStateList.Size : 0;
            }
        }

        public virtual bool IsValid
        {
            get
            {
                return true;
            }
        }

        public virtual bool IsDeinitLater
        {
            get;
            protected set;
        }

        public bool IsApplyAutoNext { get; set; } = false;
        public Action<IState, bool> StateFrameUpdater { get; set; }
        #endregion

        public FState(int name)
        {
            mStateName = name;
            mSubStateList = new KeyValueList<int, IState>();
            Init();
        }

        #region 销毁
        /// <summary>销毁</summary>
        public virtual void Dispose()
        {
            mIsStateActived = false;

            IState state = default;
            int max = mSubStateList.Size;
            var list = mSubStateList.Keys;
            for (int i = 0; i < max; i++)
            {
                state = mSubStateList.GetValue(list[i]);
                Utils.Reclaim(state);
            }
            Utils.Reclaim(mSubStateList);

            mIsInited = false;
            mSubState = default;
            mSubStateList = default;
            mFSM = default;
            mFSMName = int.MaxValue;
            mChangingState = default;
            mChangingStateParam = default;
            mSelfParam = default;
        }
        #endregion

        #region 初始化
        private void Init()
        {
            mIsInited = true;
            InitSubStates();
            CheckStateFrameUpdate();
        }

        /// <summary>初始化状态</summary>
        protected virtual void InitSubStates()
        {
            IState sub;
            int max = SubStateInfos.Length;
            for (int i = 0; i < max; i++)
            {
                sub = SubStateInfos[i];
                AddSubState(sub.StateName, sub);
            }
            Array.Clear(SubStateInfos, 0, SubStateInfos.Length);
        }
        #endregion

        public void SetFSMName(int FSMName)
        {
            int max = mSubStateList.Size;
            if (mFSM == default && max > 0)
            {
                int key;
                IState subState;
                List<int> keys = mSubStateList.Keys;
                for (int i = 0; i < max; i++)
                {
                    key = keys[i];
                    subState = mSubStateList[key];
                    subState.SetFSMName(FSMName);
                }
            }
            else { }

            mFSMName = FSMName;
        }

        #region 状态管理
        /// <summary>添加子状态</summary>
        public void AddSubState(int name, IState sub)
        {
            if (mSubStateList == default)
            {
                return;
            }
            else { }

            if (mSubStateList.IsContainsKey(name))
            {
#if !RELEASE
                "error:不能添加重复的状态 {0}".Log(name.ToString());
#endif
            }
            else
            {
                mSubStateList.Put(name, sub);
            }
            CheckStateFrameUpdate();
        }

        /// <summary>移除子状态</summary>
        public void RemoveSubState(int name)
        {
            if (mSubStateList == default)
            {
                return;
            }
            else { }

            if (mSubStateList.IsContainsKey(name))
            {
                IState state = mSubStateList.Remove(name);
                if (state == mSubState)
                {
                    ChangeSubStateToDefault();
                }
                else { }
                Utils.Reclaim(state);
            }
            else { }

            CheckStateFrameUpdate();
        }

        /// <summary>检测是否加入刷帧序列，用于子状态的切换</summary>
        private void CheckStateFrameUpdate()
        {
            if (mIsInited)
            {
                if (mSubStateList != default)
                {
                    bool isAdd = mSubStateList.Size > 0;
                    StateFrameUpdater?.Invoke(this, isAdd);
                }
                else { }
            }
            else { }
        }

        protected IState GetSubState(int name)
        {
            return mSubStateList?.GetValue(name);
        }

        protected IState GetSubStateByIndex(int index)
        {
            return mSubStateList?.GetValueByIndex(index);
        }

        #endregion

        #region 状态和子状态的激活、关闭、更新
        /// <summary>初始化状态</summary>
        public virtual void InitState(IStateParam param = default)
        {
            mIsStateActived = true;
            mSelfParam = param;
        }

        /// <summary>关闭状态</summary>
        public virtual void DeinitState()
        {
            mIsStateActived = false;
        }

        /// <summary>状态更新</summary>
        public virtual void UpdateState(int dTime)
        {
            if (!mIsStateActived)
            {
                return;
            }
            else { }
        }

        /// <summary>子状态更新</summary>
        protected virtual void UpdateSubState(int dTime)
        {
            mSubState?.UpdateState(dTime);
        }
        #endregion

        #region 状态更改
        /// <summary>更改至任意子状态，只有最后一次调用生效</summary>
        public virtual void ChangeSubState(int name, IStateParam param = default)
        {
            if (mSubStateList == default)
            {
                return;
            }
            else { }

            if ((mSubState != default) && (name == mSubState.StateName))
            {
                if (param != default)
                {
                    mSubState.SetStateParam(param);
                }
                else
                {
                    return;
                }
            }
            else { }

            if (mSubStateList.IsContainsKey(name))
            {
                mIsStateChanged = true;
                mChangingState = mSubStateList.GetValue(name);
                mChangingStateParam = param;
            }
            else
            {
                ChangeSubStateToDefault(param);
            }
        }

        /// <summary>更改至默认子状态</summary>
        public void ChangeSubStateToDefault(IStateParam param = default)
        {
            if (mSubStateList.IsContainsKey(DefaultState))
            {
                ChangeSubState(DefaultState, param);
            }
            else { }
        }

        /// <summary>将状态机更改至任意状态</summary>
        public virtual void ChangeToState(int name, IStateParam param = default)
        {
            if (mFSM != default)
            {
                mFSM.ChangeState(name, param);
            }
            else { }
        }

        /// <summary>将状态机更改至下一个状态</summary>
        public virtual void ChangeToNextState(IStateParam param = default)
        {
            if (mFSM != default)
            {
                mFSM.ChangeToNextState(param);
            }
            else { }
        }

        /// <summary>将状态机更改至上一个状态</summary>
        public virtual void ChangeToPreviousState(IStateParam param = default)
        {
            if (mFSM != default)
            {
                mFSM.ChangeToPreviousState(param);
            }
            else { }
        }

        /// <summary>将状态机更改至默认状态</summary>
        public virtual void ChangeToDefaultState(IStateParam param = default)
        {
            if (mFSM != default)
            {
                mFSM.ChangeToDefaultState(param);
            }
            else { }
        }

        public virtual void SetStateParam(IStateParam param)
        {
            mSelfParam = param;
        }
        #endregion
        
        public virtual void OnUpdate(int dTime)
        {
            if (mIsStateChanged)
            {
                if (mSubState != default)
                {
                    mSubState.DeinitState();
                }
                else { }

                mSubState = mChangingState;
                mSubState.InitState(mChangingStateParam);
                mChangingStateParam = default;
                mIsStateChanged = false;

                "fsm changed".Log(mSubState != default, StateName.ToString(), "sub state", mSubState.StateName.ToString());
            }
            else { }
        }

        public IStateMachine GetFSM()
        {
            return mFSM;
        }

        public void SetFSM(IStateMachine fsm)
        {
            if(mFSM == default)
            {
                mFSM = fsm;
            }
            else { }
        }
    }
}