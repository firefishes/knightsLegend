using ShipDock.Applications;
using ShipDock.Notices;
using ShipDock.Tools;
using UnityEngine;

namespace KLGame
{
    public class KLRoleComponent : RoleComponent
    {
        [SerializeField]
        private Transform m_BloodyEffectTF;

        protected string mIsAtkParamName = "IsAtk";
        protected string mFire1ParamName = "Fire1";
        protected ComboMotionCreater mNormalAtkMotionCreater;
        
        private AnimationInfoUpdater mUnderAttackUpdater;

        protected override void Awake()
        {
            base.Awake();

            m_RoleRigidbody.isKinematic = true;
        }

        protected override void Init()
        {
            base.Init();

            ValueItem[] triggers = new ValueItem[]
            {
                ValueItem.New("IsAtk", true)
            };
            ValueItem[] trans = new ValueItem[]
            {
                ValueItem.New("Atk1", 1f),
                ValueItem.New("Atk1", 2f),
                ValueItem.New("Atk1", 3f),
            };
            mNormalAtkMotionCreater = new ComboMotionCreater(3, triggers, trans, OnAtk1Completed);
            mNormalAtkMotionCreater.SetCheckComboTime(1.5f);

            FreezeAllRotation(false);
        }

        protected override void OnInited()
        {
            base.OnInited();

            KLRole = mRole as IKLRole;
        }

        protected override void InitRoleInputCallbacks()
        {
            base.InitRoleInputCallbacks();

            SetRoleInputCallback(UserInputPhases.ROLE_INPUT_PHASE_UNDERATTACKED, UnderAttack);
        }

        protected virtual void OnAtk1Completed()
        {
            //UnderAttack();
            //UnderAttack();
            //UnderAttack();
            //UnderAttack();
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

        protected void SetUnderAttackParam()
        {
            if (mUnderAttackUpdater == default)
            {
                mUnderAttackUpdater = new AnimationInfoUpdater();
            }
            if (mUnderAttackUpdater.HasCompleted)
            {
                mUnderAttackUpdater.Start(m_RoleAnimator, 0f, OnAtkedMotion, ValueItem.New("Atked", 1f), ValueItem.New("Forward", -0.6f));
            }
        }

        protected override void UpdateAnimations()
        {
            base.UpdateAnimations();

            mNormalAtkMotionCreater?.CheckAnimator(ref m_RoleAnimator);
            mNormalAtkMotionCreater?.CountComboTime(ref m_RoleAnimator);
            
        }

        public void UnderAttack()
        {
            MoveBlock = true;
            SetUnderAttackParam();
        }

        private void OnAtkedMotion(Animator target)
        {
            MoveBlock = false;
            m_RoleAnimator.SetFloat("Atked", 0f);
            m_RoleAnimator.SetFloat(m_BlendTreeInfo.MoveMotionName, 0f);
            mRole.RoleInput.SetInputPhase(UserInputPhases.ROLE_INPUT_PHASE_AFTER_MOVE);
        }

        protected override bool CheckMoveBlock()
        {
            return MoveBlock;
        }

        public IKLRole KLRole { get; private set; }
        public bool MoveBlock { get; set; }
    }
}
