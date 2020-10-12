using ShipDock.Notices;
using ShipDock.Tools;
using System.Collections.Generic;

namespace ShipDock.Applications
{
    public interface IConfigNotice : IParamNotice<string[]>
    {
        Dictionary<int, T> GetConfigRaw<T>(string name) where T : IConfig, new();
        void SetConfigHolders(params IConfigHolder[] args);
        KeyValueList<string, IConfigHolder> ConfigHolders { get; }
        bool IsClearHolderList { get; set; }
    }
}