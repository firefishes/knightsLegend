#define _G_LOG

using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Server;
using ShipDock.Tools;
using System.Collections.Generic;
using ConfigLoader = ShipDock.Loader.Loader;

namespace ShipDock.Applications
{
    public class ConfigServer : Server.Server
    {
        public bool hasLocalsConfig;

        private IConfigNotice mLoadConfigNotice;
        private string mConfigLoading;
        private List<string> mConfigReady;
        private Queue<string> mWillLoadNames;
        private KeyValueList<string, IConfigHolder> mConfigHolders;

        public ConfigServer(string serverName = ShipDockConsts.SERVER_CONFIG) : base(serverName) { }

        public override void InitServer()
        {
            base.InitServer();

            ServersHolder.AddResolvableConfig(new ResolvableConfigItem<IConfigNotice, ConfigNotice>("ConfigNotice"));

            mConfigHolders = new KeyValueList<string, IConfigHolder>();

            Register<IConfigNotice>(GetConfigNotice, Pooling<ConfigNotice>.Instance);

            if(hasLocalsConfig)
            {
                Register<IConfigHolder>(GetLocalCNConfig);
            }
        }

        /// <summary>配置消息对象解析器</summary>
        [Resolvable("ConfigNotice")]
        private void GetConfigNotice(ref IConfigNotice target) { }

        [Resolvable("Locals_CN")]
        private void GetLocalCNConfig(ref IConfigHolder target) { }

        public override void ServerReady()
        {
            base.ServerReady();

            Add<IConfigNotice>(LoadConfig);
        }

        private void CreateConfigHolder(ref IConfigNotice target)
        {
            string name;
            IConfigHolder configHolder;
            string[] configNames = target.ParamValue;
            int max = configNames.Length;
            for (int i = 0; i < max; i++)
            {
                name = configNames[i];
                if (mConfigHolders.ContainsKey(name))
                {
                    mConfigReady.Add(name);
                }
                else
                {
                    configHolder = Resolve<IConfigHolder>(name);
                    configHolder.SetCongfigName(name);
                    mConfigHolders[name] = configHolder;

                    mWillLoadNames.Enqueue(name);
                }
            }
        }

        [Callable("LoadConfig", "ConfigNotice")]
        private void LoadConfig(ref IConfigNotice target)
        {
            if (mWillLoadNames != default)
            {
                Utils.Reclaim(ref mWillLoadNames, false);
            }
            mConfigReady = new List<string>();
            mWillLoadNames = new Queue<string>();

            mLoadConfigNotice = target;

            CreateConfigHolder(ref target);

            ConfigLoader loader = default;
            LoaderConfirm(true, loader);
        }

        private void LoaderConfirm(bool flag, ConfigLoader loader)
        {
            if (!flag)
            {
                "error: Config load failed, message is {0}".Log(loader.LoadError);
            }
            if (mWillLoadNames.Count > 0)
            {
                LoadConfigItem(ref loader);
            }
            else
            {
                if (loader != default)
                {
                    ParseConfigHolder(loader.ResultData);
                }
                ConfigResultReady();
            }
        }

        private void ConfigResultReady()
        {
            string configName;
            int max = mConfigReady.Count;
            IConfigHolder[] holders = new IConfigHolder[max];
            for (int i = 0; i < max; i++)
            {
                configName = mConfigReady[i];
                holders[i] = mConfigHolders[configName];
            }

            Utils.Reclaim(ref mConfigReady);

            mLoadConfigNotice.SetConfigHolders(holders);

            int noticeName = mLoadConfigNotice.Name;
            noticeName.Broadcast(mLoadConfigNotice);
            mLoadConfigNotice = default;
        }

        private void ParseConfigHolder(byte[] vs)
        {
            IConfigHolder holder = mConfigHolders[mConfigLoading];
            holder.SetSource(ref vs);
            mConfigReady.Add(mConfigLoading);
        }

        private void LoadConfigItem(ref ConfigLoader loader)
        {
            if (loader != default)
            {
                ParseConfigHolder(loader.ResultData);

                loader.Dispose();
            }
            else
            {
                loader = new ConfigLoader();
            }
            mConfigLoading = mWillLoadNames.Dequeue();
            loader.CompleteEvent.AddListener(LoaderConfirm);
            string url = AppPaths.StreamingResDataRoot.Append("configs/", mConfigLoading, ".bin");//"res_data/configs/LocalsCN.bin"
            loader.Load(url);
        }

        private ConfigHolder<C> GetHolder<C>(string name) where C : IConfig, new()
        {
            return (ConfigHolder<C>)mConfigHolders[name];
        }
    }
}
