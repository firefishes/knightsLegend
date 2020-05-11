using ShipDock.Applications;
using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class KLBattleServer : Server
    {
        public KLBattleServer() : base()
        {
            ServerName = KLConsts.S_BATTLE;
        }

        public override void InitServer()
        {
            base.InitServer();

            Register<IParamNotice<ICommonRole>>(BattleRoleParam, Pooling<ParamNotice<ICommonRole>>.Instance);
        }

        [Resolvable("SetBattleRoleParam")]
        private void BattleRoleParam(ref IParamNotice<ICommonRole> target) { }

        public override void ServerReady()
        {
            base.ServerReady();

            Add<IParamNotice<ICommonRole>>(EnterBattle);
        }

        [Callable("EnterBattle", "SetBattleRoleParam")]
        private void EnterBattle(ref IParamNotice<ICommonRole> target)
        {
            Debug.Log("EnterBattle");
            Debug.Log(target.ParamValue);
            Revert(target, "SetBattleRoleParam");
        }
    }
}