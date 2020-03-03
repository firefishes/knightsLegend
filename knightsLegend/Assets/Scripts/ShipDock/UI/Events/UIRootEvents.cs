using System;
using UnityEngine.Events;

namespace ShipDock.UI
{
    [Serializable]
    public class OnUIRootAwaked : UnityEvent<IUIRoot> { };
}