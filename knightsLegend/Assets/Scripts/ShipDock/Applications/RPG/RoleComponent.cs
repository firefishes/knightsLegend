#define G_LOG
#define TEST_MOVER

using ShipDock.Notices;
using UnityEngine;

namespace ShipDock.Applications
{
    public abstract class RoleComponent : MonoBehaviour, INotificationSender
    {

#if TEST_MOVER
        [SerializeField]
        protected Transform m_Mover;
#endif
        [SerializeField]
        protected CommonRoleMustSubgroup m_RoleMustSubgroup;
        [SerializeField]
        protected Rigidbody m_RoleRigidbody;
        [SerializeField]
        protected CapsuleCollider m_RoleCollider;
        [SerializeField]
        private Collider m_RoleScanedCollider;
        
        [SerializeField]
        protected Animator m_RoleAnimator;
        [SerializeField]
        protected RoleBlendTreeInfo m_BlendTreeInfo = new RoleBlendTreeInfo();

        protected IRoleData mRoleData;
        protected IRPGRoleInput mRoleInput;
        protected RoleEntitas mRole;
        protected CommonRoleAnimatorInfo mAnimatorInfo;
        protected AnimatorStateInfo mAnimatorStateInfo;
        protected RaycastHit mGroundHitInfo;
        protected float mGroundCheckDistance = 0.3f;

        private string mName;
        private int mInstanceID;
        private bool mIsRoleNameSynced;
        private Vector3 mInitPosition;
        private ComponentBridge mBrigae;

        protected virtual void Awake()
        {
            Init();

            mBrigae = new ComponentBridge(OnInited);
            mBrigae.Start();
        }

        private void OnDestroy()
        {
            Purge();

            mBrigae?.Dispose();
            mBrigae = default;
            
            this.Remove(OnRoleNotices);
        }

        protected virtual void Purge()
        {
        }

        protected virtual void Init()
        {
            mName = name;
            mInstanceID = GetInstanceID();
            
            m_RoleMustSubgroup = new CommonRoleMustSubgroup
            {
                roleColliderID = m_RoleCollider.GetInstanceID(),
                roleScanedColliderID = m_RoleScanedCollider.GetInstanceID(),
                animatorID = m_RoleAnimator.GetInstanceID(),
                rigidbodyID = m_RoleRigidbody.GetInstanceID(),
                origGroundCheckDistance = mGroundCheckDistance
            };

            FreezeAllRotation(true);
            m_RoleMustSubgroup.Init(ref m_RoleCollider);
            
        }

        protected virtual void SetRoleData()
        {
            if (mRole != default)
            {
                mRoleData = mRole.RoleDataSource;
            }
        }

        protected void FreezeAllRotation(bool flag)
        {
            if (m_RoleRigidbody == default)
            {
                return;
            }
            if (flag)
            {
                m_RoleRigidbody.constraints = RigidbodyConstraints.FreezeRotationX |
                                              RigidbodyConstraints.FreezeRotationY |
                                              RigidbodyConstraints.FreezeRotationZ;
            }
            else
            {
                m_RoleRigidbody.constraints = RigidbodyConstraints.None;
            }
        }

        protected void Start()
        {
            mInitPosition = transform.localPosition;
        }

        protected virtual void OnInited()
        {
            SetRoleEntitas();

            if(mRole != default)
            {
                SetRoleData();

                mRole.Name = mName;
                mRole.SetEntitasID(mInstanceID);
                mRole.RoleMustSubgroup = m_RoleMustSubgroup;
                mRole.SpeedCurrent = mRole.Speed;
                mRole.InitComponents();

                InitNotices();
            }
        }
        
        private void InitNotices()
        {
            this.Add(OnRoleNotices);
        }

        protected abstract void SetRoleEntitas();//Set mRole with a sub class of RoleEntitas.
        protected abstract void OnRoleNotices(INoticeBase<int> obj);//Fill the logic of Notice handler function.
        protected abstract bool CheckUnableToMove();//Make default to false
        
        protected virtual void UpdateByPositionComponent()
        {
            mRole.Direction = transform.forward;
            mRole.Position = transform.position;
        }

        protected virtual void SyncInfos()
        {
            if (mRoleInput == default)
            {
                mRoleInput = mRole.RoleInput as IRPGRoleInput;
            }
            mRoleInput.SetDeltaTime(Time.deltaTime);
            if (!mIsRoleNameSynced && !string.IsNullOrEmpty(mRole.Name))
            {
                mIsRoleNameSynced = true;
                name = mRole.Name;
            }
        }

        private void Update()
        {
            if (mRole != default)
            {
                SyncInfos();
                UpdateByPositionComponent();
                UpdateAnimations();

                mRoleInput.ShouldGetUserInput = true;
            }
        }

        protected virtual void UpdateAnimations()
        {
        }

        protected virtual void UpdateAnimator()
        {
            UpdateAnimatorParams();
        }

        protected virtual void UpdateAnimatorParams()
        {
            // update the animator parameters
            m_RoleAnimator.SetBool(m_BlendTreeInfo.OnGroundParamName, mRole.IsGrounded);
        }

        public bool IsKinematic
        {
            get
            {
                return m_RoleRigidbody != default ? m_RoleRigidbody.isKinematic : false;
            }
            set
            {
                if (m_RoleRigidbody != default)
                {
                    m_RoleRigidbody.isKinematic = value;
                }
            }
        }

        public ICommonRole RoleEntitas
        {
            get
            {
                return mRole;
            }
        }
    }
}

