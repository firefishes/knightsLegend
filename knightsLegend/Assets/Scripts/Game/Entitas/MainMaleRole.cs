using ShipDock.Applications;

namespace KLGame
{
    public class MainMaleRole : KLRole, IMainRole
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

            ProvideGoals = new IGoal[]
            {
                new WillKillGoal(),
                new MoveToTargetGoal(),
            };

            ProvideWorldStates = new IWorldState[]
            {
                new RoleAliveState(this)
            };
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
