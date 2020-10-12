namespace ShipDock.Applications
{
    public interface IConfig
    {
        int GetID();
        void Parse(ByteBuffer buffer);
        string CRCValue { get; }
    }
}