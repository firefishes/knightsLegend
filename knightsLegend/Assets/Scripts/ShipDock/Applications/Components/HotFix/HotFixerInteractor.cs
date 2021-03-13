using ShipDock.Notices;
using System;

namespace ShipDock.Applications
{
    /// <summary>
    /// UI交互器抽象类，用于容纳热更端的 UI 逻辑
    /// </summary>
    public abstract class HotFixerInteractor
    {
        public HotFixerUI UI { get; private set; }
        public HotFixerUIAgent Agent { get; private set; }
        public UIModularHotFixer UIModular { get; private set; }

        public virtual void Release()
        {
            UI = default;
            Agent = default;
            UIModular = default;
        }

        public void SetUIModular(UIModularHotFixer modular)
        {
            UIModular = modular;
        }

        public virtual void InitInteractor(HotFixerUI UIOwner, HotFixerUIAgent agent)
        {
            UI = UIOwner;
            Agent = agent;
        }

        public virtual void Dispatch(int name, INoticeBase<int> param = default)
        {
            Agent.Dispatch(name, param);
        }

        public abstract void UpdateInteractor();
    }
}