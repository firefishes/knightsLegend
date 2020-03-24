using ShipDock.Applications;
using ShipDock.Notices;
using ShipDock.Server;

namespace KLGame
{
    public static class KLConsts
    {
        public const string A_RES_DATA = "res_data/res_data";
        public const string A_RES_BRIGEDS = "res_brigdes";
        public const string A_MAIN_MALE_ROLE = "roles/main_male_role";
        public const string A_ENMEY_ROLE = "roles/enmey_role";

        public const string S_KL = "ServerKL";
        public const string S_DATAS = "ServerDatas";
        public const string S_FW_COMPONENTS = "ServerComponents";
        public const string S_LENS = "ServerLens";

        public const int C_ROLE_MUST = 0;
        public const int C_ROLE_INPUT = 1;
        public const int C_POSITION = 2;
        public const int C_ROLE_COLLIDER = 3;
        public const int C_ROLE_CAMP = 4;
        public const int C_ROLE_AI_ATK = 5;
        public const int C_ROLE_INPUT_PHASES = 6;
        public const int C_ROLE_MOVE = 7;
        public const int C_ROLE_TIMES = 8;
        public const int C_PROCESS = 9;

        public const int D_GAME = 0;
        public const int D_PLAYER = 1;

        public const int DC_CAMP_ROLE_CREATED = 2000;

        public const int ROLE_INPUT_TYPE_DEFAULT = 0;
        public const int ROLE_INPUT_TYPE_ENEMY = 1;

        private static readonly IResolvableConfig[] KLServerConfigs =
        {
            //new ResolvableConfigItem<INotice, GameNotice>("GameNotice"),
            new ResolvableConfigItem<IParamNotice<ICommonRole>, CampRoleNotice>("CampRoleCreated"),
            new ResolvableConfigItem<IParamNotice<ICommonRole>, ParamNotice<ICommonRole>>("SetUserRole"),
            new ResolvableConfigItem<IParamNotice<KLRoleComponent>, ParamNotice<KLRoleComponent>>("PlayerRole_0"),
            new ResolvableConfigItem<IParamNotice<KLCamerasComponent>, ParamNotice<KLCamerasComponent>>("SetLensParamer"),
        };

        public static readonly IResolvableConfig[] ServerConfigs = MainServer.ServerConfigs.ContactToArr(KLServerConfigs);
    }

}