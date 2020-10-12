using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Tools;
using System.Collections.Generic;

namespace ShipDock.Applications
{
    public class ConfigNotice : ParamNotice<string[]>, IConfigNotice
    {
        public void SetConfigHolders(params IConfigHolder[] args)
        {
            ConfigHolders = new KeyValueList<string, IConfigHolder>();

            string name;
            int max = args.Length;
            for (int i = 0; i < max; i++)
            {
                name = args[i].ConfigName;
                ConfigHolders[name] = args[i];
            }
        }

        public override void ToPool()
        {
            if (IsClearHolderList)
            {
                ConfigHolders.Clear();
            }
            ConfigHolders = default;
            IsClearHolderList = false;

            Pooling<ConfigNotice>.To(this);
        }

        public Dictionary<int, T> GetConfigRaw<T>(string name) where T : IConfig, new()
        {
            ConfigHolder<T> holder = ConfigHolders[name] as ConfigHolder<T>;
            return holder.Config;
        }

        public bool IsClearHolderList { get; set; }
        public KeyValueList<string, IConfigHolder> ConfigHolders { get; private set; }
    }

}