using ShipDock.Interfaces;
using UnityEngine;

namespace ShipDock.Loader
{
    public interface IAssetBundleInfo : IDispose
    {
        int LoadCount { get; set; }
        T GetAsset<T>(string path) where T : Object;
        GameObject GetAsset(string path);
        AssetBundle Asset { get; }
        string Name { get; }
    }
}

