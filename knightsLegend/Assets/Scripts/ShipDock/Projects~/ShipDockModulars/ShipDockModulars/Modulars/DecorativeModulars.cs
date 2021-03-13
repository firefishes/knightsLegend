using ShipDock.Notices;
using ShipDock.Tools;
using System;

namespace ShipDock.Modulars
{
    /// <summary>
    /// 装饰化模块管理器
    /// </summary>
    public class DecorativeModulars : IAppModulars
    {
        /// <summary>所有模块</summary>
        private KeyValueList<int, IModular> mModulars;
        /// <summary>消息生成器函数的映射</summary>
        private KeyValueList<int, Func<int, INoticeBase<int>>> mNoticeCreaters;
        /// <summary>装饰器函数的映射</summary>
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

        /// <summary>
        /// 添加消息装饰器函数
        /// </summary>
        /// <param name="noticeName">消息名</param>
        /// <param name="method">装饰器函数</param>
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

        /// <summary>
        /// 移除消息装饰器函数
        /// </summary>
        /// <param name="noticeName"></param>
        /// <param name="method"></param>
        public void RemoveNoticeDecorator(int noticeName, Action<int, INoticeBase<int>> method)
        {
            if (mNoticeDecorator.ContainsKey(noticeName))
            {
                "log".Log(noticeName.ToString().Append(" remove decorator "));
                mNoticeDecorator[noticeName] -= method;
            }
        }

        /// <summary>
        /// 添加消息生成器函数
        /// </summary>
        /// <param name="noticeName"></param>
        /// <param name="method"></param>
        public void AddNoticeCreater(int noticeName, Func<int, INoticeBase<int>> method)
        {
            "error".Log(mNoticeCreaters.ContainsKey(noticeName), string.Format("{0}'s creater is existed..", noticeName));
            if (!mNoticeCreaters.ContainsKey(noticeName))
            {
                "Creater {0} added.".Log(noticeName.ToString());
                mNoticeCreaters[noticeName] = method;
            }
        }

        /// <summary>
        /// 移除消息生成器函数
        /// </summary>
        /// <param name="noticeName"></param>
        /// <param name="method"></param>
        public void RemoveNoticeCreater(int noticeName, Func<int, INoticeBase<int>> method)
        {
            if (mNoticeCreaters.ContainsKey(noticeName))
            {
                "Creater {0} removed.".Log(noticeName.ToString());
                mNoticeCreaters.Remove(noticeName);
            }
        }

        /// <summary>
        /// 添加模块
        /// </summary>
        /// <param name="modulars"></param>
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

        /// <summary>
        /// 发送模块消息
        /// </summary>
        /// <param name="noticeName">消息名</param>
        /// <param name="param">消息体对象，如不为空，则不调用对应的生成器函数</param>
        /// <returns></returns>
        public INoticeBase<int> NotifyModular(int noticeName, INoticeBase<int> param = default)
        {
            Func<int, INoticeBase<int>> creater = mNoticeCreaters[noticeName];
            bool applyCreater = param == default;
            INoticeBase<int> notice = applyCreater ? creater?.Invoke(noticeName) : param;//调用消息体对象生成器函数

            #region Logs
            "error".Log(param == default && creater == default, "Notice creater is null..".Append(" notice = ", noticeName.ToString()));
            "warning".Log(notice == default, "Brocast notice is null..".Append(" notice = ", noticeName.ToString()));
            "warning".Log(!mNoticeDecorator.ContainsKey(noticeName), string.Format("Notice {0} decorator is empty..", noticeName));
            #endregion

            if (notice != default)
            {
                Action<int, INoticeBase<int>> decorator = mNoticeDecorator[noticeName];
                decorator?.Invoke(noticeName, notice);//调用消息体装饰器函数
                noticeName.Broadcast(notice);//广播模块装饰后的消息
                "log".Log(string.Format("Modular App brodcast {0}", noticeName.ToString()));
            }
            else
            {
                "log".Log("Notify modular by default notice");
                noticeName.Broadcast(notice);//广播普通消息
            }
            return notice;
        }
    }
}
