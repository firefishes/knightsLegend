using ShipDock.Interfaces;

namespace ShipDock.Loader
{
    public interface ICustomAssetBundle : IDispose
    {
        T GetCustomAsset<T>(string name, string path) where T : UnityEngine.Object;
    }
}
