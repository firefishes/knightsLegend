
#if CINEMACHINE
using Cinemachine;
#endif
using System;
using UnityEngine;

namespace ShipDock.Applications
{
    [Serializable]
    public class VirtualCameraSubgroup
    {
        [SerializeField]
        private string m_Name;
#if CINEMACHINE
        [SerializeField]
        private CinemachineVirtualCamera m_VirturalCamera;
#endif

        public string Name
        {
            get
            {
                return m_Name;
            }
        }

#if CINEMACHINE
        public CinemachineVirtualCamera VirtualCamera
        {
            get
            {
                return m_VirturalCamera;
            }
        }
#endif
    }
}
