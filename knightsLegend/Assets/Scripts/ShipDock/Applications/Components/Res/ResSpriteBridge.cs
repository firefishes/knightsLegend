using UnityEngine;

namespace ShipDock.Applications
{
    public class ResSpriteBridge : ResBridge, IResSpriteBridge
    {
        protected override void Awake()
        {
            base.Awake();

            if(m_Asset != default)
            {
                Sprite = Assets.Get<Sprite>(m_Asset.GetABName(), m_Asset.GetAssetName());
                Texture = Sprite.texture;
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Sprite = default;
            Texture = default;
        }

        public Sprite Sprite { get; private set; }
        public Texture Texture { get; private set; }
    }
}

