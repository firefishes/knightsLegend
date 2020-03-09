using ShipDock.Applications;
using ShipDock.Server;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class KLComponentServer : Server
    {
        public KLComponentServer()
        {
            ServerName = KLConsts.S_FW_COMPONENTS;
        }

        public override void InitServer()
        {
            base.InitServer();

            CreateComponents();
        }

        private void CreateComponents()
        {
            ShipDockApp app = ShipDockApp.Instance;
            var components = app.Components;
            components.Create<RoleMustComponent>(KLConsts.C_ROLE_MUST);
            //components.Create<RoleCampComponent>(KLConsts.COMPONENT_ROLE_CAMP);
            components.Create<KLInputComponent>(KLConsts.C_ROLE_INPUT);
            components.Create<PositionComponent>(KLConsts.C_POSITION);
            components.Create<RoleColliderComponent>(KLConsts.C_ROLE_COLLIDER);
        }

        public override void ServerReady()
        {
            base.ServerReady();
        }
    }
}

