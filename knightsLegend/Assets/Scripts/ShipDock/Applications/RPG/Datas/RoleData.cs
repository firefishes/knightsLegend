using System;

namespace ShipDock.Applications
{
    [Serializable]
    public struct RoleData : IRoleData
    {
        public int ConfigID { get; set; }
        public float Hp { get; set; }
        public float Speed { get; set; }
        public float JumpPower { get; set; }
        public float GravityMultiplier { get; set; }
        public float StationaryTurnSpeed { get; set; }
        public float MovingTurnSpeed { get; set; }
    }
}
