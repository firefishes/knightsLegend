using ShipDock.Interfaces;
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
    public class StateMachines : IDispose
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
                Utils.Reclaim(fsm);
            }
            Utils.Reclaim(ref mFSMMapper);
        }

        /// <summary>注册状态机</summary>
        public void Register(IStateMachine fsm)
        {
            if (!HasFSM(fsm.Name))
            {
                fsm.FSMFrameUpdater = FSMFrameUpdater;
                fsm.StateFrameUpdater = StateFrameUpdater;
                mFSMMapper.Put(fsm.Name, fsm);
            }
            else
            {
                Testers.Tester.Instance.Log(
                    Testers.TesterBaseApp.Instance, 
                    Testers.TesterBaseApp.LOG, 
                    typeof(StateMachines).ToString(), "不能注册重复的状态机");
            }
        }

        public IStateMachine GetFSM(int name)
        {
            return mFSMMapper.GetValue(name);
        }

        public T GetFSM<T>(int name)
        {
            return (T)GetFSM(name);
        }

        /// <summary>注销状态机</summary>
        public void Unregister(int name, bool isDispose = false)
        {
            IStateMachine fsm = mFSMMapper.Remove(name);
            if (isDispose && (fsm != null))
            {
                fsm.Dispose();
            }
        }

        public bool HasFSM(int name)
        {
            return mFSMMapper.IsContainsKey(name);
        }

        public Action<IStateMachine, bool> FSMFrameUpdater { get; set; }
        public Action<IState, bool> StateFrameUpdater { get; set; }
    }

}