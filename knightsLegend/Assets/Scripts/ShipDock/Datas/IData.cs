using ShipDock.Notices;

namespace ShipDock.Datas
{
    public interface IData
    {
        int DataName { get; }
    }

    public interface IDataExtracter
    {
        void OnDataChanged(IData data, int keyName);
    }
}
