using ShipDock.Interfaces;
using ShipDock.Tools;
using System;
using System.Collections.Generic;

namespace ShipDock.ECS
{
    public static class ShipDockECSSetting
    {
        public static bool isMergeUpdateMode = false;
        public static bool isUpdateByCallLate = false;
    }

    /// <summary>
    /// 
    /// ECS组件上下文环境
    /// 
    /// </summary>
    public class ShipDockComponentContext : IShipDockComponentContext, IDispose
    {
        private int mFinalUpdateTime;
        private IShipDockEntitas mFinalUpdateEntitas;
        private IShipDockComponent mSystem;
        private List<IShipDockComponent> mComponents;
        private List<int> mUpdateByTicks;
        private List<int> mUpdateByScene;
        private List<int> mDeletdComponents;
        private KeyValueList<int, int> mNameAutoIDMapper;
        private IntegerMapper<IShipDockComponent> mMapper;
        private Action<int, IShipDockEntitas> mFinalUpdateMethod;
        private DoubleBuffers<int> mQueueUpdateTime;
        private DoubleBuffers<IShipDockEntitas> mQueueUpdateEntitas;
        private DoubleBuffers<Action<int, IShipDockEntitas>> mQueueUpdateExecute;

        public ShipDockComponentContext()
        {
            mSystem = default;
            mNameAutoIDMapper = new KeyValueList<int, int>();
            mMapper = new IntegerMapper<IShipDockComponent>();

            mDeletdComponents = new List<int>();
            mUpdateByTicks = new List<int>();
            mUpdateByScene = new List<int>();
            mComponents = new List<IShipDockComponent>();

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
            Utils.Reclaim(ref mComponents);
            Utils.Reclaim(mQueueUpdateTime);
            Utils.Reclaim(mQueueUpdateEntitas);
            Utils.Reclaim(mQueueUpdateExecute);
            Utils.Reclaim(mMapper);
            Utils.Reclaim(mNameAutoIDMapper);
            RelateComponentsReFiller = default;
            PreUpdate = default;
            mSystem = default;
        }

        public int Create<T>(T target, int name, bool isUpdateByScene = false, params int[] willRelateComponents) where T : IShipDockComponent, new()
        {
            SetComponentUpdateMode(target, isUpdateByScene, willRelateComponents);
            AddComponentToMapper(name, ref target, out int autoID);

            return autoID;
        }

        public int Create<T>(int name, bool isUpdateByScene = false, params int[] willRelateComponents) where T : IShipDockComponent, new()
        {
            T target = new T();

            SetComponentUpdateMode(target, isUpdateByScene, willRelateComponents);
            AddComponentToMapper(name, ref target, out int autoID);
            
            return autoID;
        }

        private void AddComponentToMapper<T>(int name, ref T target, out int autoID) where T : IShipDockComponent, new()
        {
            autoID = mMapper.Add(target, out int statu);
            if (statu == 0)
            {
                mNameAutoIDMapper[name] = autoID;

                target.SetComponentID(autoID);
                target.Init(this);
                RelateComponentsReFiller?.Invoke(name, target, this);
            }
            else
            {
                autoID = -1;
            }
        }

        private void SetComponentUpdateMode<T>(T target, bool isUpdateByScene = false, params int[] willRelateComponents) where T : IShipDockComponent, new()
        {
            bool isSystem = target.IsSystem;
            if (isSystem)
            {
                ISystemComponent system = target as ISystemComponent;
                system.RelateComponents = willRelateComponents;
            }
            target.SetSceneUpdate(isUpdateByScene);
            target.OnFinalUpdateForTime = OnFinalUpdateForTime;
            target.OnFinalUpdateForEntitas = OnFinalUpdateForEntitas;
            target.OnFinalUpdateForExecute = OnFinalUpdateForExecute;

            mComponents.Add(target);
            int index = mComponents.Count - 1;
            if (isSystem)
            {
                if (target.IsSceneUpdate)
                {
                    mUpdateByScene.Add(index);
                }
                else
                {
                    mUpdateByTicks.Add(index);
                }
            }
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

        public T GetEntitasWithComponents<T>(params int[] nameArgs) where T : IShipDockEntitas, new()
        {
            T result = new T();
            int max = nameArgs.Length;
            int name;
            IShipDockComponent component;
            for (int i = 0; i < max; i++)
            {
                name = nameArgs[i];
                component = RefComponentByName(name);
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
            if (mNameAutoIDMapper.IsContainsKey(name))
            {
                int id = mNameAutoIDMapper[name];
                component = mMapper.Get(id);
            }
            return component;
        }

        /// <summary>
        /// 标记需要移除的组件
        /// </summary>
        public void RemoveComponent(IShipDockComponent target)
        {
            if (!mDeletdComponents.Contains(target.ID))
            {
                mDeletdComponents.Add(target.ID);
            }
        }

        /// <summary>
        /// 移除已标记的组件
        /// </summary>
        public void RemoveSingedComponents()
        {
            int max = mDeletdComponents.Count;
            for (int i = 0; i < max; i++)
            {
                int id = mDeletdComponents[i];
                IShipDockComponent target = mMapper.Get(id);

                int index = mNameAutoIDMapper.Values.IndexOf(id);

                id = mNameAutoIDMapper.Keys[index];
                mNameAutoIDMapper.Remove(id);

                if (index >= 0)
                {
                    mNameAutoIDMapper.Remove(id);
                }
                int compIndex = mComponents.IndexOf(target);
                List<int> updateList = target.IsSceneUpdate ? mUpdateByScene : mUpdateByTicks;
                updateList.Remove(compIndex);
                mMapper.Remove(target, out int statu);

                target.Dispose();
            }
            if (max > 0)
            {
                mDeletdComponents.Clear();
            }
        }

        private void RefComponentByIndex(int value, ref int compIndex, ref List<int> updateList, ref IShipDockComponent comp)
        {
            compIndex = updateList[value];
            comp = compIndex < mComponents.Count ? mComponents[compIndex] : default;
        }

        public void RefComponentByIndex(int value, bool isSceneUpdate, ref IShipDockComponent comp)
        {
            int index = 0;
            List<int> list = isSceneUpdate ? mUpdateByScene : mUpdateByTicks;
            RefComponentByIndex(value, ref index, ref list, ref comp);
        }

        /// <summary>
        /// 更新组件的同时检测需要释放的组件
        /// </summary>
        public void UpdateAndFreeComponents(int time, Action<Action<int>> method = default)
        {
            int compIndex = 0;
            int max = mUpdateByTicks.Count;
            for (int i = 0; i < max; i++)
            {
                RefComponentByIndex(i, ref compIndex, ref mUpdateByTicks, ref mSystem);

                if ((mSystem != default) && !mDeletdComponents.Contains(mSystem.ID))
                {
                    if (method == default)
                    {
                        mSystem.UpdateComponent(time);
                        mSystem.FreeComponent(time);
                    }
                    else
                    {
                        method.Invoke(mSystem.UpdateComponent);
                        method.Invoke(mSystem.FreeComponent);
                    }
                }
            }

            FinalUpdate(time);

            RemoveSingedComponents();
        }

        /// <summary>
        /// 更新组件
        /// </summary>
        public void UpdateComponentUnit(int time, Action<Action<int>> method = default)
        {
            CountTime += time;//与主线程的帧率时间保持一致，避更新新过快

            while (CountTime > FrameTimeInScene)
            {
                PreUpdate?.Invoke(mUpdateByTicks, false);

                int compIndex = 0;
                int max = mUpdateByTicks.Count;
                for (int i = 0; i < max; i++)
                {
                    RefComponentByIndex(i, ref compIndex, ref mUpdateByTicks, ref mSystem);
                    if ((mSystem != default) && mSystem.IsSystemChanged && !mSystem.IsSceneUpdate)
                    {
                        if (!mDeletdComponents.Contains(mSystem.ID))
                        {
                            if (method == default)
                            {
                                mSystem.UpdateComponent(time);
                            }
                            else
                            {
                                method.Invoke(mSystem.UpdateComponent);
                            }
                        }
                        mSystem.SystemChecked();
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

        /// <summary>
        /// 检测是否有需要释放的组件
        /// </summary>
        public void FreeComponentUnit(int time, Action<Action<int>> method = default)
        {
            int compIndex = 0;
            int max = mUpdateByTicks.Count;
            for (int i = 0; i < max; i++)
            {
                RefComponentByIndex(i, ref compIndex, ref mUpdateByTicks, ref mSystem);

                if ((mSystem != default) && !mDeletdComponents.Contains(mSystem.ID))
                {
                    if (method == default)
                    {
                        mSystem.FreeComponent(time);
                    }
                    else
                    {
                        method.Invoke(mSystem.FreeComponent);
                    }
                }
            }
        }

        public void UpdateAndFreeComponentsInScene(int time, Action<Action<int>> method = default)
        {
            int compIndex = 0;
            int max = mUpdateByScene.Count;
            for (int i = 0; i < max; i++)
            {
                RefComponentByIndex(i, ref compIndex, ref mUpdateByScene, ref mSystem);

                if ((mSystem != default) && !mDeletdComponents.Contains(mSystem.ID))
                {
                    if (method == default)
                    {
                        mSystem.UpdateComponent(time);
                        mSystem.FreeComponent(time);
                    }
                    else
                    {
                        method.Invoke(mSystem.UpdateComponent);
                        method.Invoke(mSystem.FreeComponent);
                    }
                }
            }
            RemoveSingedComponents();
        }

        public void UpdateComponentUnitInScene(int time, Action<Action<int>> method = default)
        {
            int compIndex = 0;
            int max = mUpdateByScene.Count;
            for (int i = 0; i < max; i++)
            {
                RefComponentByIndex(i, ref compIndex, ref mUpdateByScene, ref mSystem);

                if ((mSystem != default) && !mDeletdComponents.Contains(mSystem.ID))
                {
                    if (method == default)
                    {
                        mSystem.UpdateComponent(time);
                    }
                    else
                    {
                        method.Invoke(mSystem.UpdateComponent);
                    }
                }
            }
        }

        public void FreeComponentUnitInScene(int time, Action<Action<int>> method = default)
        {
            int compIndex = 0;
            int max = mUpdateByScene.Count;
            for (int i = 0; i < max; i++)
            {
                RefComponentByIndex(i, ref compIndex, ref mUpdateByScene, ref mSystem);

                if ((mSystem != default) && !mDeletdComponents.Contains(mSystem.ID))
                {
                    if (method == default)
                    {
                        mSystem.FreeComponent(time);
                    }
                    else
                    {
                        method.Invoke(mSystem.FreeComponent);
                    }
                }
            }
        }

        public Action<int, IShipDockComponent, IShipDockComponentContext> RelateComponentsReFiller { get; set; }
        public Action<List<int>, bool> PreUpdate { get; set; }
        public int CountTime { get; private set; }
        public int FrameTimeInScene { get; set; }
    }
}