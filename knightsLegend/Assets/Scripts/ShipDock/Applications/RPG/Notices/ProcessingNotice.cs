using ShipDock.Notices;

namespace ShipDock.Applications
{
    public class ProcessingNotice : Notice
    {
        public void Reinit(int colliderID, int processingType, ProcessingHitInfo hitInfo)
        {
            HitColliderID = colliderID;
            ProcessingType = processingType;
            HitInfo = hitInfo;
        }

        public void Commit(INotificationSender sender)
        {
            NotifcationSender = sender;
            ShipDockApp.Instance.Notificater.Broadcast(this);
        }

        public ProcessingHitInfo HitInfo { get; set; }
        public int HitColliderID { get; set; }
        public int ProcessingType { set; get; }
    }
}