#define _G_LOG

using System;
using ShipDock.Applications;
using ShipDock.FSM;
using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Server;
using ShipDock.Testers;
using UnityEngine;

namespace KLGame
{
    public class KLRoleComponent : RoleComponent, IKLRoleSceneComponent
    {
        [SerializeField]
        protected RoleFSMObj m_FSMStates;
        [SerializeField]
        protected RoleSkillListSObj m_Skills;
        [SerializeField]
        private RoleCollider m_BloodyEffectTF;
        [SerializeField]
        private AnimationCurve m_HitFrozenCurve;

        protected string mFire1ParamName = "Fire1";
        protected string mFire2ParamName = "Fire2";

        private Vector3 mWeapontPos;
        
        protected override void Awake()
        {
            base.Awake();

            IsKinematic = true;
        }

        protected override void Init()
        {
            base.Init();

            m_Skills = m_Skills.LoadObj();
            m_Skills?.Init();
            FreezeAllRotation(false);
        }

        protected override void OnInited()
        {
            base.OnInited();

            KLRole = mRole as IKLRole;
            KLRole.CollidingChanger += OnRoleAttackHitTrigger;

            m_FSMStates = m_FSMStates.LoadObj();
            m_FSMStates.Init();
            m_FSMStates.FillToSceneComponent(this);

            RoleFSM = (mRole.RoleInput as KLRoleInputInfo).AnimatorFSM;
            (RoleFSM as CommonRoleFSM).SetAnimator(ref m_RoleAnimator);
            RoleFSM.Run(default, NormalRoleStateName.GROUNDED);
        }

        /// <summary>
        /// 角色的碰撞触发
        /// </summary>
        /// <param name="entitasID">角色实体id</param>
        /// <param name="colliderID">角色检测到的碰撞体id</param>
        /// <param name="isTrigger">是否为触发器</param>
        /// <param name="isCollided">如果为触发器，其值是否为已触发</param>
        protected virtual void OnRoleAttackHitTrigger(int entitasID, int colliderID, bool isTrigger, bool isCollided)
        {
            if (isTrigger && isCollided)
            {
                var fsm = RoleFSM as IAssailableCommiter;
                if (fsm.HitCommit(colliderID))//先判定攻击是否有效，然后添加流程事件处理，最后加入攻击结算的队列
                {
                    ProcessingNotice notice = Pooling<ProcessingNotice>.From();
                    notice.Reinit(colliderID, ProcessingType.HIT, new ProcessingHitInfo
                    {
                        entitasID = entitasID,
                        hitColliderID = colliderID,
                        isTrigger = isTrigger,
                        isCollided = isCollided
                    });
                    KLRole.Dispatch(notice);
                    notice.ToPool();
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
            switch (obj.Name)
            {
                case KLConsts.N_MOVE_BLOCK:
                    MoveBlock = true;
                    break;
                case KLConsts.N_MOVE_UNBLOCK:
                    MoveBlock = false;
                    break;
                case KLConsts.N_AFTER_UNDER_ATTACK:
                    ActiveRoleInputPhase(UserInputPhases.ROLE_INPUT_PHASE_UNDERATTACKED, true);
                    mRoleInput.SetInputPhase(KLConsts.ENEMY_INPUT_PHASE_ATTACK_AI);
                    MoveBlock = false;
                    break;
            }
        }

        protected override void UpdateAnimations()
        {
            base.UpdateAnimations();

            KLRole.CurQuaternaion = transform.rotation;

            mWeapontPos = m_BloodyEffectTF.transform.position - mWeapontPos;
            KLRole.WeapontPos = mWeapontPos;

            if (m_RoleAnimator.GetFloat("Speed") != 1f)
            {
                Time.timeScale = Time.timeScale * m_HitFrozenCurve.Evaluate(Time.deltaTime);
            }
            else
            {
                Time.timeScale = 1f;
            }
        }

        public void UnderAttack()
        {
            ActiveRoleInputPhase(UserInputPhases.ROLE_INPUT_PHASE_UNDERATTACKED, false);

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
        }

        protected override bool CheckUnableToMove()
        {
            Tester.Instance.Log(KLTester.Instance, KLTester.LOG0, !MoveBlock, "warnning: MoveBlock ".Append(MoveBlock.ToString()));
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

        public virtual void FillRoleFSMAIStateParam(IKLRoleFSMAIParam param)
        {
            param.KLRole = KLRole;
        }

        public void RoleFSMStateEntered(int stateName)
        {
            m_FSMStates.RoleFSMStateEntered(this, stateName);
        }

        public void RoleFSMStateWillFinish(int stateName)
        {
            m_FSMStates.RoleFSMStateWillFinish(this, stateName);
        }

        public void RoleFSMStateCombo(int stateName)
        {
            m_FSMStates.RoleFSMStateCombo(this, stateName);
        }

        private Vector3 CameraNodePosOffset { get; set; }

        protected IStateMachine RoleFSM { get; set; }

        public int CurrentSkillID { get; protected set; } = int.MaxValue;
        public IKLRole KLRole { get; private set; }
        public bool MoveBlock { get; set; }
    }

}
