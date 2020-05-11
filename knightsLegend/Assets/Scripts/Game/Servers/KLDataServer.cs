using ShipDock.Applications;
using ShipDock.Datas;
using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Server;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class KLDataServer : DataServer
    {
        private int mRoleIndex;

        public KLDataServer() : base()
        {
        }

        public override void InitServer()
        {
            base.InitServer();

            Register<IParamNotice<ICommonRole>>(CampRoleCreated, Pooling<CampRoleNotice>.Instance);
            Register<IParamNotice<ICommonRole>>(SetUserRoleResolver, Pooling<ParamNotice<ICommonRole>>.Instance);

        }

        public override void ServerReady()
        {
            base.ServerReady();

            Add<IParamNotice<ICommonRole>>(AddCampRole);
            Add<IParamNotice<ICommonRole>>(SetUserRole);
            
        }

        [Resolvable("SetUserRole")]
        private void SetUserRoleResolver<I>(ref I target) { }

        [Resolvable("CampRoleCreated")]
        private void CampRoleCreated(ref IParamNotice<ICommonRole> target)
        {
            var component = mRelater.ComponentRef<KLRoleCampComponent>(KLConsts.C_ROLE_CAMP);
            target.ParamValue = component.RoleCreated;
        }

        [Callable("SetUserRole", "SetUserRole")]
        private void SetUserRole(ref IParamNotice<ICommonRole> target)
        {
            IParamNotice<ICommonRole> notice = target as IParamNotice<ICommonRole>;
            KLRole role = notice.ParamValue as KLRole;

            KLPlayerData data = mRelater.DataRef<KLPlayerData>(KLConsts.D_PLAYER);
            data.SetCurrentRole(role);
        }

        [Callable("AddCampRole", "CampRoleCreated")]
        private void AddCampRole(ref IParamNotice<ICommonRole> target)
        {
            var data = mRelater.DataRef<KLPlayerData>(KLConsts.D_PLAYER);
            data.AddCampRole(target.ParamValue as KLRole);
            target.ParamValue.Name = "Role_".Append(mRoleIndex.ToString());
            mRoleIndex++;
        }

        public override int[] RelatedDataNames { get; } = new int[]
        {
            KLConsts.D_PLAYER,
            KLConsts.D_CONFIG,
            KLConsts.D_BATTLE,
        };

        public override int[] RelatedComponentNames { get; } = new int[]
        {
            KLConsts.C_ROLE_CAMP
        };

        public override IData[] DataList { get; } = new IData[]
        {
            new KLPlayerData(),
            new KLConfigData(),
            new KLBattleData(),
        };

        public override string DataServerName { get; } = KLConsts.S_DATAS;
    }
}
