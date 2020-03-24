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

        private int mUnderAttackValue;
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
            if ((mUnderAttackValue > 0) && mUnderAttackUpdater.HasCompleted)
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
            mUnderAttackValue++;
        }

        private void OnAtkedMotion(Animator target)
        {
            mUnderAttackValue--;
            if(mUnderAttackValue <= 0)
            {
                mUnderAttackValue = 0;
                mUnderAttackUpdater.Stop();
            }
            mRole.RoleInput.SetInputPhase(UserInputPhases.ROLE_INPUT_PHASE_AFTER_MOVE);
        }
    }
}
