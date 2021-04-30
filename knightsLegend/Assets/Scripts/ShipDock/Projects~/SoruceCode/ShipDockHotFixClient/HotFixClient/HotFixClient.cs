using ShipDock.Applications;
using ShipDock.Commons;
using ShipDock.Config;
using ShipDock.Datas;
using ShipDock.FSM;
using ShipDock.Modulars;
using ShipDock.Network;
using ShipDock.Notices;
using ShipDock.Sounds;
using ShipDock.Tools;
using System;
using System.Collections.Generic;

public static class HotFixClientExtensions
{
    public static Dictionary<int, ConfigT> GetConfig<ConfigT>(this string configName) where ConfigT : IConfig, new()
    {
        Dictionary<int, ConfigT> result = default;
        int dataName = ShipDock.HotFix.HotFixClient.configsDataName;
        int modularName = ShipDock.HotFix.HotFixClient.configsModularName;
        if (dataName != int.MaxValue && modularName != int.MaxValue)
        {
            result = ShipDock.HotFix.HotFixClient.Instance.GetConfig<ConfigT>(dataName, modularName, configName, out _);
        }
        else { }

        return result;
    }
}

namespace ShipDock.HotFix
{
    /// <summary>
    /// 
    /// 完全热更新客户端
    /// 
    /// 用于支持完全使用ILRuntime热更方式运行应用程序
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class HotFixClient
    {
        public static int configsDataName = int.MaxValue;
        public static int configsModularName = int.MaxValue;

        private static HotFixClient instance;

        public static HotFixClient Instance
        {
            get
            {
                if (instance == default)
                {
                    instance = new HotFixClient();
                }
                else { }
                return instance;
            }
        }

        /// <summary>状态机中各个状态帧更新器的映射</summary>
        private KeyValueList<IState, IUpdate> mStateMapper;
        /// <summary>各个状态机帧更新器的映射</summary>
        private KeyValueList<IStateMachine, IUpdate> mFSMMapper;
        /// <summary>普通帧更新器的映射</summary>
        private KeyValueList<Action<int>, MethodUpdater> mUpdaterMapper;
        /// <summary>业务逻辑的全局配置</summary>
        private IConfig SettingsConfig { get; set; }

        /// <summary>业务逻辑配置解析容器绑定辅助器</summary>
        public ConfigHelper Configs { get; private set; }
        /// <summary>业务逻辑大模块管理器</summary>
        public DecorativeModulars Modulars { get; private set; }
        /// <summary>数据曾总管理器</summary>
        public DataWarehouse Datas { get; private set; }
        /// <summary>网络请求分发器</summary>
        public NetDistributer NetDistributer { get; private set; }
        /// <summary>状态机管理器</summary>
        public StateMachines FSMs { get; private set; }
        /// <summary>声音管理器</summary>
        public SoundEffects Sounds { get; private set; }
        /// <summary>主工程的热更入口组件</summary>
        public HotFixerComponent HotFixEnter { get; set; }

        public int PlaySoundNoticeName { get; set; } = int.MaxValue;
        public int PlayBGMNoticeName { get; set; } = int.MaxValue;
        public int StopBGMNoticeName { get; set; } = int.MaxValue;
        public int AddSoundsNoticeName { get; set; } = int.MaxValue;
        public int RemoveSoundsNoticeName { get; set; } = int.MaxValue;

        private HotFixClient()
        {
            "log".Log("HotFixClient");

            mStateMapper = new KeyValueList<IState, IUpdate>();
            mFSMMapper = new KeyValueList<IStateMachine, IUpdate>();
            mUpdaterMapper = new KeyValueList<Action<int>, MethodUpdater>();

            Modulars = new DecorativeModulars();
            Configs = new ConfigHelper();
            Datas = new DataWarehouse();
            FSMs = new StateMachines
            {
                FSMFrameUpdater = OnFSMFrameUpdater,
                StateFrameUpdater = OnStateFrameUpdater,
            };

            Sounds = new SoundEffects();
            Sounds.Init();

            NetDistributer = new NetDistributer();
            NetDistributer.Init();

            #region 对主工程框架重填充各热更端的功能单元，以使热更端获得原框架相同的所有功能
            Framework framework = Framework.Instance;
            framework.ReloadUnit(new IFrameworkUnit[] {
                framework.CreateUnitByBridge(Framework.UNIT_MODULARS, Modulars),
                framework.CreateUnitByBridge(Framework.UNIT_CONFIG, Configs),
                framework.CreateUnitByBridge(Framework.UNIT_DATA, Datas),
                framework.CreateUnitByBridge(Framework.UNIT_FSM, FSMs),
                framework.CreateUnitByBridge(Framework.UNIT_SOUND, Sounds),
            });
            #endregion
        }

        public void SetSoundEffectNotices(int playSound, int playBGM, int stopBGM, int addSounds, int removeSounds)
        {
            PlaySoundNoticeName = playSound;
            PlayBGMNoticeName = playBGM;
            StopBGMNoticeName = stopBGM;
            AddSoundsNoticeName = addSounds;
            RemoveSoundsNoticeName = removeSounds;

            CheckAndAddSoundsNotice(PlaySoundNoticeName, OnPlaySound);
            CheckAndAddSoundsNotice(PlayBGMNoticeName, OnPlayBGM);
            CheckAndAddSoundsNotice(StopBGMNoticeName, OnStopBGM);
            CheckAndAddSoundsNotice(AddSoundsNoticeName, OnAddSounds);
            CheckAndAddSoundsNotice(RemoveSoundsNoticeName, OnRemoveSounds);
        }

        private void CheckAndAddSoundsNotice(int noticeName, Action<INoticeBase<int>> handler)
        {
            if (noticeName != int.MaxValue)
            {
                noticeName.Add(handler);
            }
            else { }
        }

        private void OnRemoveSounds(INoticeBase<int> param)
        {
            if (param is IParamNotice<int> soundNotice)
            {
                Sounds.RemoveSound(soundNotice.ParamValue);
            }
            else { }
        }

        private void OnAddSounds(INoticeBase<int> param)
        {
            if (param is IParamNotice<List<SoundItem>> soundNotice)
            {
                Sounds.SetPlayList(soundNotice.ParamValue.ToArray());
                soundNotice.ParamValue.Clear();
            }
            else { }
        }

        private void OnPlaySound(INoticeBase<int> param)
        {
            if (param is IParamNotice<string> soundNotice)
            {
                Sounds.PlaySound(soundNotice.ParamValue);
            }
            else { }
        }

        private void OnPlayBGM(INoticeBase<int> param)
        {
            if (param is IParamNotice<string> soundNotice)
            {
                Sounds.PlayBGM(soundNotice.ParamValue);
            }
            else { }
        }

        private void OnStopBGM(INoticeBase<int> param)
        {
            Sounds.StopBGM();
        }

        private void OnStateFrameUpdater(IState state, bool isAdd)
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
                else { }
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
                else { }
            }
            else
            {
                IUpdate updater = mFSMMapper.GetValue(fsm, true);
                UpdaterNotice.RemoveSceneUpdater(updater);
            }
        }

        public void Clean()
        {
            PlaySoundNoticeName.Remove(OnPlaySound);
            PlayBGMNoticeName.Remove(OnPlayBGM);
            StopBGMNoticeName.Remove(OnStopBGM);
            AddSoundsNoticeName.Remove(OnAddSounds);
            RemoveSoundsNoticeName.Remove(OnRemoveSounds);

            Modulars.Dispose();
            Datas.Dispose();
        }

        public void InitGroupConfigs(int dataName, int groupName, ref ConfigsResult result)
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

        public void AddUpdate(Action<int> method)
        {
            if (!mUpdaterMapper.ContainsKey(method))
            {
                MethodUpdater updater = new MethodUpdater
                {
                    Update = method
                };
                mUpdaterMapper[method] = updater;
                UpdaterNotice.AddSceneUpdater(updater);
            }
            else { }
        }

        public void RemoveUpdate(Action<int> method)
        {
            if (mUpdaterMapper.ContainsKey(method))
            {
                MethodUpdater updater = mUpdaterMapper.GetValue(method, true);
                UpdaterNotice.RemoveSceneUpdater(updater);
                updater.Dispose();
            }
            else { }
        }

        public void StartCorutine(System.Collections.IEnumerator target)
        {
            HotFixEnter.StartCoroutine(target);
        }
    }
}
