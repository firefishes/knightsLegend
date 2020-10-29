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
        }

        public void CreateRaw()
        {
            if ((m_Asset != default) && (Sprite == default))
            {
                if (Assets == default)
                {
                    Assets = ShipDockApp.Instance.ABs;
                }
                Sprite = Assets.Get<Sprite>(m_Asset.GetABName(), m_Asset.GetAssetName());
                Texture = Sprite.texture;
            }
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

