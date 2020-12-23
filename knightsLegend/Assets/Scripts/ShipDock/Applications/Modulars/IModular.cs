using ShipDock.Notices;

namespace ShipDock.Applications
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
