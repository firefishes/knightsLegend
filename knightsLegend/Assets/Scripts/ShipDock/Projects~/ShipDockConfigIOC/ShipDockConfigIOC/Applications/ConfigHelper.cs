using ShipDock.Config;
using ShipDock.Loader;
using ShipDock.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{
    public class ConfigHelper
    {
        private string mABName;
        private string mConfigLoading;
        private Action<ConfigsResult> mLoadConfigNotice;
        private Queue<string> mWillLoadNames;
        private List<string> mConfigReady;
        private KeyValueList<string, IConfigHolder> mConfigHolders;
        private KeyValueList<string, Func<IConfigHolder>> mConfigHolderCreater;

        public ConfigHelper()
        {
            mConfigHolders = new KeyValueList<string, IConfigHolder>();
            mConfigHolderCreater = new KeyValueList<string, Func<IConfigHolder>>();
        }

        public void AddHolderType<T>(string name) where T : IConfig, new()
        {
            Func<IConfigHolder> creater = () =>
            {
                return new ConfigHolder<T>();
            };
            AddHolderType(name, creater);
        }

        public void AddHolderType(string name, Func<IConfigHolder> creater = default)
        {
            mConfigHolderCreater[name] = creater;
        }

        public void Load(string configABName, Action<ConfigsResult> target, params string[] configNames)
        {
            mABName = configABName;
            if (mWillLoadNames != default)
            {
                Utils.Reclaim(ref mWillLoadNames, false);
            }
            mConfigReady = new List<string>();
            mWillLoadNames = new Queue<string>();

            mLoadConfigNotice = target;

            CreateConfigHolder(ref configNames);

            LoaderConfirm(default);
        }

        private void CreateConfigHolder(ref string[] configNames)
        {
            string name;
            IConfigHolder configHolder;
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
                    configHolder = GetHolder(name);
                    configHolder.SetCongfigName(name);
                    mConfigHolders[name] = configHolder;

                    mWillLoadNames.Enqueue(name);
                }
            }
        }

        private IConfigHolder GetHolder(string name)
        {
            Func<IConfigHolder> func = mConfigHolderCreater[name];
            return func.Invoke();
        }

        private void LoaderConfirm(byte[] vs)
        {
            if (mWillLoadNames.Count > 0)
            {
                LoadConfigItem(ref vs);
            }
            else
            {
                if (vs != default)
                {
                    ParseConfigHolder(vs);
                }
                ConfigResultReady();
            }
        }

        private void LoadConfigItem(ref byte[] vs)
        {
            if (vs != default)
            {
                ParseConfigHolder(vs);
            }

            mConfigLoading = mWillLoadNames.Dequeue();
            AssetBundles abs = Framework.Instance.GetUnit<AssetBundles>(Framework.UNIT_AB);
            TextAsset data = abs.Get<TextAsset>(mABName, mConfigLoading);

            LoaderConfirm(data.bytes);
        }

        private void ParseConfigHolder(byte[] vs)
        {
            IConfigHolder holder = mConfigHolders[mConfigLoading];
            holder.SetSource(ref vs);
            mConfigReady.Add(mConfigLoading);
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

            ConfigsResult configsResult = new ConfigsResult();
            configsResult.SetConfigHolders(holders);

            mLoadConfigNotice?.Invoke(configsResult);
            mLoadConfigNotice = default;
        }
    }
}
