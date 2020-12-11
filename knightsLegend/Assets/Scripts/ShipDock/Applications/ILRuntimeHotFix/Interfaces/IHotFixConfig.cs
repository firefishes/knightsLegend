using ILRuntime.Runtime.Enviorment;
using AutoAdaptorGeneratesDic = System.Collections.Generic.Dictionary<string, System.Type>;

namespace ShipDock.Applications
{
    public interface IHotFixConfig
    {
        string SpaceName { get; set; }
        System.Action<AppDomain> RegisterMethods { get; set; }
        CrossBindingAdaptor[] Adaptors { get; set; }
        AutoAdaptorGeneratesDic AutoAdaptorGenerates { get; set; }
    }
}