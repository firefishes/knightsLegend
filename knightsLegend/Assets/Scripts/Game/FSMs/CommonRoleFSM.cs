using System;
using ShipDock.Applications;
using ShipDock.FSM;
using UnityEngine;

namespace KLGame
{
    public class CommonRoleFSM : AnimatorStateMachine
    {
        public CommonRoleFSM(int name) : base(default, name, ShipDockApp.Instance.StateMachines.Register)
        {
        }

        public KLRoleInputInfo RoleInput { get; set; } 
        public ICommonRole RoleEntitas { get; set; }
    }
}
