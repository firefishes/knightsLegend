using ShipDock.Datas;
using ShipDock.Tools;

namespace ShipDock.Applications
{
    public class ConfigData : DataProxy
    {
        private KeyValueList<int, ConfigsResult> mConfigs;

        public ConfigData(int name) : base(name)
        {
            mConfigs = new KeyValueList<int, ConfigsResult>();
        }

        public bool HasConfigGroup(int name)
        {
            return mConfigs != default && mConfigs.ContainsKey(name);
        }

        public void AddConfigs(int name, ConfigsResult results)
        {
            if (HasConfigGroup(name))
            {
                ConfigsResult temp = GetConfigs(name);
                temp.Clean();
            }
            mConfigs[name] = results;
        }

        public void RemoveConfigs(int name)
        {
            ConfigsResult configs = mConfigs.Remove(name);
            configs.Clean();
        }

        public ConfigsResult GetConfigs(int name)
        {
            return HasConfigGroup(name) ? mConfigs[name] : default;
        }
    }
}
