
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System;
using UnityEngine;

namespace ShipDock.Loader
{
    public enum CustomAssetType
    {
        GAME_OBJECT,
        TEXTURE_2D,
        SPRITE,
        AUDIO_CLIP,
        TEXT_ASSET,
        ASSET_BUNDLE,
    }

    [Serializable]
    public class CustomAsset
    {
        public CustomAssetType customAssetType;
#if ODIN_INSPECTOR
        [Indent(1)]
#endif
        public bool refresh;
#if ODIN_INSPECTOR
        [Indent(1)]
#endif
        public string assetName;
#if ODIN_INSPECTOR
        [ShowIf("customAssetType", CustomAssetType.GAME_OBJECT)]
        [Indent(1)]
#endif
        public GameObject asset;
#if ODIN_INSPECTOR
        [ShowIf("customAssetType", CustomAssetType.TEXTURE_2D)]
        [Indent(1)]
#endif
        public Texture2D tex2D;
#if ODIN_INSPECTOR
        [ShowIf("customAssetType", CustomAssetType.SPRITE)]
        [Indent(1)]
#endif
        public Sprite sprite;
#if ODIN_INSPECTOR
        [ShowIf("customAssetType", CustomAssetType.AUDIO_CLIP)]
        [Indent(1)]
#endif
        public AudioClip audioClip;
#if ODIN_INSPECTOR
        [ShowIf("customAssetType", CustomAssetType.TEXT_ASSET)]
        [Indent(1)]
#endif
        public TextAsset textData;
#if ODIN_INSPECTOR
        [ShowIf("customAssetType", CustomAssetType.ASSET_BUNDLE)]
        [Indent(1)]
#endif
        public AssetBundle assetBundle;

        public void UpdateCustomAssetName()
        {
            if (asset != default)
            {
                assetName = asset.name;
            }
            else if (textData != default)
            {
                assetName = textData.name;
            }
            else if (sprite != default)
            {
                assetName = sprite.name;
            }
            else if (audioClip != default)
            {
                assetName = audioClip.name;
            }
            else { }
        }

        public void SyncCustomAssetType()
        {
            if (asset != default)
            {
                customAssetType = CustomAssetType.GAME_OBJECT;
            }
            else if (tex2D != default)
            {
                customAssetType = CustomAssetType.TEXTURE_2D;
            }
            else if (sprite != default)
            {
                customAssetType = CustomAssetType.SPRITE;
            }
            else if (audioClip != default)
            {
                customAssetType = CustomAssetType.AUDIO_CLIP;
            }
            else if (textData != default)
            {
                customAssetType = CustomAssetType.TEXT_ASSET;
            }
            else if (assetBundle != default)
            {
                customAssetType = CustomAssetType.ASSET_BUNDLE;
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