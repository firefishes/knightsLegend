
using ShipDock.Pooling;

namespace ShipDock.Notices
{
    /// <summary>
    /// 
    /// 消息类
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class Notice : INotice, IPoolable
    {

        public Notice() { }

        public Notice(int name) : this(name, -1) { }

        public Notice(int name, int subCommand = -1)
        {
            SetNoticeName(name);
        }

        public void Reinit(int name)
        {
            SetNoticeName(name);
        }

        public virtual void ToPool()
        {
            Pooling<Notice>.To(this);
        }
        
        public virtual void Dispose()
        {
            Purge();
        }
        
        protected virtual void Purge()
        {
            IsRecivedNotice = false;
        }
        
        public virtual void Revert()
        {
            Purge();
        }

        public void SetNoticeName(int value)
        {
            Name = value;
        }

        public bool CheckSender(INotificationSender target)
        {
            return target == NotifcationSender;
        }

        public bool IsRecivedNotice { get; set; }
        public virtual int Name { get; private set; }
        public INotificationSender NotifcationSender { get; set; }
    }
}