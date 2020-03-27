#define TEST_MOVER
#define _DEBUG_ENMEY_POS

using ShipDock.Notices;
using ShipDock.Tools;
using System;
using UnityEngine;
using UnityEngine.AI;

namespace ShipDock.Applications
{
    public abstract class RoleComponent : MonoBehaviour
    {

#if TEST_MOVER
        [SerializeField]
        protected Transform m_Mover;
#endif
        [SerializeField]
        private NavMeshAgent m_NavMeshAgent;
        [SerializeField]
        private RoleInfo m_RoleCompInfo = new RoleInfo();
        [SerializeField]
        private CommonRoleMustSubgroup m_RoleMustSubgroup;
        [SerializeField]
        protected Rigidbody m_RoleRigidbody;
        [SerializeField]
        private CapsuleCollider m_RoleCollider;
        [SerializeField]
        private Transform m_CameraNode;
        
        [SerializeField]
        protected Animator m_RoleAnimator;
        [SerializeField]
        protected RoleBlendTreeInfo m_BlendTreeInfo = new RoleBlendTreeInfo();

        protected IRoleData mRoleData;
        protected IRoleInput mRoleInput;
        protected RoleEntitas mRole;

        private bool mIsRoleNameSynced;
        private float mGroundCheckDistance = 0.3f;
        private Vector3 mInitPosition;
        private Action mSceneCompCallaback;
        private Ray mCrouchRay;
        private RaycastHit mGroundHitInfo;
        private AnimatorStateInfo mAnimatorStateInfo;
        private CommonRoleAnimatorInfo mAnimatorInfo;
        private ComponentBridge mBrigae;
        private KeyValueList<int, Action> mRoleInputCallbacks;

        protected virtual void Awake()
        {
            Init();

            mBrigae = new ComponentBridge(OnInited);
            mBrigae.Start();
        }

        private void OnDestroy()
        {
            mBrigae?.Dispose();
            mBrigae = default;
            
            GetInstanceID().Remove(OnRoleNotices);

            Utils.Reclaim(ref mRoleInputCallbacks);
        }

        protected abstract void InitRoleData();

        protected virtual void Init()
        {

            InitRoleInputCallbacks();
            InitRoleData();
            
            m_RoleMustSubgroup = new CommonRoleMustSubgroup
            {
                roleColliderID = m_RoleCollider.GetInstanceID(),
                animatorID = m_RoleAnimator.GetInstanceID(),
                rigidbodyID = m_RoleRigidbody.GetInstanceID(),
                origGroundCheckDistance = mGroundCheckDistance
            };

            FreezeAllRotation(true);
            m_RoleMustSubgroup.Init(ref m_RoleCollider);
        }

        protected virtual void InitRoleInputCallbacks()
        {
            mRoleInputCallbacks = new KeyValueList<int, Action>();
            SetRoleInputCallback(UserInputPhases.ROLE_INPUT_PHASE_MOVE_READY, CheckRoleInputMovePhase);
            SetRoleInputCallback(UserInputPhases.ROLE_INPUT_PHASE_CHECK_GROUND, CheckRoleInputGroundPhase);
            SetRoleInputCallback(UserInputPhases.ROLE_INPUT_PHASE_CHECK_CROUCH, CheckRoleInputCrouchPhase);
            SetRoleInputCallback(UserInputPhases.ROLE_INPUT_PHASE_AFTER_MOVE, CheckRoleAfterMovePhase);
        }

        protected void SetRoleInputCallback(int phaseName, Action callback)
        {
            mRoleInputCallbacks?.Put(phaseName, callback);
        }

        protected void FreezeAllRotation(bool flag)
        {
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

#if UNITY_EDITOR && DEBUG_ENMEY_POS
        private static RoleComponent sceneSelectedRole;

        [SerializeField]
        private bool m_IsShowEnemyPos;

        private GUIStyle mEnemyLabelStyle;

        private void OnGUI()
        {
            if(m_IsShowEnemyPos)
            {
                if (sceneSelectedRole != default)
                {
                    sceneSelectedRole.CancelShowEnemyPos();
                }
                sceneSelectedRole = this;

                if (mEnemyLabelStyle == default)
                {
                    mEnemyLabelStyle = new GUIStyle("enemyPosLabel")
                    {
                        fontSize = 30
                    };

                }
                string content = (mRole != default) ? mRole.GetDistFromMainLockDown().ToString() : string.Empty;

                GUILayout.Label(content, mEnemyLabelStyle);
            }
        }

        public void CancelShowEnemyPos()
        {
            m_IsShowEnemyPos = false;
        }
#endif

        protected void Start()
        {
            mInitPosition = transform.localPosition;
        }

        protected virtual void OnInited()
        {
            SetRoleEntitas();

            if(mRole != default)
            {
                mRole.Name = name;
                mRole.RoleMustSubgroup = m_RoleMustSubgroup;
                mRole.InitComponents();
                mRole.SpeedCurrent = mRole.Speed;
                mRoleData = mRole.RoleDataSource;
                mRole.SetSourceID(GetInstanceID());

                InitNotices();
            }
        }
        
        private void InitNotices()
        {
            GetInstanceID().Add(OnRoleNotices);
        }

        protected abstract void SetRoleEntitas();
        protected abstract void OnRoleNotices(INoticeBase<int> obj);
        protected abstract bool CheckMoveBlock();

        private void UpdateNavMeshAgent()
        {
            if (mRole.FindngPath)
            {
                if ((mRole.EnemyMainLockDown != default) && !CheckMoveBlock())
                {
                    m_NavMeshAgent.destination = mRole.EnemyMainLockDown.Position;
                    mRoleInput.SetMoveValue(m_NavMeshAgent.velocity);
                    transform.LookAt(m_NavMeshAgent.destination);
                }
            }
            else
            {
                if (mRoleInput != default)
                {
                    mRoleInput.SetMoveValue(Vector3.zero);
                }
            }
        }

        protected void UpdateByPositionComponent()
        {
            mRole.Direction = transform.forward;
            mRole.Position = transform.position;
            if (mRole.PositionEnabled)
            {
                UpdateNavMeshAgent();
            }
            else
            {
                UpdateByUserControlled();
            }
        }

        protected virtual void UpdateByUserControlled()
        {
            SetNavMeshAgentStopped(true);
            UpdateRoleInputMoveValue(out Vector3 v);
            SetRoleRigidbodyVelocity(CreateRoleRigidbodyVelocity(v));
        }

        protected void SetNavMeshAgentStopped(bool flag)
        {
            m_NavMeshAgent.isStopped = true;
        }

        protected virtual void UpdateRoleInputMoveValue(out Vector3 v)
        {
            Vector3 userInputValue = mRoleInput.GetUserInputValue();
            v = new Vector3(userInputValue.x, 0, userInputValue.y);
            mRoleInput.SetMoveValue(v);
        }

        protected void SetRoleRigidbodyVelocity(Vector3 v)
        {
            if (CheckMoveBlock())
            {
                v = Vector3.zero;
            }
            if (IsKinematic)
            {
                transform.position += v * Time.deltaTime;
            }
            else
            {
                m_RoleRigidbody.velocity = v;
            }
        }

        protected virtual Vector3 CreateRoleRigidbodyVelocity(Vector3 v)
        {
            return IsKinematic ? v * mRole.SpeedCurrent : v * mRole.SpeedCurrent * 10;
        }

        protected void CheckRoleInputGroundPhase()
        {
            transform.Rotate(0, mRoleInput.ExtraTurnRotationOut, 0);

            mAnimatorInfo = mRole.RoleAnimatorInfo;
            mAnimatorStateInfo = m_RoleAnimator.GetCurrentAnimatorStateInfo(0);
            mAnimatorInfo.IsMovementBlendTree = mAnimatorStateInfo.IsName(m_BlendTreeInfo.MainBlendTreeName);

            Vector3 velocity = m_RoleRigidbody.velocity;
            if (mRole.IsGrounded)
            {
                bool flag = mRoleInput.HandleGroundedMovement(ref mRoleInput, ref mAnimatorInfo);
                if (flag)
                {
                    // jump!
                    mRole.IsGrounded = false;
                    m_RoleRigidbody.velocity = new Vector3(velocity.x, mRoleData.JumpPower, velocity.z);
                    m_RoleAnimator.applyRootMotion = false;
                    mGroundCheckDistance = 0.3f;
                }
            }
            else
            {
                mRoleInput.HandleAirborneMovement(ref mRoleData);
                m_RoleRigidbody.AddForce(mRoleInput.ExtraGravityForceOut);
                mGroundCheckDistance = velocity.y < 0 ? m_RoleMustSubgroup.origGroundCheckDistance : 0.01f;
            }
            mRoleInput.NextPhase();
            mRoleInput.ResetEntitasCalled(UserInputPhases.ROLE_INPUT_PHASE_AMOUT_EXTRAN_TURN);
        }

        protected void CheckRoleInputCrouchPhase()
        {
            if (mRole.IsGroundedAndCrouch)
            {
                if (!mRoleInput.IsCrouching())
                {
                    mRoleInput.SetCrouching(true);
                    m_RoleCollider.height = m_RoleMustSubgroup.capsuleHeight / 2f;
                    m_RoleCollider.center = m_RoleMustSubgroup.capsuleCenter / 2f;
                }
            }
            else
            {
                if(!UpdateCrouchingByRay())
                {
                    mRoleInput.SetCrouching (false);
                    m_RoleCollider.height = m_RoleMustSubgroup.capsuleHeight;
                    m_RoleCollider.center = m_RoleMustSubgroup.capsuleCenter;
                }
            }
            // prevent standing up in crouch-only zones
            if (!mRoleInput.IsCrouching())
            {
                UpdateCrouchingByRay();
            }
            UpdateAnimator();
            mRoleInput.NextPhase();
            mRoleInput.ResetEntitasCalled(UserInputPhases.ROLE_INPUT_PHASE_SCALE_CAPSULE);
        }

        protected virtual void CheckRoleAfterMovePhase()
        {
            mRoleInput.SetInputPhase(UserInputPhases.ROLE_INPUT_PHASE_MOVE_READY);
        }

        private bool UpdateCrouchingByRay()
        {
            mCrouchRay = new Ray(m_RoleRigidbody.position + Vector3.up * m_RoleCollider.radius * RoleInfo.KHalf, Vector3.up);
            float crouchRayLength = m_RoleMustSubgroup.capsuleHeight - m_RoleCollider.radius * RoleInfo.KHalf;
            bool flag = Physics.SphereCast(mCrouchRay, m_RoleCollider.radius * RoleInfo.KHalf, crouchRayLength, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            if (flag)
            {
                mRoleInput.SetCrouching(true);
            }
            return flag;
        }

        private void SyncInfos()
        {
            m_RoleCompInfo.SetHp(mRole.RoleDataSource.Hp);
            m_RoleCompInfo.SetSpeed(mRole.SpeedCurrent);
            if (m_NavMeshAgent != default)
            {
                m_NavMeshAgent.speed = mRole.SpeedCurrent;
            }
            if (mRoleInput == default)
            {
                mRoleInput = mRole.RoleInput;
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
                ExecuteSceneComponentInput();
                mRoleInput.ShouldGetUserInput = true;

                UpdateAnimations();
            }
        }

        protected void ExecuteSceneComponentInput()
        {
            if (mRoleInput != default)
            {
                int phaseValue = mRoleInput.RoleInputPhase;
                mSceneCompCallaback = mRoleInputCallbacks[phaseValue];
                mRoleInput.ExecuteBySceneComponent(ref mSceneCompCallaback);
            }
        }

        protected virtual void UpdateAnimations()
        {
        }

        private void CheckRoleInputMovePhase()
        {
            Vector3 v = transform.InverseTransformDirection(mRoleInput.GetMoveValue());
            mRoleInput.SetMoveValue(v);

            CheckGroundStatus();

            mRoleInput.NextPhase();
            mRoleInput.ResetEntitasCalled(UserInputPhases.ROLE_INPUT_PHASE_MOVE_READY);
        }

        private void CheckGroundStatus()
        {

#if UNITY_EDITOR
            // helper to visualise the ground check ray in the scene view
            //Debug.DrawLine(transform.localPosition + (Vector3.up * 0.1f), transform.localPosition + (Vector3.up * 0.1f) + (Vector3.down * mGroundCheckDistance));
#endif
            // 0.1f is a small offset to start the ray from inside the character
            // it is also good to note that the transform position in the sample assets is at the base of the character

            //Debug.Log(transform.localPosition + (Vector3.up * 0.1f));
            //Debug.Log(Physics.Raycast(transform.localPosition + (Vector3.up * 0.1f), Vector3.down, out mGroundHitInfo, mGroundCheckDistance));

            bool flag = Physics.Raycast(transform.localPosition + (Vector3.up * 0.1f), Vector3.down, out mGroundHitInfo, mGroundCheckDistance);
            SetRoleGround(flag);
        }

        private void SetRoleGround(bool value)
        {
            mRole.IsGrounded = value;
            m_RoleAnimator.applyRootMotion = value;
            mRole.GroundNormal = value ? mGroundHitInfo.normal : Vector3.up;
        }

        protected virtual void UpdateAnimatorParams()
        {
            // update the animator parameters
            UpdateAnimatorForwardParam();
            UpdateAnimatorTurnParam();
            m_RoleAnimator.SetBool(m_BlendTreeInfo.OnGroundParamName, mRole.IsGrounded);

            if (m_BlendTreeInfo.ApplyCrouchMotion)
            {
                m_RoleAnimator.SetBool(m_BlendTreeInfo.CrouchParamName, mRoleInput.IsCrouching());
            }
            if (m_BlendTreeInfo.ApplyJumpMotion && !mRole.IsGrounded)
            {
                m_RoleAnimator.SetFloat(m_BlendTreeInfo.JumpMotionName, m_RoleRigidbody.velocity.y);
            }
        }

        protected virtual void UpdateAnimatorForwardParam()
        {
            m_RoleAnimator.SetFloat(m_BlendTreeInfo.MoveMotionName, mRoleInput.ForwardAmount, 0.1f, Time.deltaTime);
        }

        protected virtual void UpdateAnimatorTurnParam()
        {
            m_RoleAnimator.SetFloat(m_BlendTreeInfo.TurnMotionName, mRoleInput.TurnAmount, 0.1f, Time.deltaTime);
        }

        private void UpdateLegMotionParam()
        {
            // calculate which leg is behind, so as to leave that leg trailing in the jump animation
            // (This code is reliant on the specific run cycle offset in our animations,
            // and assumes one leg passes the other at the normalized clip times of 0.0 and 0.5)
            m_RoleCompInfo.SetRunCycleLegOffset(mRole.RoleAnimatorInfo.RunCycleLegOffset);
            float runCycle = Mathf.Repeat(m_RoleAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime + m_RoleCompInfo.RunCycleLegOffset(), 1);
            float jumpLeg = (runCycle < RoleInfo.KHalf ? 1 : -1) * mRoleInput.ForwardAmount;
            if (mRole.IsGrounded && m_BlendTreeInfo.ApplyJumpMotion)
            {
                m_RoleAnimator.SetFloat(m_BlendTreeInfo.JumpLegMotionName, jumpLeg);
            }
        }

        private void UpdateGroundedStatu()
        {
            // the anim speed multiplier allows the overall speed of walking/running to be tweaked in the inspector,
            // which affects the movement speed because of the root motion.
            if (mRole.IsGrounded && mRoleInput.GetMoveValue().magnitude > 0)
            {
                m_RoleCompInfo.SetAnimSpeedMultiplier(mRole.RoleAnimatorInfo.AnimSpeedMultiplier);
                m_RoleAnimator.speed = m_RoleCompInfo.AnimSpeedMultiplier();
            }
            else
            {
                // don't use that while airborne
                m_RoleAnimator.speed = 1;
            }
        }

        private void UpdateAnimator()
        {
            UpdateAnimatorParams();
            UpdateLegMotionParam();
            UpdateGroundedStatu();
        }

        public void OnAnimatorMove()
        {
            // we implement this function to override the default root motion.
            // this allows us to modify the positional speed before it's applied.
            if (mRole != default && mRole.IsGrounded && Time.deltaTime > 0)
            {
                m_RoleCompInfo.SetMoveSpeedMultiplier(mRole.RoleAnimatorInfo.MoveSpeedMultiplier);
                Vector3 v = m_RoleAnimator.deltaPosition * m_RoleCompInfo.MoveSpeedMultiplier() / Time.deltaTime;

                // we preserve the existing y part of the current velocity.
                v.y = m_RoleRigidbody.velocity.y;
                m_RoleRigidbody.velocity = v;
            }
        }

        public Transform CameraNode
        {
            get
            {
                return m_CameraNode;
            }
        }

        public bool IsKinematic
        {
            get
            {
                return m_RoleRigidbody != default ? m_RoleRigidbody.isKinematic : false;
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

