using System;
using UnityEngine;

namespace ShipDock.Loader
{
    [Serializable]
    public class CustomAssetInfo
    {
        public string assetName;
        public GameObject asset;
        public Texture2D tex2D;
        public Sprite sprite;
        public AudioClip audioClip;
        public TextAsset textData;
        public AssetBundle assetBundle; 
    }

}