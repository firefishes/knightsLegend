using ShipDock.Notices;
using ShipDock.Pooling;

namespace ShipDock.Applications
{
    public class ProcessingNotice : Notice
    {
        public override void ToPool()
        {
            Pooling<ProcessingNotice>.To(this);
        }

        public void Reinit(int colliderID, int processingType, ProcessingHitInfo hitInfo)
        {
            HitColliderID = colliderID;
            ProcessingType = processingType;
            HitInfo = hitInfo;
        }
        
        public ProcessingHitInfo HitInfo { get; set; }
        public int HitColliderID { get; set; }
        public int ProcessingType { set; get; }
    }
}