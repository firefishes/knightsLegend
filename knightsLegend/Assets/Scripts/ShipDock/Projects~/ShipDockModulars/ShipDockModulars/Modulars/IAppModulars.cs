using ShipDock.Notices;
using System;

namespace ShipDock.Modulars
{
    public interface IAppModulars
    {
        void AddNoticeCreater(int noticeName, Func<int, INoticeBase<int>> method);
        void RemoveNoticeCreater(int noticeName, Func<int, INoticeBase<int>> method);
        void AddNoticeDecorator(int noticeName, Action<int, INoticeBase<int>> method);
        void RemoveNoticeDecorator(int noticeName, Action<int, INoticeBase<int>> method);
        INoticeBase<int> NotifyModular(int name, INoticeBase<int> param = default);
    }
}