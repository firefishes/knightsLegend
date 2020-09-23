using UnityEngine;

namespace ShipDock.UI
{

    public class UIRoot : MonoBehaviour, IUIRoot
    {

        [SerializeField]
        private Camera m_UICamera;
        [SerializeField]
        private Canvas m_MainCanvas;
        [SerializeField]
        private OnUIRootAwaked m_OnAwaked = new OnUIRootAwaked();
        
        private void Awake()
        {
            UICamera = m_UICamera;
            MainCanvas = m_MainCanvas;
            m_OnAwaked?.Invoke(this);
        }

        public Canvas MainCanvas { get; private set; }
        public Camera UICamera { get; private set; }
    }
}

