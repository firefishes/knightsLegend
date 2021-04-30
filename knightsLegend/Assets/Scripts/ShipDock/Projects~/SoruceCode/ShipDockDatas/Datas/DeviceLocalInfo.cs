using System;

namespace ShipDock.Datas
{
    [Serializable]
    public class DeviceLocalInfo
    {
        public bool has_allowed_microphone;
        public bool has_allowed_camera;
        public bool has_agree_privacy;

        public DeviceLocalInfo() { }
    }
}