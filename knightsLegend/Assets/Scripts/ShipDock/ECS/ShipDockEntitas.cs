using ShipDock.Tools;
using System.Collections.Generic;

namespace ShipDock.ECS
{
    public class ShipDockEntitas : IShipDockEntitas
    {
        private List<int> mBindedToComponentIDs = new List<int>();

        public virtual void InitComponents()
        {
        }

        public virtual void Dispose()
        {
            WillDestroy = true;
        }

        public void AddComponent(IShipDockComponent component)
        {
            if ((component != default) && !HasComponent(component.ID))
            {
                int id = component.SetEntitas(this);
                ComponentList.Add(component.ID);
                mBindedToComponentIDs.Add(id);
            }
        }

        public void SetEntitasID(int id)
        {
            if (ID == int.MaxValue)
            {
                ID = id;
            }
        }

        public void RemoveComponent(IShipDockComponent component)
        {
            if ((component != default) && HasComponent(component.ID))
            {
                int id = component.ID;

                int index = ComponentList.IndexOf(id);
                if (index >= 0)
                {
                    int entitasID = mBindedToComponentIDs[index];
                    mBindedToComponentIDs.RemoveAt(index);
                    ComponentList.Remove(id);
                    component.DropEntitas(this, entitasID);
                }
            }
            if (WillDestroy)
            {
                if ((ComponentList != default) && (ComponentList.Count == 0))
                {
                    List<int> list = ComponentList;
                    Utils.Reclaim(ref list);
                    Utils.Reclaim(ref mBindedToComponentIDs);
                    ID = int.MaxValue;
                }
            }
        }

        public bool HasComponent(int componentID)
        {
            return (ComponentList != default) && ComponentList.Contains(componentID);
        }

        public int FindEntitasInComponent(IShipDockComponent component)
        {
            int id = component.ID;
            if (HasComponent(id))
            {
                id = ComponentList.IndexOf(id);
                id = mBindedToComponentIDs[id];
            }
            else
            {
                id = -1;
            }
            return id;
        }

        public List<int> ComponentList { get; } = new List<int>();
        public int ID { get; private set; } = int.MaxValue;
        public bool WillDestroy { get; private set; } = false;
    }
}
