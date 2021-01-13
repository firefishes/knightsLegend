#define _G_LOG

using ShipDock.Config;
using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Server;

namespace ShipDock.Applications
{
    public class ConfigServer : Server.Server
    {
        public bool hasLocalsConfig;
        private ConfigHelper mHelper;

        public ConfigServer(string serverName = ShipDockConsts.SERVER_CONFIG) : base(serverName) { }

        public override void InitServer()
        {
            base.InitServer();

            ServersHolder.AddResolvableConfig(new ResolvableConfigItem<IConfigNotice, ConfigNotice>("ConfigNotice"));

            Register<IConfigNotice>(GetConfigNotice, Pooling<ConfigNotice>.Instance);

            if(hasLocalsConfig)
            {
                Register<IConfigHolder>(GetLocalCNConfig);
            }

            mHelper = new ConfigHelper();

            SetConfigCreater("Locals_CN");
        }

        private void SetConfigCreater(string name)
        {
            mHelper.AddHolderType(name, () =>
            {
                return Resolve<IConfigHolder>(name);
            });
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
            Add<IParamNotice<string>>(AddConfigCreater);
        }

        [Callable("LoadConfig", "ConfigNotice")]
        private void LoadConfig(ref IConfigNotice target) { }

        [Callable("AddConfigCreater", "string")]
        private void AddConfigCreater(ref IParamNotice<string> target)
        {
            SetConfigCreater(target.ParamValue);
        }
    }
}
