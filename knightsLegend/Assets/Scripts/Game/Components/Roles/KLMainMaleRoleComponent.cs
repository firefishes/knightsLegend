using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Server;
using ShipDock.Tools;
using UnityEngine;

namespace KLGame
{
    public class KLMainMaleRoleComponent : KLRoleComponent
    {
        protected override void Awake()
        {
            base.Awake();

            m_RoleRigidbody.mass = 20;
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

        protected override void UpdateRoleInputMoveValue(out Vector3 v)
        {
            if (!m_RoleAnimator.GetBool(mIsAtkParamName))
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
                //mNormalAtkMotionCreater.AddComboMotion(ref m_RoleAnimator);
                m_Skills?.skillMotions.StartSkill(1, ref m_RoleAnimator);
                mRoleInput.SetUserInputValue(mFire1ParamName, false);

                PlayerHit hit = Pooling<PlayerHit>.From();
                hit.PlayerRole = KLRole;
                hit.HitInfoScope.minAngle = 15f;
                hit.HitInfoScope.minDistance = 1.5f;
                hit.HitInfoScope.startPos = mRole.Position;
                hit.HitInfoScope.startRotation = transform.rotation;
                hit.HitInfoScope.Draws();
                KLRole.Processing.AddProcess(hit);
            }
        }
    }
}