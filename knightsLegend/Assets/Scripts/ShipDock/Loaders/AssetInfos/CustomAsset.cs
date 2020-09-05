
using System;
using UnityEngine;

namespace ShipDock.Loader
{
    [Serializable]
    public class CustomAsset
    {
        public string assetName;
        public GameObject asset;
        public Texture2D tex2D;
        public Sprite sprite;
        public AudioClip audioClip;
        public TextAsset textData;
        public AssetBundle assetBundle;
        public bool refresh;

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
    }

}