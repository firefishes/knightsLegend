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

        protected override void SetRoleEntitas()
        {
            mRole = new MainMaleRole(m_FSMStates);

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

        protected override Vector3 CreateRoleRigidbodyVelocity(Vector3 v)
        {
            return base.CreateRoleRigidbodyVelocity(v);
        }

        protected override void UpdateAnimatorParams()
        {
            base.UpdateAnimatorParams();

            if (mRoleInput.GetUserInputValue(mFire1ParamName))
            {
                mRoleInput.SetUserInputValue(mFire1ParamName, false);

                CurrentSkillID = 1;

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
            else if (mRoleInput.GetUserInputValue(mFire2ParamName))
            {
                if (RoleFSM.Current.StateName != NormalRoleStateName.NORMAL_DEF)
                {
                    CurrentSkillID = 3;
                    KLRoleFSMStateParam param = Pooling<KLRoleFSMStateParam>.From();
                    param.Reinit(this, 2);
                    RoleFSM.ChangeState(NormalRoleStateName.NORMAL_DEF, param);
                }
            }
            else
            {
                if (!mRoleInput.GetUserInputValue(mFire2ParamName) && (RoleFSM.Current.StateName == NormalRoleStateName.NORMAL_DEF))
                {
                    m_RoleAnimator.SetFloat("DefenceType", 0f);
                    RoleFSM.ChangeState(NormalRoleStateName.GROUNDED);
                }
            }
        }

        public override void OnATKCompleted()
        {
            base.OnATKCompleted();
        }
    }
}