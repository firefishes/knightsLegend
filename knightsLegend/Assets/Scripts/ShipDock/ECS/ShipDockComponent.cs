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

        public ShipDockComponent() { }

        public virtual void Dispose()
        {
            CleanAllEntitas(ref mEntitasIDs);
            CleanAllEntitas(ref mEntitasIDsRelease);

            OnFinalUpdateForTime = default;
            OnFinalUpdateForEntitas = default;
            OnFinalUpdateForExecute = default;
            OnEntitasStretch = default;

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

        public virtual void Init(IShipDockComponentManager manager)
        {
            mEntitasIDs = new List<int>();
            mEntitasIDsRemoved = new List<int>();
            mEntitasIDsRelease = new List<int>();
            mEntitas = new IntegerMapper<IShipDockEntitas>();
        }

        public virtual int SetEntitas(IShipDockEntitas target)
        {
            int aid = mEntitas.Add(target, out int statu);
            if (statu == 0)
            {
                mEntitasIDs.Add(aid);
                OnEntitasStretch?.Invoke(target, false);
            }
            return aid;
        }

        public IShipDockEntitas GetEntitas(int aid)
        {
            IShipDockEntitas result = mEntitas.Get(aid, out _);
            return result;
        }

        public void GetEntitasRef(int id, out IShipDockEntitas entitas)
        {
            entitas = mEntitas.Get(id, out _);
        }

        /// <summary>
        /// 删除实体
        /// </summary>
        public virtual int DropEntitas(IShipDockEntitas target, int entitasID)
        {
            if (mEntitasIDsRemoved.Contains(entitasID))
            {
                return 1;
            }
            else
            {
                OnEntitasStretch?.Invoke(target, true);
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
                if (i >= mEntitasIDs.Count)
                {
                    i = 0;
                    max = mEntitasIDs.Count;
                }
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

        /// <summary>
        /// 检测组件中需要释放的实体
        /// </summary>
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

        /// <summary>
        /// 释放实体
        /// </summary>
        protected virtual void FreeEntitas(int mid, ref IShipDockEntitas entitas, out int statu)
        {
            entitas.RemoveComponent(this);//此处在实体无需销毁时可能为重复操作
            mEntitas.Remove(entitas, out statu);
            mEntitasIDs.Remove(mid);
        }

        public void SetSceneUpdate(bool value)
        {
            IsSceneUpdate = value;
        }

        #region TODO 系统特性，需要迁移
        public virtual void SystemChecked()
        {
            IsSystemChanged = true;
        }

        public bool IsSystem { get; protected set; }
        public bool IsSystemChanged { get; set; }
        #endregion
        public int ID { get; private set; } = int.MaxValue;
        public bool IsSceneUpdate { get; private set; }
        public Action<int> OnFinalUpdateForTime { private get; set; }
        public Action<IShipDockEntitas> OnFinalUpdateForEntitas { set; private get; }
        public Action<Action<int, IShipDockEntitas>> OnFinalUpdateForExecute { set; private get; }
        public Action<IShipDockEntitas, bool> OnEntitasStretch { get; set; }
    }
}