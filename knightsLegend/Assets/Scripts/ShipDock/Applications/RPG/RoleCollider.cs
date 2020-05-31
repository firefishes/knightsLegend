#define G_LOG

using ShipDock.Notices;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{
    public class RoleCollider : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField]
        private bool m_IsLogTrigger;
#endif
        [SerializeField]
        private bool m_IsNotificational;
        [SerializeField]
        private int m_ActiveColliderNoticeName = int.MaxValue;
        [SerializeField]
        private RoleComponent m_RoleSceneComp;
        [SerializeField]
        private Collider m_Collider;
        
        private bool mIsTrigger = true;
        private List<int> mCollidingRoles;
        private ICommonRole mRoleEnitas;
        private ComponentBridge mBridge;

        private void Awake()
        {
            mRoleEnitas = m_RoleSceneComp.RoleEntitas;
            mCollidingRoles = mRoleEnitas.CollidingRoles;
            mBridge = new ComponentBridge(OnInit);
            mBridge.Start();
        }

        private void OnInit()
        {
            if(m_ActiveColliderNoticeName != int.MaxValue)
            {
                (mRoleEnitas as INotificationSender).Add(ActiveCollider);
            }
        }

        private void ActiveCollider(INoticeBase<int> param)
        {
            if (param.Name == m_ActiveColliderNoticeName && param is IParamNotice<bool>)
            {
                m_IsNotificational = (param as IParamNotice<bool>).ParamValue;
            }
        }

        private void OnDestroy()
        {
            mCollidingRoles = default;
            m_RoleSceneComp = default;
            mRoleEnitas = default;
        }

        private void AddColliding(int id, bool isTrigger, out int statu)
        {
            if (mRoleEnitas != default)
            {
                if (mCollidingRoles.Contains(id))
                {
                    statu = 1;
                }
                else
                {
                    statu = 0;
                    mCollidingRoles.Add(id);
                }
                if (m_IsNotificational)
                {
                    mRoleEnitas.CollidingChanged(id, isTrigger, true);
                }
            }
            else
            {
                statu = 2;
            }
        }

        private void RemoveColliding(int id, bool isTrigger, out int statu)
        {
            if (mRoleEnitas != default)
            {
                if (mCollidingRoles.Contains(id))
                {
                    statu = 0;
                    mCollidingRoles.Remove(id);
                }
                else
                {
                    statu = 1;
                }
                if (m_IsNotificational)
                {
                    mRoleEnitas.CollidingChanged(id, isTrigger, false);
                }
            }
            else
            {
                statu = 2;
            }
        }

        private void OnTriggerEnter(Collider other)
        {
            int id = other.GetInstanceID();
            AddColliding(id, true, out int statu);
#if UNITY_EDITOR
            switch (statu)
            {
                case 0:
                    Testers.Tester.Instance.Log(TesterRPG.Instance, TesterRPG.LOG, m_IsLogTrigger, "log: Collider trigger: ".Append(m_RoleSceneComp.name, " - ", other.transform.name, ", ", name));
                    break;
            }
#endif
        }

        private void OnTriggerExit(Collider other)
        {
            int id = other.GetInstanceID();
            RemoveColliding(id, true, out int statu);
#if UNITY_EDITOR
            switch (statu)
            {
                case 0:
                    Testers.Tester.Instance.Log(TesterRPG.Instance, TesterRPG.LOG, m_IsLogTrigger, "log: Collider trigger exit: ".Append(m_RoleSceneComp.name, " - ", other.transform.name, ", ", name));
                    break;
            }
#endif
        }

        private void OnCollisionEnter(Collision collision)
        {
            int id = collision.collider.GetInstanceID();
            AddColliding(id, false, out _);
        }

        private void OnCollisionExit(Collision collision)
        {
            int id = collision.collider.GetInstanceID();
            RemoveColliding(id, false, out _);
        }

        public Collider Collider
        {
            get
            {
                return m_Collider;
            }
        }
    }

}