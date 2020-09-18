﻿namespace ShipDock.Datas
{
    public interface IDataProxy
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
