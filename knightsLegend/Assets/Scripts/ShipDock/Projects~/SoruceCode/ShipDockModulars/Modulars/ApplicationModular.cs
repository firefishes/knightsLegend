using ShipDock.Notices;

namespace ShipDock.Modulars
{
    public abstract class ApplicationModular : IModular
    {
        public virtual int[] ModularNoticeCreate { get; }
        public virtual int[] ModularNoticeDecorater { get; }
        public virtual int[] ModularNoticeListener { get; }
        public virtual int ModularName { get; protected set; }

        protected virtual IAppModulars Modulars { get; set; }

        public virtual void Dispose()
        {
            int noticeName;
            int[] list = ModularNoticeCreate;
            int max = list != default ? list.Length : 0;
            for (int i = 0; i < max; i++)
            {
                noticeName = list[i];
                Modulars.RemoveNoticeCreater(noticeName, NoticeCreater);
            }
            list = ModularNoticeDecorater;
            max = list != default ? list.Length : 0;
            for (int i = 0; i < max; i++)
            {
                noticeName = list[i];
                Modulars.RemoveNoticeDecorator(noticeName, NoticeDecorator);
            }
            list = ModularNoticeListener;
            max = list != default ? list.Length : 0;
            for (int i = 0; i < max; i++)
            {
                noticeName = list[i];
                noticeName.Remove(NoticesHandler);
            }

            Purge();

            Modulars = default;
        }

        public virtual void InitModular()
        {
            int noticeName;
            int[] list = ModularNoticeCreate;
            int max = list != default ? list.Length : 0;
            for (int i = 0; i < max; i++)
            {
                noticeName = list[i];
                Modulars.AddNoticeCreater(noticeName, NoticeCreater);
            }
            list = ModularNoticeDecorater;
            max = list != default ? list.Length : 0;
            for (int i = 0; i < max; i++)
            {
                noticeName = list[i];
                Modulars.AddNoticeDecorator(noticeName, NoticeDecorator);
            }
            list = ModularNoticeListener;
            max = list != default ? list.Length : 0;
            for (int i = 0; i < max; i++)
            {
                noticeName = list[i];
                noticeName.Add(NoticesHandler);
            }
        }

        public abstract void Purge();

        protected virtual void NoticesHandler(INoticeBase<int> param) { }

        protected virtual INoticeBase<int> NoticeCreater(int name) { return default; }

        protected virtual void NoticeDecorator(int noticeName, INoticeBase<int> param) { }

        public virtual INoticeBase<int> NotifyModular(int name, INoticeBase<int> param = default)
        {
            return Modulars != default ? Modulars.NotifyModular(name, param) : default;
        }

        public virtual void SetModularManager(IAppModulars modulars)
        {
            Modulars = modulars;
        }
    }
}
