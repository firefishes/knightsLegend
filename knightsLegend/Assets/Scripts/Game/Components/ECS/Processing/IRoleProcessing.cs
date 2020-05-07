using ShipDock.Applications;

namespace KLGame
{
    public interface IRoleProcessing : IProcessing
    {
        IKLRole Initiator { get; set; }
        IKLRole Target { get; set; }
    }

}