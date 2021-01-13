using ShipDock.Tools;

namespace ShipDock.Config
{
    public interface IConfig
    {
        int GetID();
        void Parse(ByteBuffer buffer);
        string CRCValue { get; }
    }
}