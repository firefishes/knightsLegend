using ShipDock.Tools;
using System.Collections.Generic;

namespace ShipDock.ECS
{
    public abstract class SystemComponent : ShipDockComponent, ISystemComponent
    {
        private KeyValueList<int, IShipDockComponent> mRelatedComponents;

        public SystemComponent()
        {
            IsSystem = true;
            IsSystemChanged = true;
            mRelatedComponents = new KeyValueList<int, IShipDockComponent>();
        }

        public override void Dispose()
        {
            base.Dispose();

            Context = default;

            Utils.Reclaim(ref mRelatedComponents);
        }

        public override void Init(IShipDockComponentManager manager)
        {
            base.Init(manager);

            Context = manager;

            FillRelateComponents(manager);

            List<IShipDockComponent> values = mRelatedComponents.Values;

            IShipDockComponent item;
            int max = values.Count;
            for (int i = 0; i < max; i++)
            {
                item = values[i];
                item.OnEntitasStretch += ComponentEntitasStretch;
            }
        }

        /// <summary>
        /// 填充需要关联的组件
        /// </summary>
        public void FillRelateComponents(IShipDockComponentManager manager)
        {
            int name;
            IShipDockComponent item;
            int max = RelateComponents != default ? RelateComponents.Length : 0;
            for (int i = 0; i < max; i++)
            {
                name = RelateComponents[i];
                if (!mRelatedComponents.ContainsKey(name))
                {
                    item = manager.RefComponentByName(name);
                    if (item != default)
                    {
                        mRelatedComponents[name] = item;
                    }
                }
            }
            bool needCheckReFill = (max > 0) && (mRelatedComponents.Size != max);
            if (needCheckReFill)
            {
                manager.RelateComponentsReFiller += ReFillRelateComponents;
            }
        }

        /// <summary>
        /// 重新填充关联的组件
        /// </summary>
        protected virtual void ReFillRelateComponents(int name, IShipDockComponent target, IShipDockComponentManager manager)
        {
            int item;
            int max = RelateComponents != default ? RelateComponents.Length : 0;
            for (int i = 0; i < max; i++)
            {
                item = RelateComponents[i];
                if (item == name)
                {
                    if (!mRelatedComponents.ContainsKey(name))
                    {
                        mRelatedComponents[name] = target;
                    }
                    break;
                }
            }

            bool needCheckReFill = (max > 0) && (mRelatedComponents.Size != max);
            if (!needCheckReFill)
            {
                manager.RelateComponentsReFiller -= ReFillRelateComponents;
            }
        }

        public T GetRelatedComponent<T>(int aid) where T : IShipDockComponent
        {
            return (T)mRelatedComponents[aid];
        }

        /// <summary>
        /// 组件中的实体数量发生变化的回调
        /// </summary>
        public virtual void ComponentEntitasStretch(IShipDockEntitas entitas, bool isRemove)
        {
            if (isRemove)
            {
                entitas.RemoveComponent(this);
            }
            else
            {
                entitas.AddComponent(this);
            }
        }

        public int[] RelateComponents { get; set; }
        protected IShipDockComponentManager Context { get; private set; }

    }
}