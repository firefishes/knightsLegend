namespace ShipDock.Datas
{
    public interface IDataProxy
    {
        void Register(IDataExtracter dataHandler);
        int DataName { get; }
    }

    public interface IDataExtracter
    {
        void OnDataProxyNotify(IDataProxy data, int keyName);
    }
}
