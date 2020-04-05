using System;
using ShipDock.Applications;

namespace KLGame
{
    public class MainMaleRole : KLRole
    {
        public MainMaleRole()
        {
            IRoleData data = KLRoleData.GetRoleDataByRandom();
            data.Speed = 18f;
            data.ConfigID = 0;
            SetRoleData(data);
            
            IsUserControlling = true;
            PositionEnabled = false;

            Camp = 0;
        }
        
        protected override IRoleInput CreateRoleInputInfo()
        {
            var fsm = new MainMaleRoleFSM(RoleFSMName)
            {
                RoleEntitas = this
            };
            return new KLRoleInputInfo(this, fsm);
        }

        public override int RoleFSMName { get; } = KLConsts.RFSM_MAIN_MALE_ROLE;
    }
}
