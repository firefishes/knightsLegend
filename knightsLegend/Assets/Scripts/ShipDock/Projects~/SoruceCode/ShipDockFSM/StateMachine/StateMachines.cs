using ShipDock.Tools;
using System;

namespace ShipDock.FSM
{
    /// <summary>
    /// 
    /// 状态机管理器单例
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class StateMachines
    {

        private KeyValueList<int, IStateMachine> mFSMMapper;

        public StateMachines()
        {
            mFSMMapper = new KeyValueList<int, IStateMachine>();
        }

        public void Dispose()
        {
            var list = mFSMMapper.Keys;
            IStateMachine fsm = default;
            int max = list.Count;
            for (int i = 0; i < max; i++)
            {
                fsm = mFSMMapper.GetValue(list[i]);
#if ILRUNTIME
                fsm?.Dispose();
#else
                Utils.Reclaim(fsm);
#endif
            }

#if ILRUNTIME
            mFSMMapper?.Dispose();
#else
            Utils.Reclaim(ref mFSMMapper);
#endif
        }

        /// <summary>注册状态机</summary>
        public void Register(IStateMachine fsm)
        {
            if (HasFSM(fsm.Name))
            {
                "error:不能注册重复的状态机 {0}".Log(fsm.GetType().Name);
            }
            else
            {
                "log:状态机已注册 {0}, name is {1}".Log(fsm.GetType().Name, fsm.Name.ToString());
                fsm.FSMFrameUpdater = FSMFrameUpdater;
                fsm.StateFrameUpdater = StateFrameUpdater;
                mFSMMapper.Put(fsm.Name, fsm);
            }
        }

        public IStateMachine GetFSM(int name)
        {
            return mFSMMapper.GetValue(name);
        }

        public T GetFSM<T>(int name)
        {
            IStateMachine fsm = GetFSM(name);
            return fsm != default ? (T)GetFSM(name) : default;
        }

        /// <summary>注销状态机</summary>
        public void Unregister(int name, bool isDispose = false)
        {
            StateMachine fsm = mFSMMapper.Remove(name) as StateMachine;
            if (isDispose && (fsm != default))
            {
                fsm.Dispose();
            }
            else { }
        }

        public bool HasFSM(int name)
        {
            return mFSMMapper.IsContainsKey(name);
        }

        public Action<IStateMachine, bool> FSMFrameUpdater { get; set; }
        public Action<IState, bool> StateFrameUpdater { get; set; }
    }

}