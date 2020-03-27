using ShipDock.Applications;
using ShipDock.Server;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

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

            components.Create<RoleColliderComponent>(KLConsts.C_ROLE_COLLIDER);
            components.Create<KLProcessComponent>(KLConsts.C_PROCESS);
            components.Create<KLRoleTimesComponent>(KLConsts.C_ROLE_TIMES);
            components.Create<RoleMoveComponent>(KLConsts.C_ROLE_MOVE);
            components.Create<RoleAIAtkComponent>(KLConsts.C_ROLE_AI_ATK);
            components.Create<RoleMustComponent>(KLConsts.C_ROLE_MUST);
            components.Create<KLRoleCampComponent>(KLConsts.C_ROLE_CAMP);
            components.Create<KLInputComponent>(KLConsts.C_ROLE_INPUT);
            components.Create<PositionComponent>(KLConsts.C_POSITION);
        }

        public override void ServerReady()
        {
            base.ServerReady();
        }
    }
}

