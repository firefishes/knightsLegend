namespace ShipDock.Server
{
    public interface IResolverCacher<InterfaceT>
    {
        void SetDelegate(ResolveDelegate<InterfaceT> target);
        ResolveDelegate<InterfaceT> DelegateTarget { get; }
    }
}