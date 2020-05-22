using ShipDock.Applications;
using ShipDock.Notices;
using ShipDock.Server;

namespace KLGame
{
    public static class RoleAnimationFeedBackConsts
    {
        public const int FEED_BACK_DEFAULT = -1;
        public const int FEED_BACK_BY_HIT = 0;
    }

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
        public const string S_BATTLE = "ServerBattle";

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
        public const int C_ROLE_BATTLE_DATA = 10;
        public const int C_ROLE_AI_DEF = 11;

        public const int D_GAME = 0;
        public const int D_PLAYER = 1;
        public const int D_CONFIG = 2;
        public const int D_BATTLE = 3;

        public const int N_TRIGGER_HIT = 1000;
        public const int N_TRIGGER_ROLE_ACTIVE = 1001;
        public const int N_BRAK_WORKING_AI = 1002;
        public const int N_AI_RESET = 1003;
        public const int N_AFTER_UNDER_ATTACK = 1004;
        //public const int N_INIT_ENTITAS_CALLBACKS = 1005;
        public const int N_MOVE_BLOCK = 1006;
        public const int N_MOVE_UNBLOCK = 1007;
        public const int N_ENEMY_AI_ANTICIPATION = 1008;
        public const int N_AI_ANTICIPATION = 1009;

        public const int DC_CAMP_ROLE_CREATED = 2000;
        
        public const int ROLE_INPUT_TYPE_DEFAULT = 0;
        public const int ROLE_INPUT_TYPE_ENEMY = 1;

        public const int RFSM_MAIN_MALE_ROLE = 0;
        public const int RFSM_NORMAL_ENMEY = 1;

        public const int T_ROLE_1 = 0;
        public const int T_ROLE_2 = 1;
        public const int T_ROLE_STATE_DELAY_FINISH = 2;
        public const int T_ROLE_STATE_FEED_BACK = 3;
        //public const int T_AI_ATK_TIME = 4;
        public const int T_AI_ATK_HIT_TIME = 5;
        public const int T_AI_THINKING = 6;

        public const int T_AI_THINKING_TIME_TASK_ATK = 0;
        public const int T_AI_THINKING_TIME_TASK_DEF = 1;

        public const int ENEMY_INPUT_PHASE_ATTACK_AI = 6;
        //public const int ENEMY_INPUT_PHASE_UPDATE_NROMAL_ATK_TRIGGER_TIME = 7;
        //public const int ENEMY_INPUT_PHASE_NROMAL_ATK = 8;
        public const int ENEMY_INPUT_PHASE_AFTER_NROMAL_ATK = 9;
        
        public const int FIELD_HP = 0;
        public const int FIELD_M_HP = 1;
        public const int FIELD_QI = 2;
        public const int FIELD_M_QI = 3;
        public const int FIELD_IN_POWER = 4;
        public const int FIELD_M_IN_POWER = 5;
        public const int FIELD_FlAWS = 6;
        public const int FIELD_M_FlAWS = 7;

        public const int FIELD_MOVE_SPEED = 105;
        public const int FIELD_MOVING_TURN_SPEED = 106;
        public const int FIELD_STATIONARY_TURN_SPEED = 107;

        private static readonly IResolvableConfig[] KLServerConfigs =
        {
            //new ResolvableConfigItem<INotice, GameNotice>("GameNotice"),
            new ResolvableConfigItem<IParamNotice<ICommonRole>, CampRoleNotice>("CampRoleCreated"),
            new ResolvableConfigItem<IParamNotice<ICommonRole>, ParamNotice<ICommonRole>>("SetUserRole"),
            new ResolvableConfigItem<IParamNotice<ICommonRole>, ParamNotice<ICommonRole>>("SetBattleRoleParam"),
            new ResolvableConfigItem<IParamNotice<KLRoleComponent>, ParamNotice<KLRoleComponent>>("PlayerRole_0"),
            new ResolvableConfigItem<IParamNotice<KLCamerasComponent>, ParamNotice<KLCamerasComponent>>("SetLensParamer"),
            new ResolvableConfigItem<IParamNotice<NormalATKStateParam>, ParamNotice<NormalATKStateParam>>("NormalATKStateParam"),
            new ResolvableConfigItem<IParamNotice<IKLRole>, ParamNotice<IKLRole>>("KLRole"),
        };

        public static readonly IResolvableConfig[] ServerConfigs = MainServer.ServerConfigs.ContactToArr(KLServerConfigs);
    }

}