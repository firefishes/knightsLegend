using UnityEngine;

namespace ShipDock.Applications
{
    public class ResTextureBridge : ResBridge, IResTextureBridge
    {
        protected override void Awake()
        {
            base.Awake();

            if(m_Asset != default)
            {
                Texture = Assets.Get<Texture>(m_Asset.GetABName(), m_Asset.GetAssetName());
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            Texture = default;
        }

        public Texture Texture { get; private set; }
    }
}

