﻿using ShipDock.Interfaces;
using ShipDock.Tools;
using System;
using System.Collections.Generic;

namespace ShipDock.ECS
{
    public static class ShipDockComponentManagerSetting
    {
        public static bool isMergeUpdateMode = false;
        public static bool isUpdateByCallLate = false;
    }

    public class ShipDockComponentManager : IShipDockComponentManager, IDispose
    {
        private IShipDockComponent mComponent;
        private List<int> mDeletdComponents;
        private KeyValueList<int, int> mIDMapper;
        private IntegerMapper<IShipDockComponent> mMapper;
        private List<IShipDockComponent> mUpdateByTicks;
        private List<IShipDockComponent> mUpdateByScene;

        private int mFinalUpdateTime;
        private IShipDockEntitas mFinalUpdateEntitas;
        private Action<int, IShipDockEntitas> mFinalUpdateMethod;

        private DoubleBuffers<int> mQueueUpdateTime;
        private DoubleBuffers<IShipDockEntitas> mQueueUpdateEntitas;
        private DoubleBuffers<Action<int, IShipDockEntitas>> mQueueUpdateExecute;

        public ShipDockComponentManager()
        {
            mComponent = default;
            mIDMapper = new KeyValueList<int, int>();
            mMapper = new IntegerMapper<IShipDockComponent>();

            mDeletdComponents = new List<int>();
            mUpdateByTicks = new List<IShipDockComponent>();
            mUpdateByScene = new List<IShipDockComponent>();

            mQueueUpdateTime = new DoubleBuffers<int>();
            mQueueUpdateEntitas = new DoubleBuffers<IShipDockEntitas>();
            mQueueUpdateExecute = new DoubleBuffers<Action<int, IShipDockEntitas>>();
            
            mQueueUpdateExecute.OnDequeue += OnQueueUpdateExecute;
        }

        public void Dispose()
        {
            Utils.Reclaim(ref mUpdateByTicks);
            Utils.Reclaim(ref mUpdateByScene);
            Utils.Reclaim(ref mDeletdComponents);
            Utils.Reclaim(mMapper);
            Utils.Reclaim(mIDMapper);
            mComponent = default;
        }

        public int Create<T>(int name, bool isUpdateByScene = false, params int[] willRelateComponents) where T : IShipDockComponent, new()
        {
            T target = new T
            {
                RelateComponents = willRelateComponents
            };
            target.SetSceneUpdate(isUpdateByScene);
            target.OnFinalUpdateForTime = OnFinalUpdateForTime;
            target.OnFinalUpdateForEntitas = OnFinalUpdateForEntitas;
            target.OnFinalUpdateForExecute = OnFinalUpdateForExecute;

            int aid = mMapper.Add(target, out int statu);
            if (isUpdateByScene)
            {
                mUpdateByScene.Add(target);
            }
            else
            {
                mUpdateByTicks.Add(target);
            }

            if (statu == 0)
            {
                mIDMapper[name] = aid;

                target.SetComponentID(aid);
                target.FillRelateComponents(this);
                target.Init();
                RelateComponentsReFiller?.Invoke(name, target, this);
            }
            else
            {
                aid = -1;
            }
            return aid;
        }

        private void OnFinalUpdateForExecute(Action<int, IShipDockEntitas> method)
        {
            mQueueUpdateExecute.Enqueue(method, false);
        }

        private void OnFinalUpdateForEntitas(IShipDockEntitas entitas)
        {
            mQueueUpdateEntitas.Enqueue(entitas, false);
        }

        private void OnFinalUpdateForTime(int time)
        {
            mQueueUpdateTime.Enqueue(time, false);
        }

        public T GetEntitasWithComponents<T>(params int[] aidArgs) where T : IShipDockEntitas, new()
        {
            T result = new T();
            int max = aidArgs.Length;
            int aid;
            IShipDockComponent component;
            for (int i = 0; i < max; i++)
            {
                aid = aidArgs[i];
                component = RefComponentByName(aid);
                if (component != default)
                {
                    result.AddComponent(component);
                }
            }
            return result;
        }

        public IShipDockComponent RefComponentByName(int name)
        {
            IShipDockComponent component = default;
            if (mIDMapper.IsContainsKey(name))
            {
                int id = mIDMapper[name];
                component = mMapper.Get(id);
            }
            return component;
        }

        public void RemoveComponent(IShipDockComponent target)
        {
            if (!mDeletdComponents.Contains(target.ID))
            {
                mDeletdComponents.Add(target.ID);
            }
        }

        public void RemoveSingedComponents()
        {
            int max = mDeletdComponents.Count;
            for (int i = 0; i < max; i++)
            {
                int id = mDeletdComponents[i];
                IShipDockComponent target = mMapper.Get(id);
                
                int index = mIDMapper.Values.IndexOf(id);

                id = mIDMapper.Keys[index];
                mIDMapper.Remove(id);

                if (index >= 0)
                {
                    mIDMapper.Remove(id);
                }
                List<IShipDockComponent> updateList = target.IsSceneUpdate ? mUpdateByScene : mUpdateByTicks;
                updateList.Remove(target);
                mMapper.Remove(target, out int statu);

                target.Dispose();

            }
            if(max > 0)
            {
                mDeletdComponents.Clear();
            }
        }

        public void UpdateAndFreeComponents(int time, Action<Action<int>> method = default)
        {
            int max = mUpdateByTicks.Count;
            for (int i = 0; i < max; i++)
            {
                mComponent = mUpdateByTicks[i];

                if (!mDeletdComponents.Contains(mComponent.ID))
                {
                    if (method == default)
                    {
                        mComponent.UpdateComponent(time);
                        mComponent.FreeComponent(time);
                    }
                    else
                    {
                        method.Invoke(mComponent.UpdateComponent);
                        method.Invoke(mComponent.FreeComponent);
                    }
                }
            }

            FinalUpdate(time);

            RemoveSingedComponents();
        }

        public void UpdateComponentUnit(int time, Action<Action<int>> method = default)
        {
            CountTime += time;

            while (CountTime > FrameTimeInScene)
            {
                int max = mUpdateByTicks.Count;
                for (int i = 0; i < max; i++)
                {
                    mComponent = mUpdateByTicks[i];

                    if (!mDeletdComponents.Contains(mComponent.ID))
                    {
                        if (method == default)
                        {
                            mComponent.UpdateComponent(time);
                        }
                        else
                        {
                            method.Invoke(mComponent.UpdateComponent);
                        }
                    }
                }

                FinalUpdate(time);

                CountTime -= FrameTimeInScene;
            }
        }

        private void OnQueueUpdateExecute(int time, Action<int, IShipDockEntitas> current)
        {
            mFinalUpdateTime = mQueueUpdateTime.Current;
            mFinalUpdateEntitas = mQueueUpdateEntitas.Current;
            mFinalUpdateMethod = current;
            
            if (mFinalUpdateEntitas == default)
            {
                mFinalUpdateMethod.Invoke(mFinalUpdateTime, default);
            }
            else
            {
                if (!mFinalUpdateEntitas.WillDestroy && (mFinalUpdateEntitas.ID != int.MaxValue))
                {
                    mFinalUpdateMethod.Invoke(mFinalUpdateTime, mFinalUpdateEntitas);
                }
            }
        }

        private void FinalUpdate(int time)
        {
            mQueueUpdateTime.Step(time);
            mQueueUpdateEntitas.Step(time);
            mQueueUpdateExecute.Step(time);
            mFinalUpdateEntitas = default;
            mFinalUpdateMethod = default;
        }

        public void FreeComponentUnit(int time, Action<Action<int>> method = default)
        {
            int max = mUpdateByTicks.Count;
            for (int i = 0; i < max; i++)
            {
                mComponent = mUpdateByTicks[i];
                
                if (!mDeletdComponents.Contains(mComponent.ID))
                {
                    if (method == default)
                    {
                        mComponent.FreeComponent(time);
                    }
                    else
                    {
                        method.Invoke(mComponent.FreeComponent);
                    }
                }
            }
        }

        public void UpdateAndFreeComponentsInScene(int time, Action<Action<int>> method = default)
        {
            int max = mUpdateByScene.Count;
            for (int i = 0; i < max; i++)
            {
                mComponent = mUpdateByScene[i];

                if (!mDeletdComponents.Contains(mComponent.ID))
                {
                    if (method == default)
                    {
                        mComponent.UpdateComponent(time);
                        mComponent.FreeComponent(time);
                    }
                    else
                    {
                        method.Invoke(mComponent.UpdateComponent);
                        method.Invoke(mComponent.FreeComponent);
                    }
                }
            }
            RemoveSingedComponents();
        }

        public void UpdateComponentUnitInScene(int time, Action<Action<int>> method = default)
        {
            int max = mUpdateByScene.Count;
            for (int i = 0; i < max; i++)
            {
                mComponent = mUpdateByScene[i];

                if (!mDeletdComponents.Contains(mComponent.ID))
                {
                    if (method == default)
                    {
                        mComponent.UpdateComponent(time);
                    }
                    else
                    {
                        method.Invoke(mComponent.UpdateComponent);
                    }
                }
            }
        }

        public void FreeComponentUnitInScene(int time, Action<Action<int>> method = default)
        {
            int max = mUpdateByScene.Count;
            for (int i = 0; i < max; i++)
            {
                mComponent = mUpdateByScene[i];

                if (!mDeletdComponents.Contains(mComponent.ID))
                {
                    if (method == default)
                    {
                        mComponent.FreeComponent(time);
                    }
                    else
                    {
                        method.Invoke(mComponent.FreeComponent);
                    }
                }
            }
        }

        public bool Asynced { get; private set; }
        public Action<IShipDockComponent> CustomUpdate { get; set; }
        public Action<int, IShipDockComponent, IShipDockComponentManager> RelateComponentsReFiller { get; set; }
        public int CountTime { get; private set; }
        public int FrameTimeInScene { get; set; }
    }
}