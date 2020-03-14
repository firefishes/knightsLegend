using System;
using System.Collections.Generic;
using ShipDock.Applications;
using ShipDock.Notices;
using ShipDock.Server;
using ShipDock.Tools;
using UnityEngine;

namespace KLGame
{
    public class KLRoleComponent : RoleComponent
    {
        private ComboMotionCreater mNormalAtkMotionCreater;

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
            mNormalAtkMotionCreater = new ComboMotionCreater(3, triggers, trans, OnAtk1Completed)
            {
                //ComboNext = OnNormalAtkComboNext,
                //ComboReseted = OnNormalAtkComboReseted
            };
            mNormalAtkMotionCreater.SetCheckComboTime(1.5f);

            FreezeAllRotation(false);
        }

        private void OnNormalAtkComboNext()
        {
            m_RoleAnimator.SetBool("IsAtk1Combo", true);
            Debug.Log(m_RoleAnimator.GetBool("IsAtk1Combo"));
        }

        private void OnNormalAtkComboReseted()
        {
            m_RoleAnimator.SetBool("IsAtk1Combo", false);
            m_RoleAnimator.SetFloat("Atk1", 0f);
        }

        private void OnAtk1Completed()
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
            mRole = new MainMaleRole();

            KLConsts.S_LENS.DeliveParam<KLCameraServer, KLRoleComponent>("InitPlayerRoleLen", "PlayerRole_0", OnSetRoleInitParam);
        }

        [Resolvable("PlayerRole_0")]
        private void OnSetRoleInitParam(ref IParamNotice<KLRoleComponent> target)
        {
            target.ParamValue = this;
        }

        protected override void OnRoleNotices(INoticeBase<int> obj)
        {
        }

        private string mIsAtkParamName = "IsAtk";
        private string mFire1ParamName = "Fire1";

        protected override void UpdateRoleInputMoveValue(out Vector3 v)
        {
            if (!m_RoleAnimator.GetBool(mIsAtkParamName))
            {
                Vector3 userInputValue = mRoleInput.GetUserInputValue();

                float x = userInputValue.x / 4;
                x = (Mathf.Abs(userInputValue.y) < 0.1f) ? x : -x;
                v = Quaternion.Euler(transform.eulerAngles) * new Vector3(x, 0, userInputValue.y);
                mRoleInput.SetMoveValue(v);
            }
            else
            {
                v = Vector3.zero;
            }
        }

        protected override void UpdateAnimatorParams()
        {
            base.UpdateAnimatorParams();
            
            if (mRoleInput.GetUserInputValue(mFire1ParamName))
            {
                mNormalAtkMotionCreater.AddComboMotion(ref m_RoleAnimator);
                mRoleInput.SetUserInputValue(mFire1ParamName, false);
            }
            SetUnderAttackParam();
        }

        private void SetUnderAttackParam()
        {
            if (mUnderAttackUpdater == default)
            {
                mUnderAttackUpdater = new AnimationInfoUpdater();
            }
            if ((mUnderAttackValue > 0) && mUnderAttackUpdater.HasCompleted)
            {
                mUnderAttackUpdater.Start(m_RoleAnimator, 0f, OnAtkedMotion, ValueItem.New("Atked", mUnderAttackValue * 0.2f), ValueItem.New("Forward", -0.6f));
            }
        }

        protected override void UpdateAnimations()
        {
            base.UpdateAnimations();

            mNormalAtkMotionCreater?.CheckAnimator(ref m_RoleAnimator);
            mNormalAtkMotionCreater?.CountComboTime(ref m_RoleAnimator);

        }

        private int mUnderAttackValue;
        private AnimationInfoUpdater mUnderAttackUpdater;

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
        }
    }
}
