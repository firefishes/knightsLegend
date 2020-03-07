
#if CINEMACHINE
using Cinemachine;
#endif
using ShipDock.Notices;
using ShipDock.Server;
using ShipDock.Tools;
using UnityEngine;

namespace ShipDock.Applications
{
    public class CamerLens<S, C>: MonoBehaviour, ICamerLens where S : IServer where C : ICamerLens
    {
        [SerializeField]
        private string m_LensServerName;
        [SerializeField]
        private VirtualCameraSubgroup[] m_CameraGroups;
        [SerializeField]
        private GameObject m_CameraFollower;

        private ComponentBridge mCompBridge;
        private KeyValueList<string, VirtualCameraSubgroup> mMapper;

        private void Awake()
        {
            mCompBridge = new ComponentBridge(OnInited);
            mCompBridge.Start();
        }

        private void OnInited()
        {
            mMapper = new KeyValueList<string, VirtualCameraSubgroup>();
            int max = m_CameraGroups.Length;
            for (int i = 0; i < max; i++)
            {
                mMapper[m_CameraGroups[i].Name] = m_CameraGroups[i];
            }
            
            m_LensServerName.DeliveParam<S, ICamerLens>("SetLens", "SetLensParamer", OnSetLens);
        }

        private void OnSetLens(ref IParamNotice<ICamerLens> target)
        {
            IParamNotice<ICamerLens> notice = target as IParamNotice<ICamerLens>;
            notice.ParamValue = this;
        }

#if CINEMACHINE
        public CinemachineVirtualCamera GetVirtualCamera(string name)
        {
            return mMapper[name].VirtualCamera;
        }
#endif

        public GameObject CameraFollower
        {
            get
            {
                return m_CameraFollower;
            }
        }
    }
}