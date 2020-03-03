namespace ShipDock.Applications
{
    public interface IRoleData
    {
        int ConfigID { get; set; }
        float Hp { get; set; }
        float Speed { get; set; }
        float JumpPower { get; set; }
        float GravityMultiplier { get; set; }
        float StationaryTurnSpeed { get; set; }
        float MovingTurnSpeed { get; set; }
    }
}