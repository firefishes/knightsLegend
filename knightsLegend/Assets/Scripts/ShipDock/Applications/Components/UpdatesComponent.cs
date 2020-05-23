using UnityEngine;

namespace ShipDock.Applications
{
    public class UpdatesComponent : MonoBehaviour
    {
        [SerializeField]
        private int m_ReadyNoticeName = int.MaxValue;
        [SerializeField]
        private int m_AddItemNoticeName = int.MaxValue;
        [SerializeField]
        private int m_RemoveItemNoticeName = int.MaxValue;
        [SerializeField]
        private int m_CallLateItemNoticeName = int.MaxValue;

        private ComponentBridge mCompBridge;
        private UpdatesCacher mUpdatesCacher;

        private void Awake()
        {
            mCompBridge = new ComponentBridge(Init);
            mCompBridge.Start();
        }

        private void Init()
        {
            if ((int.MaxValue != m_AddItemNoticeName) && (int.MinValue != m_RemoveItemNoticeName))
            {
                mUpdatesCacher = new UpdatesCacher(m_AddItemNoticeName, m_RemoveItemNoticeName, m_CallLateItemNoticeName);
            }
            mCompBridge.Dispose();

            if(m_ReadyNoticeName != int.MaxValue)
            {
                m_ReadyNoticeName.Broadcast();
            }
        }

        private void Update()
        {
            mUpdatesCacher?.CheckDeleted();
            int time = (int)(Time.deltaTime * UpdatesCacher.UPDATE_CACHER_TIME_SCALE);
            mUpdatesCacher?.Update(time);
        }

        private void FixedUpdate()
        {
            mUpdatesCacher?.CheckDeleted();
            int time = (int)(Time.fixedDeltaTime * UpdatesCacher.UPDATE_CACHER_TIME_SCALE);
            mUpdatesCacher?.FixedUpdate(time);
        }

        private void LateUpdate()
        {
            mUpdatesCacher?.CheckDeleted();
            mUpdatesCacher?.LateUpdate();
        }
    }
}
