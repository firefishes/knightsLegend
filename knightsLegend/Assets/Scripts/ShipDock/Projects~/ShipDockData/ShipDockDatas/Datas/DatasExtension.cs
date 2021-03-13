using ShipDock;
using ShipDock.Datas;
using ShipDock.Tools;

public static class DatasExtension
{
    public static void AddToWarehouse(this IDataProxy target)
    {
        DataWarehouse datas = Framework.Instance.GetUnit<DataWarehouse>(Framework.UNIT_DATA);
        datas.AddData(target);
    }

    public static T GetData<T>(this int target) where T : IDataProxy
    {
        DataWarehouse datas = Framework.Instance.GetUnit<DataWarehouse>(Framework.UNIT_DATA);
        return datas.GetData<T>(target);
    }

    public static KeyValueList<int, IDataProxy> DataProxyLink(this IDataExtracter target, params int[] dataNames)
    {
        IDataProxy proxy;
        int name;
        int max = dataNames != default ? dataNames.Length : 0;
        DataWarehouse datas = Framework.Instance.GetUnit<DataWarehouse>(Framework.UNIT_DATA);
        KeyValueList<int, IDataProxy> result = new KeyValueList<int, IDataProxy>();
        for (int i = 0; i < max; i++)
        {
            name = dataNames[i];
            proxy = datas.GetData<IDataProxy>(name);
            proxy.Register(target);
            result[name] = proxy;
        }
        return result;
    }

    public static void DataProxyDelink(this IDataExtracter target, params int[] dataNames)
    {
        IDataProxy proxy;
        DataWarehouse datas = Framework.Instance.GetUnit<DataWarehouse>(Framework.UNIT_DATA);
        int max = dataNames == default ? 0 : dataNames.Length;
        for (int i = 0; i < max; i++)
        {
            proxy = datas.GetData<IDataProxy>(dataNames[i]);
            proxy.Unregister(target);
        }
    }

}