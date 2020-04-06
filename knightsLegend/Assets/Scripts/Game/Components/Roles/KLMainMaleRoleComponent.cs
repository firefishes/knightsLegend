using ShipDock.Applications;
using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Server;
using UnityEngine;

namespace KLGame
{
    public class KLMainMaleRoleComponent : KLRoleComponent
    {
        private MotionSceneInfo mSpeedDownMotionSceneInfo;
        private ComboMotionCreater mSpeedDownComboMotionCreater;

        protected override void Awake()
        {
            base.Awake();

            m_RoleRigidbody.mass = 20;
        }

        protected override void OnInited()
        {
            base.OnInited();

            RoleFSM = (mRole.RoleInput as KLRoleInputInfo).AnimatorFSM;
            (RoleFSM as CommonRoleFSM).SetAnimator(ref m_RoleAnimator);
            RoleFSM.Run(default, NormalRoleStateName.GROUNDED);

        }

        protected override void UpdateAnimations()
        {
            base.UpdateAnimations();
            
        }

        protected override void SetRoleEntitas()
        {
            mRole = new MainMaleRole();

            KLConsts.S_LENS.DeliveParam<KLCameraServer, KLRoleComponent>("InitPlayerRoleLen", "PlayerRole_0", OnSetRoleInitParam, true);
        }

        [Resolvable("PlayerRole_0")]
        private void OnSetRoleInitParam(ref IParamNotice<KLRoleComponent> target)
        {
            target.ParamValue = this;
        }

        protected override void UpdateRoleInputMoveValue(out Vector3 v)
        {
            Vector3 userInputValue = mRoleInput.GetUserInputValue();

            float x = userInputValue.x * 0.25f;
            if(!IsKinematic)
            {
                x = (Mathf.Abs(userInputValue.y) < 0.1f) ? x : -x;
            }
            v = Quaternion.Euler(transform.eulerAngles) * new Vector3(x, 0, userInputValue.y);
            mRoleInput.SetMoveValue(v);
        }

        protected override void UpdateAnimatorParams()
        {
            base.UpdateAnimatorParams();

            if (mRoleInput.GetUserInputValue(mFire1ParamName))
            {
                mRoleInput.SetUserInputValue(mFire1ParamName, false);

                CurrentSkillID = 1;
                MoveBlock = true;

                NormalATKStateParam param = Pooling<NormalATKStateParam>.From();
                param.Reinit(this, 1);
                
                if(RoleFSM.Current.StateName == NormalRoleStateName.NORMAL_ATK)
                {
                    RoleFSM.Current.SetStateParam(param);
                }
                else
                {
                    RoleFSM.ChangeState(NormalRoleStateName.NORMAL_ATK, param);
                }

            }
        }

        public override void OnATKCompleted()
        {
            base.OnATKCompleted();
        }
    }
}