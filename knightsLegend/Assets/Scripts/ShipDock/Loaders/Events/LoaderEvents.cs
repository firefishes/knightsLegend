using UnityEngine.Events;

namespace ShipDock.Loader
{
    public class OnLoaderProgress : UnityEvent<Loader> { };
    public class OnLoaderCompleted : UnityEvent<bool, Loader> { };
    public class OnAssetLoaderCompleted : OnLoaderCompleted { };
}
