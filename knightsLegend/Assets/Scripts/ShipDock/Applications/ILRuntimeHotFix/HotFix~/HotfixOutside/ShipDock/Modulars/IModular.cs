using ShipDock.Notices;

namespace ShipDock.HotFix
{
    public interface IModular
    {
        void SetModularManager(IAppModulars modulars);
        int ModularName { get; }
        void Dispose();
        void InitModular();
        INoticeBase<int> NotifyModular(int name, INoticeBase<int> param = default);
    }
}
