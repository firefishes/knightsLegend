using ShipDock.Interfaces;
using ShipDock.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.ECS
{
    public class ShipDockComponentManager : IShipDockComponentManager, IDispose
    {
        private KeyValueList<int, int> mIDMapper;
        private IShipDockComponent mComponent = default;
        private IntegerMapper<IShipDockComponent> mMapper;

        public ShipDockComponentManager()
        {
            mIDMapper = new KeyValueList<int, int>();
            mMapper = new IntegerMapper<IShipDockComponent>();
        }

        public void Dispose()
        {
            Utils.Reclaim(mMapper);
            mComponent = default;
        }

        public int Create<T>(int aid) where T : IShipDockComponent, new()
        {
            T target = new T();
            int id = mMapper.Add(target, out int statu);
            if (statu == 0)
            {
                target.SetComponentID(id);
                target.Init();
                mIDMapper[aid] = id;
            }
            else
            {
                id = -1;
            }
            return id;
        }

        public T GetEntitasWithComponents<T>(params int[] aidArgs) where T : IShipDockEntitas, new()
        {
            T result = new T();
            int max = aidArgs.Length;
            int id, aid;
            IShipDockComponent component;
            for (int i = 0; i < max; i++)
            {
                aid = aidArgs[i];
                component = GetComponentByAID(aid);
                if (component != default)
                {
                    result.AddComponent(component);
                }
            }
            return result;
        }

        public IShipDockComponent GetComponentByAID(int aid)
        {
            IShipDockComponent component = default;
            if (mIDMapper.IsContainsKey(aid))
            {
                int id = mIDMapper[aid];
                component = mMapper.Get(id);
            }
            return component;
        }

        public void RemoveComponent(IShipDockComponent target)
        {
            int id;
            int index = -1;
            int max = mMapper.Size;
            IShipDockComponent item = default;
            for (int i = 0; i < max; i++)
            {
                id = mMapper.GetIDByIndex(i);
                item = mMapper.Get(id);
                if (target.ID == item.ID)
                {
                    index = mIDMapper.Values.IndexOf(id);
                    mIDMapper.Remove(mIDMapper.Keys[index]);
                    break;
                }
            }
            if (item != default)
            {
                if (index >= 0)
                {
                    mIDMapper.Remove(mIDMapper.Keys[index]);
                }
                mMapper.Remove(target, out int statu);
                target.Dispose();
            }
        }

        public void UpdateAndFreeComponents(int time)
        {
            int id;
            int max = mMapper.Size;
            for (int i = 0; i < max; i++)
            {
                id = mMapper.GetIDByIndex(i);
                mComponent = mMapper.Get(id);
                mComponent.UpdateComponent(time);
                mComponent.FreeComponent(time);
            }
        }

        public void UpdateComponentUnit(Action<Action<int>> method)
        {
            int id;
            int max = mMapper.Size;
            for (int i = 0; i < max; i++)
            {
                id = mMapper.GetIDByIndex(i);
                mComponent = mMapper.Get(id);
                method.Invoke(mComponent.UpdateComponent);
            }
        }

        public void FreeComponentUnit(Action<Action<int>> method)
        {
            int id;
            int max = mMapper.Size;
            for (int i = 0; i < max; i++)
            {
                id = mMapper.GetIDByIndex(i);
                mComponent = mMapper.Get(id);
                method.Invoke(mComponent.FreeComponent);
            }
        }

        public Action<IShipDockComponent> CustomUpdate { get; set; }
        public bool Asynced { get; private set; }
    }
}