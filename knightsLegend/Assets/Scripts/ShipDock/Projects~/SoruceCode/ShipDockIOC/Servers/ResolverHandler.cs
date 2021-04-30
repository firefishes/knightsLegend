namespace ShipDock.Server
{
    public class ResolverHandler<InterfaceT> : IResolverHandler, 
                                               IResolverCacher<InterfaceT>, 
                                               IResolverParamer<InterfaceT>
    {
        private InterfaceT mParamTemp;

        public void Dispose()
        {
            OnlyOnce = false;
            mParamTemp = default;
            ResolverParam = default;
            DelegateTarget = default;
        }

        public void SetDelegate(ResolveDelegate<InterfaceT> target)
        {
            DelegateTarget = target;
        }

        public void AddDelegate(ResolveDelegate<InterfaceT> target)
        {
            ResolveDelegate<InterfaceT> cur = default;
            cur += target;
            cur += DelegateTarget;
            DelegateTarget = cur;
        }

        public void RemoveDelegate(ResolveDelegate<InterfaceT> target)
        {
            DelegateTarget -= target;
        }

        public void InvokeResolver()
        {
            mParamTemp = (InterfaceT)ResolverParam;
            DelegateTarget(ref mParamTemp);
            ResolverParam = mParamTemp;
        }

        public void SetParam<T>(ref T param)
        {
            ResolverParam = param;
        }

        public void SetID(int id)
        {
            ID = id;
        }

        public ResolveDelegate<InterfaceT> DelegateTarget { get; private set; }
        public object ResolverParam { get; set; }
        public bool OnlyOnce { get; set; }
        public int ID { get; private set; }
    }

}
