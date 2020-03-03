#if CROSS_PLATFORM_INPUT
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
#endif

namespace ShipDock.Applications
{
    public class TouchPadsInputer : InputerComponent
    {
#if CROSS_PLATFORM_INPUT
        [SerializeField]
        private TouchPad m_Mover;
        [SerializeField]
        private TouchPad m_TurnAndLooker;
        [SerializeField]
        private MobileControlRig m_MobileControlRig;

        protected override void SetMainServerName()
        {
            MainServerdName = m_MainServerName;
        }

        public TouchPad Mover
        {
            get
            {
                return m_Mover;
            }
        }

        public TouchPad TurnAndLooker
        {
            get
            {
                return m_TurnAndLooker;
            }
        }
        
        public MobileControlRig MobileControlRig
        {
            get
            {
                return m_MobileControlRig;
            }
        }
#endif
    }

}