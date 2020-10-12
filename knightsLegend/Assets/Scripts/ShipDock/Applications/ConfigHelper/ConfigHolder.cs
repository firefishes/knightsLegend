using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{
    public class ConfigHolder<C> : IConfigHolder where C : IConfig, new()
    {
        public void SetSource(ref byte[] bytes)
        {
            ByteBuffer buffer = ByteBuffer.Allocate(bytes);

            bool isCRCChecked = false;
            string headinfo = buffer.ReadString();

            C config;
            int size = buffer.ReadInt();
            Config = new Dictionary<int, C>();
            for (int i = 0; i < size; i++)
            {
                config = new C();
                if (!isCRCChecked)
                {
                    isCRCChecked = true;
                    if (headinfo != config.CRCValue)
                    {
                        Debug.Log(typeof(C).Name + " 文件头文件检测失败");
                    }
                }
                config.Parse(buffer);
                Config.Add(config.GetID(), config);
            }
        }

        public C GetData(int id)
        {
            return Config[id];
        }

        public void SetCongfigName(string name)
        {
            ConfigName = name;
        }

        public Dictionary<int, C> Config { get; private set; }

        public string ConfigName { get; private set; }
    }
}