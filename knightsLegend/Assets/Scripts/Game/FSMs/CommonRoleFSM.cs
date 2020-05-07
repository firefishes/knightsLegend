using System;
using ShipDock.Applications;
using ShipDock.FSM;
using UnityEngine;

namespace KLGame
{
    public class CommonRoleFSM : AnimatorStateMachine
    {
        private ICommonRole mRoleEntitas;

        //public CommonRoleFSM(int name, RoleFSMObj states) : this(name)
        //{
        //    RoleFSMObj = states;

            //IState state;
            //var stateInfos = RoleFSMObj.fsmStateInfo;
            //int max = stateInfos.Length;
            //for (int i = 0; i < max; i++)
            //{
            //    state = GetState(stateInfos[i].stateName);
            //}
        //}

        public CommonRoleFSM(int name) : base(default, name, ShipDockApp.Instance.StateMachines.Register)
        {
        }

        public ICommonRole RoleEntitas { get; set; }
        public KLRoleInputInfo RoleInput { get; set; }
        //public RoleFSMObj RoleFSMObj { get; private set; }
    }
}
