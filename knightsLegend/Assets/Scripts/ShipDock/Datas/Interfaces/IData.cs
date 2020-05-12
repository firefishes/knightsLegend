namespace ShipDock.Datas
{
    public interface IData
    {
        void Register(IDataExtracter dataHandler);
        int DataName { get; }
    }

    public interface IDataExtracter
    {
        void OnDataChanged(IData data, int keyName);
    }
}
