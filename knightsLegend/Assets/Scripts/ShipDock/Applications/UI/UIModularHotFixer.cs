using ShipDock.Datas;
using ShipDock.Notices;

namespace ShipDock.Applications
{
    public class UIModularHotFixer : UIModular<HotFixerUIAgent>
    {
        private HotFixerUI mBridge;

        public override int[] DataProxyLinks { get; } = new int[0];

        protected HotFixerUIAgent UIAgent
        {
            get
            {
                return UI;
            }
        }

        public UIModularHotFixer() { }

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

        public override void Init()
        {
            base.Init();

            mBridge = UIAgent.Bridge;
            mBridge.SetHotFixInteractor(GetInteractor());
        }

        protected virtual HotFixerInteractor GetInteractor()
        {
            return default;
        }
    }
}