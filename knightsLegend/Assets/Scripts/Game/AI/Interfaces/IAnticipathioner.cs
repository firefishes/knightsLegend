﻿using ShipDock.Notices;

namespace KLGame
{
    public interface IAnticipathioner : INotificationSender
    {
        bool IsExecuted { get; set; }
        int StateFrom { get; set; }
        AIStateWill AIStateWillChange { get; set; }
    }
}