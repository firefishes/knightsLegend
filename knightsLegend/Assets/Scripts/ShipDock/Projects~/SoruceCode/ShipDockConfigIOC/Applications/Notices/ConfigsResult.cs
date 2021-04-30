using ShipDock.Config;
using ShipDock.Tools;
using System.Collections.Generic;

namespace ShipDock.Applications
{
    public class ConfigsResult
    {

        public bool IsClearHolderList
        {
            get
            {
                return mIsClearHolderList;
            }
            set
            {
                mIsClearHolderList = value;

                if (mIsClearHolderList)
                {
                    Clean();
                }
            }
        }

        public KeyValueList<string, IConfigHolder> ConfigHolders { get; private set; }

        private bool mIsClearHolderList;

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

        public void Clean()
        {
            if (IsClearHolderList)
            {
                ConfigHolders.Clear();
            }
            ConfigHolders = default;
            IsClearHolderList = false;
        }

        public Dictionary<int, T> GetConfigRaw<T>(string name, out int statu) where T : IConfig, new()
        {
            statu = 0;
            if (ConfigHolders == default)
            {
                statu = 1;
                return default;
            }
            ConfigHolder<T> holder = ConfigHolders[name] as ConfigHolder<T>;
            return holder.Config;
        }
    }

}