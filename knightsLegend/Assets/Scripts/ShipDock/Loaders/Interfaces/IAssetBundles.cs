using UnityEngine;

namespace ShipDock.Loader
{
    public interface IAssetBundles
    {
        T Get<T>(string name, string path) where T : Object;
        GameObject Get(string name, string path);
    }

}
