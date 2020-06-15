using ShipDock.Notices;

namespace KLGame
{
    public interface IAIBehavioralInfo : INotificationSender
    {
        bool IsExecuted { get; set; }
        int StateFrom { get; set; }
        AIStateWillChange AIStateWillChange { get; set; }
    }
}