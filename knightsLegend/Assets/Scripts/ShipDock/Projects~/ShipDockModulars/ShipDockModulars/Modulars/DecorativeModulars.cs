using ShipDock.Notices;
using ShipDock.Tools;
using System;

namespace ShipDock.Modulars
{
    public class DecorativeModulars : IAppModulars
    {
        private KeyValueList<int, IModular> mModulars;
        private KeyValueList<int, Func<int, INoticeBase<int>>> mNoticeCreaters;
        private KeyValueList<int, Action<int, INoticeBase<int>>> mNoticeDecorator;

        public DecorativeModulars()
        {
            mModulars = new KeyValueList<int, IModular>();
            mNoticeCreaters = new KeyValueList<int, Func<int, INoticeBase<int>>>();
            mNoticeDecorator = new KeyValueList<int, Action<int, INoticeBase<int>>>();
        }

        public void Dispose()
        {
            mModulars?.Dispose();
            mNoticeCreaters?.Dispose();
            mNoticeDecorator?.Dispose();
        }

        public void AddNoticeDecorator(int noticeName, Action<int, INoticeBase<int>> method)
        {
            if (mNoticeDecorator.ContainsKey(noticeName))
            {
                "log".Log(noticeName.ToString().Append(" append handler "));
                mNoticeDecorator[noticeName] += method;
            }
            else
            {
                "log".Log(noticeName.ToString().Append(" add decorator "));
                mNoticeDecorator[noticeName] = default;
                mNoticeDecorator[noticeName] += method;
            }
        }

        public void RemoveNoticeDecorator(int noticeName, Action<int, INoticeBase<int>> method)
        {
            if (mNoticeDecorator.ContainsKey(noticeName))
            {
                "log".Log(noticeName.ToString().Append(" remove decorator "));
                mNoticeDecorator[noticeName] -= method;
            }
        }

        public void AddNoticeCreater(int noticeName, Func<int, INoticeBase<int>> method)
        {
            "error".Log(mNoticeCreaters.ContainsKey(noticeName), string.Format("{0}'s creater is existed..", noticeName));
            if (!mNoticeCreaters.ContainsKey(noticeName))
            {
                "Creater {0} added.".Log(noticeName.ToString());
                mNoticeCreaters[noticeName] = method;
            }
        }

        public void RemoveNoticeCreater(int noticeName, Func<int, INoticeBase<int>> method)
        {
            if (mNoticeCreaters.ContainsKey(noticeName))
            {
                "Creater {0} removed.".Log(noticeName.ToString());
                mNoticeCreaters.Remove(noticeName);
            }
        }

        public void AddModular(params IModular[] modulars)
        {
            IModular modular;
            int max = modulars.Length;
            for (int i = 0; i < max; i++)
            {
                modular = modulars[i];
                if (mModulars.ContainsKey(modular.ModularName))
                {
                    "error".Log(modular.ModularName.ToString().Append(" modular is existed"));
                    modular.Dispose();
                }
                else
                {
                    "Modular {0} is create".Log(modular.ModularName.ToString());
                    mModulars[modular.ModularName] = modular;
                    modular.SetModularManager(this);
                    modular.InitModular();
                }
            }
        }

        public INoticeBase<int> NotifyModular(int name, INoticeBase<int> param = default)
        {
            Func<int, INoticeBase<int>> func = mNoticeCreaters[name];
            bool applyCreater = param == default;
            INoticeBase<int> notice = applyCreater ? func?.Invoke(name) : param;

            "error".Log(param == default && func == default, "Notice creater is null..".Append(" notice = ", name.ToString()));
            "warning".Log(notice == default, "Brocast notice is null..".Append(" notice = ", name.ToString()));
            "warning".Log(!mNoticeDecorator.ContainsKey(name), string.Format("Notice {0} decorator is empty..", name));

            if (notice != default)
            {
                mNoticeDecorator[name]?.Invoke(name, notice);
                name.Broadcast(notice);
                "log".Log(string.Format("Modular App brodcast {0}", name.ToString()));
            }
            else
            {
                "log".Log("Notify modular by default notice");
                name.Broadcast(notice);
            }
            return notice;
        }
    }
}
