using ShipDock.Interfaces;

namespace ShipDock.Datas
{
    public interface IDataProxy : IDispose
    {
        void Register(IDataExtracter dataHandler);
        void Unregister(IDataExtracter dataHandler);
        int DataName { get; }
    }

    public interface IDataExtracter
    {
        void OnDataProxyNotify(IDataProxy data, int DCName);
    }
}
