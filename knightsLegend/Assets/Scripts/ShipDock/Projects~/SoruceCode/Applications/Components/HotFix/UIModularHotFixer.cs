using ShipDock.Datas;
using ShipDock.Notices;
using System;

namespace ShipDock.Applications
{
    public class UIModularHotFixer : UIModular<HotFixerUIAgent>
    {
        private HotFixerUI mBridge;

        public override int[] DataProxyLinks { get; set; }

        protected Func<HotFixerInteractor> UIInteracterCreater { get; set; }

        protected HotFixerUIAgent UIAgent
        {
            get
            {
                return UI;
            }
        }

        public override void Dispose()
        {
            base.Dispose();

            mBridge = default;
        }

        public override void OnDataProxyNotify(IDataProxy data, int keyName)
        {
        }

        protected override void UIModularHandler(INoticeBase<int> param)
        {
        }

        sealed public override void Init()
        {
            base.Init();

            HotFixerInteractor interacter = UIInteracterCreater?.Invoke();
            interacter.SetUIModular(this);

            mBridge = UIAgent.Bridge;
            mBridge.SetHotFixInteractor(interacter);

            ILRuntimeUtils.InvokeMethodILR(mBridge.HotFixerInteractor.UIModular, UIAgent.UIModularName, "AfterUIModularInit", 0);

        }

        sealed public override void Enter()
        {
            base.Enter();

            ILRuntimeUtils.InvokeMethodILR(mBridge.HotFixerInteractor.UIModular, UIAgent.UIModularName, "AfterUIModularEnter", 0);
        }

        sealed public override void Exit(bool isDestroy)
        {
            base.Exit(isDestroy);

            ILRuntimeUtils.InvokeMethodILR(mBridge.HotFixerInteractor.UIModular, UIAgent.UIModularName, "AfterUIModularExit", 1, isDestroy);
        }

        protected override void HideUI()
        {
            base.HideUI();

            ILRuntimeUtils.InvokeMethodILR(mBridge.HotFixerInteractor.UIModular, UIAgent.UIModularName, "AfterUIModularHide", 0);
        }

        protected override void ShowUI()
        {
            base.ShowUI();

            ILRuntimeUtils.InvokeMethodILR(mBridge.HotFixerInteractor.UIModular, UIAgent.UIModularName, "AfterUIModularShow", 0);
        }

        sealed public override void Renew()
        {
            base.Renew();

            ILRuntimeUtils.InvokeMethodILR(mBridge.HotFixerInteractor.UIModular, UIAgent.UIModularName, "AfterUIModularRenew", 0);
        }
    }
}