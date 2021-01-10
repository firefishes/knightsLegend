using ShipDock.Notices;

namespace ShipDock.Modulars
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
