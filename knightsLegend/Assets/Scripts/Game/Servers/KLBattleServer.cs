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
        private ServerRelater mRelater;

        public KLBattleServer() : base()
        {
            ServerName = KLConsts.S_BATTLE;

            mRelater = new ServerRelater
            {
                DataNames = new int[]
                {
                    KLConsts.D_BATTLE
                }
            };
        }

        protected override void Purge()
        {
            base.Purge();
            
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

            mRelater.CommitRelate();

            Add<IParamNotice<ICommonRole>>(EnterBattle);
        }

        [Callable("EnterBattle", "SetBattleRoleParam")]
        private void EnterBattle(ref IParamNotice<ICommonRole> target)
        {
            KLBattleData data = mRelater.DataRef<KLBattleData>(KLConsts.D_BATTLE);
            data.AddBattleUnit(target.ParamValue);

            Revert(target, "SetBattleRoleParam");
        }
    }
}