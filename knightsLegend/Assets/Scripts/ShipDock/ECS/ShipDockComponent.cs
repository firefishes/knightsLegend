using ShipDock.Tools;
using System;
using System.Collections.Generic;

namespace ShipDock.ECS
{
    public class ShipDockComponent : IShipDockComponent
    {
        private IShipDockEntitas mEntitasItem;
        private List<int> mEntitasIDs;
        private List<int> mEntitasIDsRelease;
        private List<int> mEntitasIDsRemoved;
        private IntegerMapper<IShipDockEntitas> mEntitas;
        private KeyValueList<int, IShipDockComponent> mRelatedComponents;

        public ShipDockComponent()
        {
            mRelatedComponents = new KeyValueList<int, IShipDockComponent>();
        }

        public virtual void Dispose()
        {
            CleanAllEntitas(ref mEntitasIDs);
            CleanAllEntitas(ref mEntitasIDsRelease);

            OnFinalUpdateForTime = default;
            OnFinalUpdateForEntitas = default;
            OnFinalUpdateForExecute = default;

            mEntitasItem = default;
            Utils.Reclaim(ref mEntitasIDs);
            Utils.Reclaim(ref mEntitasIDsRelease);
            Utils.Reclaim(mEntitas);
            ID = int.MaxValue;
        }

        private void CleanAllEntitas(ref List<int> list)
        {
            int id;
            int max = list.Count;
            for (int i = 0; i < max; i++)
            {
                id = list[i];
                mEntitasItem = GetEntitas(id);
                mEntitasItem.RemoveComponent(this);
            }
        }

        public virtual void Init()
        {
            mEntitasIDs = new List<int>();
            mEntitasIDsRemoved = new List<int>();
            mEntitasIDsRelease = new List<int>();
            mEntitas = new IntegerMapper<IShipDockEntitas>();
        }

        public virtual int SetEntitas(IShipDockEntitas target)
        {
            int id = mEntitas.Add(target, out int statu);
            if (statu == 0)
            {
                mEntitasIDs.Add(id);
            }
            return id;
        }

        public IShipDockEntitas GetEntitas(int id)
        {
            IShipDockEntitas result = mEntitas.Get(id, out _);
            return result;
        }

        public void GetEntitasRef(int id, out IShipDockEntitas entitas)
        {
            entitas = mEntitas.Get(id, out _);
        }

        public virtual int DropEntitas(IShipDockEntitas target, int entitasID)
        {
            if (mEntitasIDsRemoved.Contains(entitasID))
            {
                return 1;
            }
            else
            {
                mEntitasIDsRemoved.Add(entitasID);
            }
            return 0;
        }

        public void SetComponentID(int id)
        {
            if (ID == int.MaxValue)
            {
                ID = id;
            }
        }

        public virtual void Execute(int time, ref IShipDockEntitas target)
        {
        }

        protected void ExecuteInFinal(int time, IShipDockEntitas entitas, Action<int, IShipDockEntitas> method)
        {
            if (ID == int.MaxValue)
            {
                return;
            }
            OnFinalUpdateForTime.Invoke(time);
            OnFinalUpdateForEntitas.Invoke(entitas);
            OnFinalUpdateForExecute(method);
        }

        public void UpdateComponent(int time)
        {
            int id;
            int max = (mEntitasIDs != default) ? mEntitasIDs.Count : 0;
            for (int i = 0; i < max; i++)
            {
                id = mEntitasIDs[i];
                mEntitasItem = GetEntitas(id);
                if (mEntitasItem != default)
                {
                    if (mEntitasItem.WillDestroy || mEntitasIDsRemoved.Contains(id))
                    {
                        if (!mEntitasIDsRelease.Contains(id))
                        {
                            mEntitasIDsRelease.Add(id);
                        }
                    }
                    else
                    {
                        Execute(time, ref mEntitasItem);
                    }
                }
                else
                {
                    if (!mEntitasIDsRemoved.Contains(id))
                    {
                        mEntitasIDsRelease.Add(id);
                    }
                }
            }
            mEntitasItem = default;
        }

        public void FreeComponent(int time)
        {
            int id;
            int max = (mEntitasIDsRelease != default) ? mEntitasIDsRelease.Count : 0;
            for (int i = 0; i < max; i++)
            {
                id = mEntitasIDsRelease[i];
                mEntitasItem = GetEntitas(id);
                if (mEntitasItem != default)
                {
                    FreeEntitas(id, ref mEntitasItem, out int statu);
                }
                mEntitasIDsRemoved.Remove(id);
            }
            mEntitasIDsRelease.Clear();
            mEntitasItem = default;
        }

        protected virtual void FreeEntitas(int mid, ref IShipDockEntitas entitas, out int statu)
        {
            entitas.RemoveComponent(this);//此处在实体无需销毁时可能为重复操作
            mEntitas.Remove(entitas, out statu);
            mEntitasIDs.Remove(mid);
        }

        public void FillRelateComponents(IShipDockComponentManager manager)
        {
            int name;
            IShipDockComponent item;
            int max = RelateComponents != default ? RelateComponents.Length : 0;
            for (int i = 0; i < max; i++)
            {
                name = RelateComponents[i];
                if(!mRelatedComponents.ContainsKey(name))
                {
                    item = manager.RefComponentByName(name);
                    if(item != default)
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

        protected T GetRelatedComponent<T>(int aid) where T : IShipDockComponent
        {
            return (T)mRelatedComponents[aid];
        }

        public void SetSceneUpdate(bool value)
        {
            IsSceneUpdate = value;
        }

        //public bool IsUpdating { get; protected set; }
        public int ID { get; private set; } = int.MaxValue;
        public int[] RelateComponents { get; set; }
        public bool IsSceneUpdate { get; private set; }
        public bool IsVariableFrame { get; set; }
        public Action<int> OnFinalUpdateForTime { private get; set; }
        public Action<IShipDockEntitas> OnFinalUpdateForEntitas { set; private get; }
        public Action<Action<int, IShipDockEntitas>> OnFinalUpdateForExecute { set; private get; }
    }
}