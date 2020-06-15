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

            int[] list = ComponentNames;
            Utils.Reclaim(ref list);
        }

        public override void InitComponents()
        {
            base.InitComponents();

            IShipDockComponent component;
            var manager = ShipDockApp.Instance.Components;
            int name;
            int max = ComponentNames != default ? ComponentNames.Length : 0;
            for (int i = 0; i < max; i++)
            {
                name = ComponentNames[i];
                component = manager.RefComponentByName(name);
                AddComponent(component);
            }
        }

        public T GetComponentFromEntitas<T>(int aid) where T : IShipDockComponent
        {
            T result = default;
            if (HasComponent(aid))
            {
                int index = ComponentList.IndexOf(aid);
                result = (T)ShipDockApp.Instance.Components.RefComponentByName(ComponentList[index]);
            }
            return result;
        }

        protected abstract int[] ComponentNames { get; }
    }
}
