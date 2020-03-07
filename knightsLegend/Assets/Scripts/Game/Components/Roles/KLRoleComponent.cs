using ShipDock.Applications;
using ShipDock.Notices;
using ShipDock.Server;
using UnityEngine;

namespace KLGame
{
    public class KLRoleComponent : RoleComponent
    {
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
            Vector3 userInputValue = mRoleInput.GetUserInputValue();
            v = Quaternion.Euler(transform.eulerAngles) * new Vector3(userInputValue.x / 2, 0, userInputValue.y);
            mRoleInput.SetMoveValue(v);
        }
    }
}
