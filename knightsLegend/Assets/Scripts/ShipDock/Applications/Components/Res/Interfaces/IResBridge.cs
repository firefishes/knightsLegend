using UnityEngine;

namespace ShipDock.Applications
{
    public interface IResBridge
    {
        void SetPoolID(int id);
    }

    public interface IResPrefabBridge : IResBridge
    {
        GameObject Prefab { get; }
    }

    public interface IResTextureBridge : IResBridge
    {
        Texture Texture { get; }
    }

    public interface IResSpriteBridge : IResTextureBridge
    {
        Sprite Sprite { get; }
    }
}
