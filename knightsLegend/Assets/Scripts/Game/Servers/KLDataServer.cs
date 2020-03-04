using ShipDock.Applications;
using ShipDock.Server;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class KLDataServer : Server
    {
        private ServerRelater mRelater;

        public KLDataServer()
        {
            ServerName = KLConsts.S_DATAS;
            mRelater = new ServerRelater
            {
                DataNames = new int[]
                {
                    //KLConsts.DATA_GAME,
                    KLConsts.D_PLAYER
                },
                //ComponentNames = new int[]
                //{
                //    KLConsts.COMPONENT_ROLE_CAMP
                //}
            };

        }

        public override void InitServer()
        {
            base.InitServer();

            ShipDockApp app = ShipDockApp.Instance;
            var datas = app.Datas;
            //datas.AddData(new FWGameData());
            datas.AddData(new KLPlayerData());

            //Register<IParamNotice<IFWRole>>(CampRoleCreated, Pooling<CampRoleNotice>.Instance);
            //Register<IParamNotice<IFWRole>>(SetUserFWRoleResolver, Pooling<ParamNotice<IFWRole>>.Instance);

        }
    }
}
