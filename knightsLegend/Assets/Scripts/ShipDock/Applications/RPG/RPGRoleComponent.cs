using System;
using System.Collections;
using System.Collections.Generic;
using ShipDock.Notices;
using ShipDock.Tools;
using UnityEngine;
using UnityEngine.AI;

namespace ShipDock.Applications
{
    public class RPGRoleComponent : RoleComponent
    {
        [SerializeField]
        private NavMeshAgent m_NavMeshAgent;
        [SerializeField]
        private RoleInfo m_RoleCompInfo = new RoleInfo();
        [SerializeField]
        protected Transform m_CameraNode;

        private KeyValueList<int, Action> mRoleInputCallbacks;
        private KeyValueList<int, bool> mRoleInputCallSwitches;
        private Action mSceneCompCallaback;
        private Ray mCrouchRay;

#if UNITY_EDITOR
        [SerializeField]
        protected bool m_IsShowEnemyPos;

        private void OnGUI()
        {
            if (m_IsShowEnemyPos)
            {
                TesterRPG.Instance.ShowRoleInfoInGUI(this);
            }
        }

        public void CancelShowEnemyPos()
        {
            m_IsShowEnemyPos = false;
        }
#endif

        protected override void Init()
        {
            base.Init();

            InitRoleInputCallbacks();
        }

        protected override void Purge()
        {
            base.Purge();

            Utils.Reclaim(ref mRoleInputCallbacks);
            Utils.Reclaim(ref mRoleInputCallSwitches);
        }

        protected virtual void InitRoleInputCallbacks()
        {
            mRoleInputCallbacks = new KeyValueList<int, Action>();
            mRoleInputCallSwitches = new KeyValueList<int, bool>();
        }

        protected void SetRoleInputCallback(int phaseName, Action callback)
        {
            ActiveRoleInputPhase(phaseName, false);
            mRoleInputCallbacks?.Put(phaseName, callback);
        }

        public void ActiveRoleInputPhase(int phaseName, bool value)
        {
            mRoleInputCallSwitches[phaseName] = value;
        }

        protected override bool CheckUnableToMove()
        {
            return false;
        }

        protected override void OnRoleNotices(INoticeBase<int> obj)
        {
        }

        protected override void SetRoleEntitas()
        {
        }

        protected override void SyncInfos()
        {
            base.SyncInfos();
            
            m_RoleCompInfo.SetHp(mRole.RoleDataSource.Hp);
            m_RoleCompInfo.SetSpeed(mRole.SpeedCurrent);
            if (m_NavMeshAgent != default)
            {
                m_NavMeshAgent.speed = mRole.SpeedCurrent;
            }
        }

        protected override void UpdateByPositionComponent()
        {
            base.UpdateByPositionComponent();

            if (mRole.PositionEnabled)
            {
                AutoPathFinding();
            }
            else
            {
                UpdateByUserControlled();
            }
        }

        protected void SetNavMeshAgentStopped(bool flag)
        {
            if (m_NavMeshAgent == default)
            {
                return;
            }
            m_NavMeshAgent.isStopped = flag;
        }

        private void AutoPathFinding()
        {
            SetNavMeshAgentStopped(!mRole.FindingPath);
            if (mRole.FindingPath)
            {
                if ((mRole.TargetTracking != default) && !CheckUnableToMove())
                {
                    m_NavMeshAgent.destination = mRole.TargetTracking.Position;
                    mRoleInput.SetMoveValue(m_NavMeshAgent.velocity);
                }
            }
            else
            {
                if (mRoleInput != default)
                {
                    mRoleInput.SetMoveValue(mRoleInput.ForceMove);
                    if (IsKinematic)
                    {
                        transform.position += mRoleInput.GetMoveValue() * Time.deltaTime;
                    }
                    else
                    {
                        m_RoleRigidbody.velocity += mRoleInput.GetMoveValue();
                    }
                }
            }
            if (m_NavMeshAgent != default)
            {
                transform.LookAt(m_NavMeshAgent.destination);
            }
        }

        protected virtual void UpdateByUserControlled()
        {
            SetNavMeshAgentStopped(true);
            UpdateRoleInputMoveValue(out Vector3 v);
            SetRoleRigidbodyVelocity(CreateRoleRigidbodyVelocity(v));
        }

        protected virtual void UpdateRoleInputMoveValue(out Vector3 v)
        {
            Vector3 userInputValue = mRoleInput.GetUserInputValue();
            v = new Vector3(userInputValue.x, 0, userInputValue.y);
            mRoleInput.SetMoveValue(v);
        }

        protected virtual Vector3 CreateRoleRigidbodyVelocity(Vector3 v)
        {
            return IsKinematic ? v * mRole.SpeedCurrent : v * mRole.SpeedCurrent * 5;
        }

        protected void SetRoleRigidbodyVelocity(Vector3 v)
        {
            if (CheckUnableToMove())
            {
                v = Vector3.zero;
            }
            v += mRoleInput.ForceMove;
            if (IsKinematic)
            {
                transform.position += v * Time.deltaTime;
            }
            else
            {
                m_RoleRigidbody.velocity = v;
            }
        }

        protected void ExecuteSceneComponentInput()
        {
            if (mRoleInput != default)
            {
                int phaseValue = mRoleInput.RoleInputPhase;
                bool flag = mRoleInputCallSwitches[phaseValue];
                if (flag)
                {
                    mSceneCompCallaback = mRoleInputCallbacks[phaseValue];
                    mRoleInput.ExecuteBySceneComponent(ref mSceneCompCallaback);
                }
            }
        }

        protected override void UpdateAnimations()
        {
            ExecuteSceneComponentInput();

            base.UpdateAnimations();

            CheckRoleInputMovePhase();
            RoleAmoutExtranTurn();
            CheckRoleInputGroundPhase();
            RoleScaleCapsule();
            CheckRoleInputCrouchPhase();
        }

        private void CheckRoleInputMovePhase()
        {
            if (mRoleInput != default && mRoleInput.GetMoveValue().magnitude > 1f)
            {
                mRoleInput.MoveValueNormalize();
            }

            Vector3 v = transform.InverseTransformDirection(mRoleInput.GetMoveValue());
            mRoleInput.SetMoveValue(v);

            CheckGroundStatus();
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

        private void RoleAmoutExtranTurn()
        {
            if (mRole == default || mRoleInput == default)
            {
                return;
            }
            mRoleData = mRole.RoleDataSource;

            Vector3 move = Vector3.ProjectOnPlane(mRoleInput.GetMoveValue(), mRole.GroundNormal);
            mRoleInput.SetMoveValue(move);
            mRoleInput.UpdateAmout(RoleEntitas);
            mRoleInput.UpdateRoleExtraTurnRotation(ref mRoleData);
        }

        protected void CheckRoleInputGroundPhase()
        {
            transform.Rotate(0, mRoleInput.ExtraTurnRotationRef, 0);

            mAnimatorInfo = mRole.RoleAnimatorInfo;
            mAnimatorStateInfo = m_RoleAnimator.GetCurrentAnimatorStateInfo(0);
            mAnimatorInfo.IsMovementBlendTree = mAnimatorStateInfo.IsName(m_BlendTreeInfo.MainBlendTreeName);

            Vector3 velocity = m_RoleRigidbody.velocity;
            if (mRole.IsGrounded)
            {
                IRoleInput roleInput = mRoleInput as IRPGRoleInput;
                bool flag = mRoleInput.HandleGroundedMovement(ref roleInput, ref mAnimatorInfo);
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
                m_RoleRigidbody.AddForce(mRoleInput.ExtraGravityForceRef);
                mGroundCheckDistance = velocity.y < 0 ? m_RoleMustSubgroup.origGroundCheckDistance : 0.01f;
            }
        }

        private void RoleScaleCapsule()
        {
            IRoleInput roleInput = mRoleInput as IRPGRoleInput;
            mRoleInput?.ScaleCapsuleForCrouching(RoleEntitas, ref roleInput);
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
                if (!UpdateCrouchingByRay())
                {
                    mRoleInput.SetCrouching(false);
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
        }

        protected override void UpdateAnimator()
        {
            base.UpdateAnimator();

            UpdateLegMotionParam();
            UpdateGroundedStatu();
        }

        protected override void UpdateAnimatorParams()
        {
            base.UpdateAnimatorParams();

            UpdateAnimatorForwardParam();
            UpdateAnimatorTurnParam();

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
            float value = ShouldUpdateForwardParam() ? mRoleInput.ForwardAmount : 0f;
            m_RoleAnimator.SetFloat(m_BlendTreeInfo.MoveMotionName, value, 0.1f, Time.deltaTime);
        }

        protected virtual void UpdateAnimatorTurnParam()
        {
            float value = ShouldUpdateTurnParam() ? mRoleInput.TurnAmount : 0f;
            m_RoleAnimator.SetFloat(m_BlendTreeInfo.TurnMotionName, mRoleInput.TurnAmount, 0.1f, Time.deltaTime);
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

        protected virtual bool ShouldUpdateForwardParam()
        {
            return true;
        }

        protected virtual bool ShouldUpdateTurnParam()
        {
            return true;
        }

        protected virtual void CheckRoleAfterMovePhase()
        {
        }

        private void SetRoleGround(bool value)
        {
            mRole.IsGrounded = value;
            m_RoleAnimator.applyRootMotion = value;
            mRole.GroundNormal = value ? mGroundHitInfo.normal : Vector3.up;
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
    }
}