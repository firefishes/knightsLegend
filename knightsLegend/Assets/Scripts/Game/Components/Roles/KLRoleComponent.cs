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

            mNormalAtkMotionCreater = new ComboMotionCreater(3, "Atk1", "IsAtk", OnAtk1Completed);
            
            FreezeAllRotation(false);
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
        
        protected override void UpdateRoleInputMoveValue(out Vector3 v)
        {
            if (!m_RoleAnimator.GetBool("IsAtk"))
            {
                Vector3 userInputValue = mRoleInput.GetUserInputValue();

                float x = userInputValue.x / 2;
                x = (Mathf.Abs(userInputValue.y) < 0.05f) ? x: -x;
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
            
            if(mRoleInput.GetUserInputValue("Fire1"))
            {
                if(!m_RoleAnimator.GetBool("IsAtk"))
                {
                    m_RoleAnimator.SetBool("IsAtk", true);
                }
                mNormalAtkMotionCreater.AddComboMotion(ref m_RoleAnimator);
                mRoleInput.SetUserInputValue("Fire1", false);
            }
        }

        protected override void UpdateAnimations()
        {
            base.UpdateAnimations();

            mNormalAtkMotionCreater?.CheckAnimator(ref m_RoleAnimator);

        }

        private void OnAtk1Completed()
        {
            if(mNormalAtkMotionCreater.IsMotonsFinish)
            {
                m_RoleAnimator.SetBool("IsAtk", false);
            }
        }
    }
}
