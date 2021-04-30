
using ShipDock.Interfaces;
using ShipDock.Pooling;

namespace ShipDock.Notices
{
    /// <summary>
    /// 
    /// 消息对象接口
    /// 
    /// add by Minghua.ji
    /// 
    /// 用于在发送消息时向消息回调函数或命令对象传递参数
    /// 
    /// </summary>
    public interface INoticeBase<NameT> : IDispose
    {
        /// <summary>设置消息名</summary>
        void SetNoticeName(NameT name);
        /// <summary>获取和设置消息发送者</summary>
        INotificationSender NotifcationSender { get; set; }
        /// <summary>消息名，用于直接广播消息时识别消息名</summary>
        NameT Name { get; }
        /// <summary>消息处理函数是否已处理</summary>
        bool IsRecivedNotice { get; set; }
    }

    public interface INotice : INoticeBase<int>, IPoolable
    {
    }

}
