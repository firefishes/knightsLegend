﻿using ShipDock.Applications;

namespace KLGame
{
    public class MainMaleRole : KLRole
    {
        public MainMaleRole(RoleFSMObj fsmStates) : base()
        {
            KLRoleData data = KLRoleData.GetRoleDataByRandom();
            data.StationaryTurnSpeed = 30f;
            data.Speed = 5f;
            data.ConfigID = 0;
            data.SetSource();
            SetRoleData(data);
            
            IsUserControlling = true;
            PositionEnabled = false;

            Camp = 0;

            FSMStates = fsmStates;
        }
        
        protected override IRoleInput CreateRoleInputInfo()
        {
            RoleFSMName = RoleMustSubgroup.animatorID;
            MainMaleRoleFSM fsm = new MainMaleRoleFSM(RoleFSMName)
            {
                RoleEntitas = this
            };
            RoleFSM = fsm;
            return new KLRoleInputInfo(this, fsm);
        }

        public override int RoleFSMName { get; set; }
    }
}
