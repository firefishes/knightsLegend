using UnityEngine;

namespace ShipDock.Applications
{
    public class ResSpriteBridge : ResBridge, IResSpriteBridge
    {
        private Sprite mSprite;

        protected override void Awake()
        {
            base.Awake();

            if (m_IsCreateInAwake)
            {
                CreateRaw();
            }
            else { }
        }

        public void CreateRaw()
        {
            if ((m_Asset != default) && (Sprite == default))
            {
                if (Assets == default)
                {
                    Assets = ShipDockApp.Instance.ABs;
                }
                else { }

                string abName = m_Asset.GetABName();
                string assetName = m_Asset.GetAssetName();
                "error:Do not contains ab pack {0} when ResBridge get asset {1}".Log(!(Assets as Loader.AssetBundles).HasBundel(abName), abName, assetName);
                Sprite = Assets.Get<Sprite>(abName, assetName);
                Texture = Sprite.texture;
            }
            else { }
        }

        public Sprite CreateAsset()
        {
            return Instantiate(Sprite);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Sprite = default;
            Texture = default;
        }

        public void SetSubgroup(string ab, string asset)
        {
            m_Asset.SetSubgroup(ab, asset);
        }

        public Sprite Sprite { get; private set; }
        public Texture Texture { get; private set; }
    }
}

