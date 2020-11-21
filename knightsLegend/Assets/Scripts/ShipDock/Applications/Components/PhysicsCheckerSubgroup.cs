#define _G_LOG

using ShipDock.ECS;
using ShipDock.Interfaces;
using ShipDock.Tools;
using System;
using UnityEngine;

namespace ShipDock.Applications
{
    /// <summary>
    /// 物理检测器子组件
    /// </summary>
    [Serializable]
    public class PhysicsCheckerSubgroup : IDispose
    {
#if UNITY_EDITOR
        [Header("测试")]
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

        [Header("检测半径")]
        [SerializeField]
        private float m_Radius = 2f;
        [Header("是否激活")]
        [SerializeField]
        private bool m_CheckerEnabled;
        [SerializeField]
        private LayerMask m_ColliderLayer;
        [Header("检测频率")]
        [SerializeField]
        private TimeGapper m_CheckGapper = new TimeGapper();

        private int mColliderLayer;
        private Collider mColliderItem;
        private Collider[] mCollidersOverlay;
        private ComponentBridge mBridge;
        private RayAndHitInfo mRayAndHitInfo;
        private ICommonOverlayMapper mCommonColliderMapper;

        public void SetSubgroup(IShipDockEntitas entitas, ICommonOverlayMapper commonCollider)
        {
            bool hasData = commonCollider.IsDataValid(ref entitas);
            if (hasData)
            {
                BehaviourIDs ids = commonCollider.GetEntitasData(ref entitas);
                SubgroupID = ids.gameItemID;

                mCommonColliderMapper = commonCollider;
                mCommonColliderMapper.PhysicsChecked(SubgroupID, true);
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

#if UNITY_EDITOR
            if (CheckerOwner != default)
            {
                m_CheckRange = CheckerOwner.gameObject.AddComponent<SphereCollider>();
                m_CheckRange.isTrigger = true;
                m_CheckRange.radius = m_Radius;
            }
#endif
        }

        public void Dispose()
        {
            Utils.Reclaim(ref mCollidersOverlay);
            mBridge?.Dispose();

            mCommonColliderMapper?.RemovePhysicsChecker(SubgroupID);

            mCommonColliderMapper = default;
            CheckerOwner = default;
            mBridge = default;
            mColliderItem = default;

            SubgroupID = int.MaxValue;
        }

        private void AddColliding(int id, bool isTrigger, bool isCollided, out int statu)
        {
            statu = 0;
            if (mCommonColliderMapper != default)
            {
                if (m_CheckerEnabled)
                {
                    mCommonColliderMapper.OverlayChecked(SubgroupID, id, isTrigger, isCollided);
                }
            }
            else
            {
                statu = 2;
            }
        }

        private void RemoveColliding(int id, bool isTrigger, bool isCollided, out int statu)
        {
            statu = 0;
            if (mCommonColliderMapper != default)
            {
                if (m_CheckerEnabled)
                {
                    mCommonColliderMapper.OverlayChecked(SubgroupID, id, isTrigger, isCollided);
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
            AddColliding(id, true, false, out int statu);
        }

        public void TriggerExit(Collider other)
        {
            if (SubgroupID == int.MaxValue)
            {
                return;
            }
            int id = other.GetInstanceID();
            RemoveColliding(id, true, false, out int statu);
        }

        public void CollisionEnter(Collision collision)
        {
            if (SubgroupID == int.MaxValue)
            {
                return;
            }
            int id = collision.collider.GetInstanceID();
            AddColliding(id, false, true, out _);
        }

        public void CollisionExit(Collision collision)
        {
            if (SubgroupID == int.MaxValue)
            {
                return;
            }
            int id = collision.collider.GetInstanceID();
            RemoveColliding(id, false, true, out _);
        }

        public void UpdatePhysicsCheck(ref Transform transform, bool isTrigger, bool isCollider)
        {
            if (m_CheckGapper.isStart)
            {
                m_CheckGapper.TimeAdvanced(Time.deltaTime);
                return;
            }
            else
            {
                m_CheckGapper.Start();
            }

            if (SubgroupID == int.MaxValue)
            {
                return;
            }
            mCollidersOverlay = Physics.OverlapSphere(transform.position, mRayAndHitInfo.radius, mRayAndHitInfo.layerMask);
            int max = mCollidersOverlay != default ? mCollidersOverlay.Length : 0;
            if (max > 0)
            {
                "log: Update physics start check, SubgroupID = {0}".Log(SubgroupID.ToString());
                int id;
                for (int i = 0; i < max; i++)
                {
                    mColliderItem = mCollidersOverlay[i];
                    id = mColliderItem.GetInstanceID();
                    if (id != SubgroupID)
                    {
                        AddColliding(id, isTrigger, isCollider, out _);
                    }
                }
            }
            mCommonColliderMapper.PhysicsChecked(SubgroupID);
#if UNITY_EDITOR
            UpdateInfoForEditor();
#endif
        }

        public void SetCheckerGapperTime(float time)
        {
            m_CheckGapper.totalTime = time;
        }

        public bool CheckerEnabled
        {
            get
            {
                return m_CheckerEnabled;
            }
            set
            {
                m_CheckerEnabled = value;
            }
        }

        public TimeGapper CheckGapper
        {
            get
            {
                return m_CheckGapper;
            }
        }

        public Transform CheckerOwner { get; set; }
        public int SubgroupID { get; private set; } = int.MaxValue;
    }
}