
using System;

namespace ShipDock.Server
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class ServerPriority : Attribute
    {
        public ServerPriority(int value)
        {
            Priority = value;
        }

        public int Priority { get; set; }
    }
}