#define G_LOG

using ShipDock.ECS;
using ShipDock.Interfaces;
using ShipDock.Notices;
using ShipDock.Tools;
using System;
using UnityEngine;

namespace ShipDock.Applications
{
    [Serializable]
    public class PhysicsCheckerSubgroup : IDispose
    {
#if UNITY_EDITOR
        [SerializeField]
        private bool m_IsLogTrigger;
        [SerializeField]
        private SphereCollider m_CheckRange;

        private void UpdateInfoForEditor()
        {
            if (m_CheckRange != default)
            {
                m_Radius = m_CheckRange.radius;
                mRayAndHitInfo.radius = m_Radius;
                mRayAndHitInfo.layerMask = m_ColliderLayer.value;
            }
        }
#endif

        [SerializeField]
        private float m_Radius = 2f;
        [SerializeField]
        private bool m_IsNotificational;
        [SerializeField]
        private int m_ActiveColliderNoticeName = int.MaxValue;
        [SerializeField]
        private LayerMask m_ColliderLayer;

        private int mColliderLayer;
        private Collider mColliderItem;
        private Collider[] mCollidersOverlay;
        private ComponentBridge mBridge;
        private RayAndHitInfo mRayAndHitInfo;
        private ICommonOverlayMapper mCommonColliderMapper;
        private INotificationSender mNotificationSender;

        public void SetSubgroup(IShipDockEntitas entitas, ICommonOverlayMapper commonCollider, INotificationSender notificationSender, int activeNoticeName = int.MaxValue)
        {
            bool hasData = commonCollider.IsDataValid(ref entitas);
            if (hasData)
            {
                BehaviourIDs ids = commonCollider.GetEntitasData(ref entitas);
                SubgroupID = ids.gameItemID;

                if (activeNoticeName != int.MaxValue)
                {
                    m_ActiveColliderNoticeName = activeNoticeName;
                }

                mCommonColliderMapper = commonCollider;
                mNotificationSender = notificationSender;
                mBridge = new ComponentBridge(OnInit);
                mBridge.Start();
            }
        }

        private void OnInit()
        {
            mBridge.Dispose();

            mColliderLayer = m_ColliderLayer.value;
            mRayAndHitInfo = new RayAndHitInfo
            {
                ray = new Ray(),
                layerMask = mColliderLayer,
                radius = m_Radius
            };

            if (m_ActiveColliderNoticeName != int.MaxValue)
            {
                mNotificationSender.Add(ActiveCollider);
            }

#if UNITY_EDITOR
            if (Parasitifer != default)
            {
                m_CheckRange = Parasitifer.gameObject.AddComponent<SphereCollider>();
                m_CheckRange.isTrigger = true;
                m_CheckRange.radius = m_Radius;
            }
#endif
        }

        public void Dispose()
        {
            Utils.Reclaim(ref mCollidersOverlay);
            mBridge?.Dispose();

            mCommonColliderMapper = default;
            mNotificationSender = default;
            Parasitifer = default;
            mBridge = default;
            mColliderItem = default;

            SubgroupID = int.MaxValue;
        }

        private void ActiveCollider(INoticeBase<int> param)
        {
            if ((param.NotifcationSender == mNotificationSender) && 
                (param.Name == m_ActiveColliderNoticeName) && 
                (param is IParamNotice<bool> notice))
            {
                m_IsNotificational = notice.ParamValue;
            }
        }

        private void AddColliding(int id, bool isTrigger, out int statu)
        {
            statu = 0;
            if (mCommonColliderMapper != default)
            {
                if (m_IsNotificational)
                {
                    mCommonColliderMapper.OverlayChecked(SubgroupID, id, isTrigger, true);
                }
            }
            else
            {
                statu = 2;
            }
        }

        private void RemoveColliding(int id, bool isTrigger, out int statu)
        {
            statu = 0;
            if (mCommonColliderMapper != default)
            {
                if (m_IsNotificational)
                {
                    mCommonColliderMapper.OverlayChecked(SubgroupID, id, isTrigger, false);
                }
            }
            else
            {
                statu = 2;
            }
        }

        public void TriggerEnter(Collider other)
        {
            if (SubgroupID == int.MaxValue)
            {
                return;
            }
            int id = other.GetInstanceID();
            AddColliding(id, true, out int statu);
        }

        public void TriggerExit(Collider other)
        {
            if (SubgroupID == int.MaxValue)
            {
                return;
            }
            int id = other.GetInstanceID();
            RemoveColliding(id, true, out int statu);
        }

        public void CollisionEnter(Collision collision)
        {
            if (SubgroupID == int.MaxValue)
            {
                return;
            }
            int id = collision.collider.GetInstanceID();
            AddColliding(id, false, out _);
        }

        public void CollisionExit(Collision collision)
        {
            if (SubgroupID == int.MaxValue)
            {
                return;
            }
            int id = collision.collider.GetInstanceID();
            RemoveColliding(id, false, out _);
        }

        public void UpdatePhysicsCheck(ref Transform transform)
        {
            if (SubgroupID == int.MaxValue)
            {
                return;
            }
            mCollidersOverlay = Physics.OverlapSphere(transform.position, mRayAndHitInfo.radius, mRayAndHitInfo.layerMask);
            int max = mCollidersOverlay != default ? mCollidersOverlay.Length : 0;
            if (max > 0)
            {
                int id;
                for (int i = 0; i < max; i++)
                {
                    mColliderItem = mCollidersOverlay[i];
                    id = mColliderItem.GetInstanceID();
                    AddColliding(id, true, out _);
                }
            }
#if UNITY_EDITOR
            UpdateInfoForEditor();
#endif
        }

        public Transform Parasitifer { get; set; }
        public int SubgroupID { get; private set; } = int.MaxValue;
    }
}