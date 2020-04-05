﻿#define _G_LOG

using System;
using ShipDock.Applications;
using ShipDock.FSM;
using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Testers;
using ShipDock.Tools;
using UnityEngine;

namespace KLGame
{
    public class KLRoleComponent : RoleComponent, IKLRoleSceneComponent
    {
        [SerializeField]
        protected RoleSkillListSObj m_Skills;
        [SerializeField]
        private RoleCollider m_BloodyEffectTF;

        protected string mFire1ParamName = "Fire1";

        protected override void Awake()
        {
            base.Awake();

            m_RoleRigidbody.isKinematic = true;
        }

        protected override void Init()
        {
            base.Init();

            m_Skills?.Init();
            FreezeAllRotation(false);
        }

        protected override void OnInited()
        {
            base.OnInited();

            KLRole = mRole as IKLRole;
            KLRole.CollidingChanger += OnRoleAttackHitTrigger;
        }

        protected virtual void OnRoleAttackHitTrigger(int entitasID, int colliderID, bool isTrigger, bool isCollided)
        {
            if (isTrigger && isCollided)
            {
                var fsm = RoleFSM as IAssailableCommiter;
                if (fsm.HitCommit())
                {
                    ProcessingNotice notice = Pooling<ProcessingNotice>.From();
                    notice.Reinit(colliderID, ProcessingType.HIT, new ProcessingHitInfo
                    {
                        entitasID = entitasID,
                        hitColliderID = colliderID,
                        isTrigger = isTrigger,
                        isCollided = isCollided
                    });
                    notice.Commit(KLRole);
                    Pooling<ProcessingNotice>.To(notice);
                }
            }
        }

        protected override void InitRoleInputCallbacks()
        {
            base.InitRoleInputCallbacks();

            SetRoleInputCallback(UserInputPhases.ROLE_INPUT_PHASE_UNDERATTACKED, UnderAttack);
        }

        public virtual void OnATKCompleted()
        {
            AfterCurrentSkillReleased();
        }

        protected override void InitRoleData()
        {
        }

        protected override void SetRoleEntitas()
        {
        }

        protected override void OnRoleNotices(INoticeBase<int> obj)
        {
        }

        private Vector3 mWeapontPos;

        protected override void UpdateAnimations()
        {
            base.UpdateAnimations();

            //m_BloodyEffectTF?.Collider.

            mWeapontPos = m_BloodyEffectTF.transform.position - mWeapontPos;
            KLRole.WeapontPos = mWeapontPos;
        }

        public void UnderAttack()
        {
            KLRoleFSMStateParam param = Pooling<KLRoleFSMStateParam>.From();

            param.Reinit(this);
            m_RoleAnimator.SetFloat(m_BlendTreeInfo.MoveMotionName, 0f);

            if (RoleFSM.Current.StateName == NormalRoleStateName.UNDER_ATK)
            {
                RoleFSM.Current.SetStateParam(param);
            }
            else
            {
                RoleFSM.ChangeState(NormalRoleStateName.UNDER_ATK, param);
            }

            KLRole.RoleInput.SetInputPhase(UserInputPhases.ROLE_INPUT_PHASE_EMPTY, false);
        }

        private void OnAtkedMotion(Animator target)
        {
            MoveBlock = false;
            m_RoleAnimator.SetFloat("Atked", 0f);
            m_RoleAnimator.SetFloat(m_BlendTreeInfo.MoveMotionName, 0f);
            mRole.RoleInput.SetInputPhase(UserInputPhases.ROLE_INPUT_PHASE_AFTER_MOVE, false);
        }

        protected override bool CheckUnableToMove()
        {
            Tester.Instance.Log(KLTester.Instance, KLTester.LOG0, this is KLMainMaleRoleComponent, MoveBlock.ToString());
            return MoveBlock;
        }

        protected override bool ShouldUpdateTurnParam()
        {
            return !MoveBlock && base.ShouldUpdateTurnParam();
        }

        private void LateUpdate()
        {
            if (m_CameraNode == default)
            {
                return;
            }

            if(m_CameraNode.parent == transform)
            {
                CameraNodePosOffset = m_CameraNode.TransformPoint(m_CameraNode.localPosition);
                m_CameraNode.SetParent(default);
            }
            else
            {
                m_CameraNode.position = new Vector3(mRole.Position.x, m_CameraNode.position.y, mRole.Position.z);
                m_CameraNode.rotation = transform.rotation;
            }
        }

        protected void AfterCurrentSkillReleased()
        {
            CurrentSkillID = int.MaxValue;
        }

        public virtual void FillRoleFSMStateParam(IKLRoleFSMParam param)
        {
            param.KLRole = KLRole;
            param.CurrentSkillID = CurrentSkillID;
            param.SkillMapper = m_Skills.skillMotions;
            param.StartPos = KLRole.Position;
            param.StartRotation = transform.rotation;
        }

        private Vector3 CameraNodePosOffset { get; set; }

        protected IStateMachine RoleFSM { get; set; }

        public int CurrentSkillID { get; protected set; } = int.MaxValue;
        public IKLRole KLRole { get; private set; }
        public bool MoveBlock { get; set; }
    }

}
