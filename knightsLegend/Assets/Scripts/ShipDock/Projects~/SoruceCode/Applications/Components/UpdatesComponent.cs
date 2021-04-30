using ShipDock.Notices;
using ShipDock.Ticks;
using UnityEngine;

namespace ShipDock.Applications
{
    [DisallowMultipleComponent]
    public class UpdatesComponent : MonoBehaviour, IUpdatesComponent
    {
        [SerializeField]
        private int m_ReadyNoticeName = ShipDockConsts.NOTICE_SCENE_UPDATE_READY;
        [SerializeField]
        private int m_AddItemNoticeName = ShipDockConsts.NOTICE_ADD_SCENE_UPDATE;
        [SerializeField]
        private int m_RemoveItemNoticeName = ShipDockConsts.NOTICE_REMOVE_SCENE_UPDATE;
        [SerializeField]
        private int m_CallLateItemNoticeName = ShipDockConsts.NOTICE_SCENE_CALL_LATE;

        private UpdatesCacher mUpdatesCacher;

        private void Awake()
        {
            Framework.Instance.Updates = this;
        }

        public void Init()
        {
            if ((int.MaxValue != m_AddItemNoticeName) && (int.MinValue != m_RemoveItemNoticeName))
            {
                mUpdatesCacher = new UpdatesCacher(m_AddItemNoticeName, m_RemoveItemNoticeName, m_CallLateItemNoticeName);
            }

            if(m_ReadyNoticeName != int.MaxValue)
            {
                m_ReadyNoticeName.Broadcast();
            }
        }

        private void Update()
        {
            int time = (int)(Time.deltaTime * UpdatesCacher.UPDATE_CACHER_TIME_SCALE);
            mUpdatesCacher?.Update(time);
        }

        private void FixedUpdate()
        {
            int time = (int)(Time.fixedDeltaTime * UpdatesCacher.UPDATE_CACHER_TIME_SCALE);
            mUpdatesCacher?.FixedUpdate(time);
        }

        private void LateUpdate()
        {
            mUpdatesCacher?.LateUpdate();
            mUpdatesCacher?.CheckDeleted();
        }
    }
}
