using ShipDock.Applications;
using ShipDock.Notices;
using ShipDock.Server;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public static class KLConsts
    {
        public const string A_RES_DATA = "res_data/res_data";
        public const string A_RES_BRIGEDS = "res_brigdes";
        public const string A_MAIN_MALE_ROLE = "roles/main_male_role";

        public const string S_KL = "ServerKL";
        public const string S_KL_DATAS = "ServerDatas";
        public const string S_FW_COMPONENTS = "ServerComponents";

        public const int C_ROLE_MUST = 0;
        public const int C_ROLE_INPUT = 1;
        public const int C_POSITION = 2;
        public const int C_ROLE_COLLIDER = 3;

        //public static readonly IResolvableConfig[] ServerConfigs = {
        //new ResolvableConfigItem<INotice, Notice>("Notice"),
        //new ResolvableConfigItem<INotice, GameNotice>("GameNotice"),
        //new ResolvableConfigItem<IParamNotice<int>, ParamNotice<int>>("IntParamer"),
        //new ResolvableConfigItem<IParamNotice<Role>, ParamNotice<Role>>("PlayerRoleChoosen"),
        //new ResolvableConfigItem<IParamNotice<IFWRole>, CampRoleNotice>("CampRoleCreated"),
        //new ResolvableConfigItem<IParamNotice<IFWRole>, ParamNotice<IFWRole>>("SetUserFWRole"),
        //new ResolvableConfigItem<IParamNotice<FWInputer>, ParamNotice<FWInputer>>("FWInputerParamer"),
        //new ResolvableConfigItem<IParamNotice<FWInputer>, ParamNotice<FWInputer>>("SetFWInputerParamer"),
        //new ResolvableConfigItem<IParamNotice<FWCamerLens>, ParamNotice<FWCamerLens>>("SetLensParamer"),
        //};
        public static readonly IResolvableConfig[] ServerConfigs = MainServer.ServerConfigs.ToArray();
    }

}