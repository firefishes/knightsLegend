﻿using ShipDock.Interfaces;
using ShipDock.Notices;

namespace ShipDock.UI
{
    public interface IUIModular : INotificationSender, IUIStack, IDispose
    {
        int[] DataProxyLinks { get; }
        int UILayer { get; }
    }
}

