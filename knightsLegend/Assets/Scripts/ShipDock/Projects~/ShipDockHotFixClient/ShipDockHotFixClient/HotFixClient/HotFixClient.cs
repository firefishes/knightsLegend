using ShipDock.Applications;
using ShipDock.Commons;
using ShipDock.Config;
using ShipDock.Datas;
using ShipDock.FSM;
using ShipDock.Modulars;
using ShipDock.Notices;
using ShipDock.Tools;
using System.Collections.Generic;

namespace ShipDock.HotFix
{
    public class HotFixClient
    {
        private static HotFixClient instance;

        public static HotFixClient Instance
        {
            get
            {
                if (instance == default)
                {
                    instance = new HotFixClient();
                }
                return instance;
            }
        }

        public ConfigHelper Configs { get; private set; }
        public DecorativeModulars Modulars { get; private set; }
        public DataWarehouse Datas { get; private set; }
        public Network.NetDistributer NetDistributer { get; private set; }
        public StateMachines FSMs { get; private set; }
        public SoundEffects Sounds { get; private set; }

        private IConfig SettingsConfig { get; set; }

        private KeyValueList<IState, IUpdate> mStateMapper;
        private KeyValueList<IStateMachine, IUpdate> mFSMMapper;

        private HotFixClient()
        {
            "log".Log("HotFixClient");

            mStateMapper = new KeyValueList<IState, IUpdate>();
            mFSMMapper = new KeyValueList<IStateMachine, IUpdate>();

            Modulars = new DecorativeModulars();
            Configs = new ConfigHelper();
            Datas = new DataWarehouse();
            FSMs = new StateMachines
            {
                FSMFrameUpdater = OnFSMFrameUpdater,
                StateFrameUpdater = OnStateFrameUpdatr,
            };
            Sounds = new SoundEffects();
            Sounds.Init();

            NetDistributer = new Network.NetDistributer();
            NetDistributer.Init();

            Framework.Instance.ReloadUnit(new IFrameworkUnit[] {
                Framework.Instance.CreateUnitByBridge(Framework.UNIT_MODULARS, Modulars),
                Framework.Instance.CreateUnitByBridge(Framework.UNIT_CONFIG, Configs),
                Framework.Instance.CreateUnitByBridge(Framework.UNIT_DATA, Datas),
                Framework.Instance.CreateUnitByBridge(Framework.UNIT_FSM, FSMs),
                Framework.Instance.CreateUnitByBridge(9, Sounds),
            });
        }

        private void OnStateFrameUpdatr(IState state, bool isAdd)
        {
            if (isAdd)
            {
                if (!mStateMapper.ContainsKey(state))
                {
                    MethodUpdater updater = new MethodUpdater()
                    {
                        Update = state.UpdateState
                    };
                    mStateMapper[state] = updater;
                    UpdaterNotice.AddSceneUpdater(updater);
                }
            }
            else
            {
                IUpdate updater = mStateMapper.GetValue(state, true);
                UpdaterNotice.RemoveSceneUpdater(updater);
            }
        }

        private void OnFSMFrameUpdater(IStateMachine fsm, bool isAdd)
        {
            if (isAdd)
            {
                if (!mFSMMapper.ContainsKey(fsm))
                {
                    MethodUpdater updater = new MethodUpdater()
                    {
                        Update = fsm.UpdateState
                    };
                    mFSMMapper[fsm] = updater;
                    UpdaterNotice.AddSceneUpdater(updater);
                }
            }
            else
            {
                IUpdate updater = mFSMMapper.GetValue(fsm, true);
                UpdaterNotice.RemoveSceneUpdater(updater);
            }
        }

        public void Clean()
        {
            Modulars.Dispose();
            Datas.Dispose();
        }

        public void AddConfig(int dataName, int groupName, ref ConfigsResult result)
        {
            ConfigData data = Datas.GetData<ConfigData>(dataName);
            data.AddConfigs(groupName, result);
        }

        public Dictionary<int, ConfigT> GetConfig<ConfigT>(int dataName, int groupName, string configName, out int statu) where  ConfigT : IConfig, new()
        {
            ConfigData data = Datas.GetData<ConfigData>(dataName);
            ConfigsResult configs = data.GetConfigs(groupName);
            Dictionary<int, ConfigT> dic = configs.GetConfigRaw<ConfigT>(configName, out statu);
            return dic;
        }

        public ConfigT GetConfig<ConfigT>(int dataName, int groupName, string configName, int id) where ConfigT : IConfig, new()
        {
            ConfigData data = Datas.GetData<ConfigData>(dataName);
            ConfigsResult configs = data.GetConfigs(groupName);
            Dictionary<int, ConfigT> mapper = configs.GetConfigRaw<ConfigT>(configName, out int statu);
            return statu == 0 ? mapper[id] : default;
        }

        public void SetGlobalSettings(IConfig value)
        {
            SettingsConfig = value;
        }

        public T GlobalSettins<T>() where T : IConfig
        {
            return (T)SettingsConfig;
        }
    }
}
