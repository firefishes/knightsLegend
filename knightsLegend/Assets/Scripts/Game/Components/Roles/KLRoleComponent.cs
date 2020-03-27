#define _G_LOG

using ShipDock.Applications;
using ShipDock.Notices;
using ShipDock.Testers;
using ShipDock.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class KLRoleComponent : RoleComponent
    {
        //[SerializeField]
        //private SkillsMapper m_Skills;
        //[SerializeField]
        //protected SkillMotionsMapper m_SkillMotions = new SkillMotionsMapper();
        [SerializeField]
        protected RoleSkillList m_Skills;
        [SerializeField]
        private Transform m_BloodyEffectTF;

        protected string mIsAtkParamName = "IsAtk";
        protected string mFire1ParamName = "Fire1";
        //protected ComboMotionCreater mNormalAtkMotionCreater;
        //private AnimationInfoUpdater mUnderAttackUpdater;

        protected override void Awake()
        {
            base.Awake();

            m_RoleRigidbody.isKinematic = true;
        }

        protected override void Init()
        {
            base.Init();

            //ValueItem[] triggers = new ValueItem[]
            //{
            //    ValueItem.New("IsAtk", true)
            //};
            //ValueItem[] trans = new ValueItem[]
            //{
            //    ValueItem.New("Atk1", 1f),
            //    ValueItem.New("Atk1", 2f),
            //    ValueItem.New("Atk1", 3f),
            //};
            //mNormalAtkMotionCreater = new ComboMotionCreater(3, triggers, trans, OnAtk1Completed);
            //mNormalAtkMotionCreater.SetCheckComboTime(1.5f);

            m_Skills?.Init();

            m_Skills.skillMotions.GetValue(0).motionCompletionEvent.AddListener(OnAtk1Completed);

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

        public virtual void OnAtk1Completed()
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
            //if (mUnderAttackUpdater == default)
            //{
            //    mUnderAttackUpdater = new AnimationInfoUpdater();
            //}
            //if (mUnderAttackUpdater.HasCompleted)
            //{
            //    mUnderAttackUpdater.Start(m_RoleAnimator, 0f, OnAtkedMotion, ValueItem.New("Atked", 1f), ValueItem.New("Forward", -0.6f));
            //}
            m_Skills.skillMotions.StartSkill(0, ref m_RoleAnimator, OnAtkedMotion);
        }

        protected override void UpdateAnimations()
        {
            base.UpdateAnimations();

            m_Skills?.skillMotions?.UpdateMotions(ref m_RoleAnimator);
            //mNormalAtkMotionCreater?.CheckAnimator(ref m_RoleAnimator);
            //mNormalAtkMotionCreater?.CountComboTime(ref m_RoleAnimator);
            
        }

        public void UnderAttack()
        {
            if(MoveBlock)
            {
                return;
            }
            MoveBlock = true;
            SetUnderAttackParam();
        }

        private void OnAtkedMotion(Animator target)
        {
            MoveBlock = false;
            m_RoleAnimator.SetFloat("Atked", 0f);
            m_RoleAnimator.SetFloat(m_BlendTreeInfo.MoveMotionName, 0f);
            mRole.RoleInput.SetInputPhase(UserInputPhases.ROLE_INPUT_PHASE_AFTER_MOVE, false);
        }

        protected override bool CheckMoveBlock()
        {
            Tester.Instance.Log(KLTester.Instance, KLTester.LOG0, this is KLMainMaleRoleComponent, MoveBlock.ToString());
            return MoveBlock;
        }

        public IKLRole KLRole { get; private set; }
        public bool MoveBlock { get; set; }
    }

}
