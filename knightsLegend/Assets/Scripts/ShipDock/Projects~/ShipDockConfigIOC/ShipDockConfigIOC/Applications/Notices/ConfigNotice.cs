using ShipDock.Notices;

namespace ShipDock.Applications
{
    public class ConfigNotice : ParamNotice<string[]>
    {
        public ConfigsResult Result { get; set; }
    }
}
