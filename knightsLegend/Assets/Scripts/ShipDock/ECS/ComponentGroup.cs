using ShipDock.Applications;
using ShipDock.Tools;
using System.Collections.Generic;

namespace ShipDock.ECS
{
    public class ComponentGroup<C, K> where C : IShipDockComponent
    {
        public ComponentGroup()
        {
        }

        public ComponentGroup(K[] keys, int[] componentNames) : this(ShipDockApp.Instance.Components, ref keys, ref componentNames)
        {
        }

        public ComponentGroup(IShipDockComponentContext context, ref K[] keys, ref int[] componentNames)
        {
            int max = componentNames.Length;
            Group = new KeyValueList<K, C>(max);

            C groupItem;
            for (int i = 0; i < max; i++)
            {
                groupItem = (C)context.RefComponentByName(componentNames[i]);
                Group.Put(keys[i], groupItem);
            }
        }

        public void SetGroupMapper(ref KeyValueList<K, C> list)
        {
            Group = list;
        }

        public void Clean()
        {
            Group = default;
        }

        public C GetGroupComponent(K key)
        {
            return Group[key];
        }

        private KeyValueList<K, C> Group { get; set; }
    }

    public class DataComponentGroup<C, K> where C : IShipDockComponent, IDataValidable
    {
        public DataComponentGroup(K[] keys, int[] componentNames) : this(ShipDockApp.Instance.Components, ref keys, ref componentNames)
        {
        }

        public DataComponentGroup(IShipDockComponentContext context, ref K[] keys, ref int[] componentNames)
        {
            int max = componentNames.Length;
            Group = new KeyValueList<K, C>(max);
            
            C groupItem;
            for (int i = 0; i < max; i++)
            {
                groupItem = (C)context.RefComponentByName(componentNames[i]);
                Group.Put(keys[i], groupItem);
            }
        }

        public void Clean()
        {
            Group.Clear();
        }

        public C GetGroupComponent(K key)
        {
            return Group[key];
        }

        public void SetDataVailid<E>(ref E target, bool value) where E : IShipDockEntitas
        {
            C groupItem;
            List<C> list = Group.Values;
            int max = list.Count;
            for (int i = 0; i < max; i++)
            {
                groupItem = list[i];
                groupItem.SetDataValidable(value, ref target);
            }
        }

        private KeyValueList<K, C> Group { get; set; }
    }
}