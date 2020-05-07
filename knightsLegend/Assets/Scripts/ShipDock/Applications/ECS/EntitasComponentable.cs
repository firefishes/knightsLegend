using ShipDock.ECS;
using ShipDock.Tools;

namespace ShipDock.Applications
{
    public interface IEntitasComponentable : IShipDockEntitas
    {
        T GetComponentFromEntitas<T>(int aid) where T : IShipDockComponent;
    }
    
    public abstract class EntitasComponentable : ShipDockEntitas, IEntitasComponentable
    {
        public override void Dispose()
        {
            base.Dispose();

            int[] list = ComponentIDs;
            Utils.Reclaim(ref list);
        }

        public override void InitComponents()
        {
            base.InitComponents();

            IShipDockComponent component;
            var manager = ShipDockApp.Instance.Components;
            int aid;
            int max = ComponentIDs != default ? ComponentIDs.Length : 0;
            for (int i = 0; i < max; i++)
            {
                aid = ComponentIDs[i];
                component = manager.GetComponentByAID(aid);
                AddComponent(component);
            }
        }

        public T GetComponentFromEntitas<T>(int aid) where T : IShipDockComponent
        {
            T result = default;
            if (HasComponent(aid))
            {
                int index = ComponentList.IndexOf(aid);
                result = (T)ShipDockApp.Instance.Components.GetComponentByAID(ComponentList[index]);
            }
            return result;
        }

        protected abstract int[] ComponentIDs { get; }
    }
}
