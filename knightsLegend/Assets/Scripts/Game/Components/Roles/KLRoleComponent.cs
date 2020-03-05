using ShipDock.Applications;
using ShipDock.Notices;
using ShipDock.Server;
using System;
using System.Collections;
using System.Collections.Generic;
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
            v = new Vector3(mRoleInput.GetUserInputValue().x, 0, mRoleInput.GetUserInputValue().y);
            
            mRoleInput.SetMoveValue(v);
        }
    }
}
