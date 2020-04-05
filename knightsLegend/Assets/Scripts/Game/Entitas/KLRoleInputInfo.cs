using ShipDock.Applications;
using ShipDock.FSM;

namespace KLGame
{
    public class KLRoleInputInfo : RoleInputInfo
    {
        public KLRoleInputInfo(ICommonRole roleEntitas, CommonRoleFSM fsm) : base(roleEntitas)
        {
            AnimatorFSM = fsm;
            AnimatorFSM.RoleInput = this;
        }

        public CommonRoleFSM AnimatorFSM { get; private set; }
    }

}