#define G_LOG

using ShipDock.Testers;
using ShipDock.Tools;
using System;

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
            mSubState = null;
            mSubStateList = null;
            mFSM = null;
            mFSMName = int.MaxValue;
            mChangingState = null;
            mChangingStateParam = null;
            mSelfParam = null;
        }
        #endregion

        #region 初始化
        private void Init()
        {
            InitSubStates();
            mIsInited = true;
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
            if (mFSM == null && max > 0)
            {
                IState subState;
                for (int i = 0; i < max; i++)
                {
                    subState = mSubStateList[mSubStateList.Keys[i]];
                    subState.SetFSMName(FSMName);
                }
            }
            mFSMName = FSMName;
        }

        #region 状态管理
        /// <summary>添加子状态</summary>
        public void AddSubState(int name, IState sub)
        {
            if (mSubStateList == null)
            {
                return;
            }

            if (!mSubStateList.IsContainsKey(name))
            {
                mSubStateList.Put(name, sub);
            }
#if !RELEASE
            else
            {
                //DebugUtils.LogInColor("error: 不能添加重复的状态 ", name.ToString());
            }
#endif
            CheckStateFrameUpdate();
        }

        /// <summary>移除子状态</summary>
        public void RemoveSubState(int name)
        {
            if (mSubStateList == null)
            {
                return;
            }

            if (mSubStateList.IsContainsKey(name))
            {
                IState state = mSubStateList.Remove(name);
                if (state == mSubState)
                {
                    ChangeSubStateToDefault();
                }
                Utils.Reclaim(state);
            }
            CheckStateFrameUpdate();
        }

        /// <summary>检测是否加入刷帧序列，用于子状态的切换</summary>
        private void CheckStateFrameUpdate()
        {
            if (!mIsInited)
            {
                return;
            }

            if (mSubStateList != null)
            {
                bool isAdd = (mSubStateList.Size > 0);
                StateFrameUpdater?.Invoke(this, isAdd);
            }
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
        public virtual void InitState(IStateParam param = null)
        {
            mIsStateActived = true;
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
        }

        /// <summary>子状态更新</summary>
        protected virtual void UpdateSubState(int dTime)
        {
            mSubState?.UpdateState(dTime);
        }
        #endregion

        #region 状态更改
        /// <summary>更改至任意子状态，只有最后一次调用生效</summary>
        public virtual void ChangeSubState(int name, IStateParam param = null)
        {
            if (mSubStateList == null)
            {
                return;
            }

            if ((mSubState != null) && (name == mSubState.StateName))
            {
                if (param != null)
                {
                    mSubState.SetStateParam(param);
                }
                else
                {
                    return;
                }
            }

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
        public void ChangeSubStateToDefault(IStateParam param = null)
        {
            if (!mSubStateList.IsContainsKey(DefaultState))
            {
                return;
            }

            ChangeSubState(DefaultState, param);
        }

        /// <summary>将状态机更改至任意状态</summary>
        public virtual void ChangeToState(int name, IStateParam param = null)
        {
            if (mFSM != null)
            {
                mFSM.ChangeState(name, param);
            }
        }

        /// <summary>将状态机更改至下一个状态</summary>
        public virtual void ChangeToNextState(IStateParam param = null)
        {
            if (mFSM != null)
            {
                mFSM.ChangeToNextState(param);
            }
        }

        /// <summary>将状态机更改至上一个状态</summary>
        public virtual void ChangeToPreviousState(IStateParam param = null)
        {
            if (mFSM != null)
            {
                mFSM.ChangeToPreviousState(param);
            }
        }

        /// <summary>将状态机更改至默认状态</summary>
        public virtual void ChangeToDefaultState(IStateParam param = null)
        {
            if (mFSM != null)
            {
                mFSM.ChangeToDefaultState(param);
            }
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
                if (mSubState != null)
                {
                    mSubState.DeinitState();
                }
                mSubState = mChangingState;
                mSubState.InitState(mChangingStateParam);
                mChangingStateParam = null;
                mIsStateChanged = false;

                Tester.Instance.Log(TesterBaseApp.Instance, TesterBaseApp.LOG0, mSubState != null, GetType().Name, " => ", mSubState.StateName.ToString());
            }
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
        }

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
    }
}