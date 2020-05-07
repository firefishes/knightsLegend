using ShipDock.Applications;

namespace KLGame
{
    public class MainMaleRole : KLRole
    {
        public MainMaleRole(RoleFSMObj fsmStates) : base()
        {
            IRoleData data = KLRoleData.GetRoleDataByRandom();
            data.Speed = 18f;
            data.ConfigID = 0;
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
