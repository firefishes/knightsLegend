
using System;
using UnityEngine;

namespace ShipDock.Loader
{
    [Serializable]
    public class CustomAsset
    {
        public bool refresh;
        public string assetName;
        public GameObject asset;
        public Texture2D tex2D;
        public Sprite sprite;
        public AudioClip audioClip;
        public TextAsset textData;
        public AssetBundle assetBundle;

        public void UpdateCustomAssetName()
        {
            if (asset != default)
            {
                assetName = asset.name;
            }
        }

        public T GetAsset<T>() where T : UnityEngine.Object
        {
            T result = default;
            if (typeof(T) == typeof(GameObject))
            {
                result = asset as T;
            }
            else if (typeof(T) == typeof(Texture2D))
            {
                result = tex2D as T;
            }
            else if (typeof(T) == typeof(AudioClip))
            {
                result = audioClip as T;
            }
            else if (typeof(T) == typeof(Sprite))
            {
                result = sprite as T;
            }
            else if(typeof(T) == typeof(TextAsset))
            {
                result = textData as T;
            }
            else if(typeof(T) == typeof(AssetBundle))
            {
                result = assetBundle as T;
            }
            else
            {
                //result = assetBundle as T;
            }
            return result;
        }

        public void SyncFromInfo(ref CustomAssetInfo assetInfo)
        {
            asset = assetInfo.asset;
            tex2D = assetInfo.tex2D;
            audioClip = assetInfo.audioClip;
            sprite = assetInfo.sprite;
            textData = assetInfo.textData;
            assetBundle = assetInfo.assetBundle;
            assetName = assetInfo.assetName;

            UpdateCustomAssetName();
        }

        public void WriteToInfo(ref CustomAssetInfo assetInfo)
        {
            assetInfo.assetName = assetName;
            assetInfo.asset = asset;
            assetInfo.tex2D = tex2D;
            assetInfo.audioClip = audioClip;
            assetInfo.sprite = sprite;
            assetInfo.textData = textData;
            assetInfo.assetBundle = assetBundle;
        }
    }

}