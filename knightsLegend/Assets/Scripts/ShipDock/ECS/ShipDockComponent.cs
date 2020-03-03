using ShipDock.Tools;
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

        public virtual void Dispose()
        {
            CleanAllEntitas(ref mEntitasIDs);
            CleanAllEntitas(ref mEntitasIDsRelease);

            mEntitasItem = default;
            Utils.Reclaim(ref mEntitasIDs);
            Utils.Reclaim(ref mEntitasIDsRelease);
            Utils.Reclaim(mEntitas);
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

        public void UpdateComponent(int time)
        {
            Asynced = false;
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
            Asynced = true;
        }

        public void FreeComponent(int time)
        {
            int id;
            int statu;
            int max = (mEntitasIDsRelease != default) ? mEntitasIDsRelease.Count : 0;
            for (int i = 0; i < max; i++)
            {
                id = mEntitasIDsRelease[i];
                mEntitasItem = GetEntitas(id);
                if (mEntitasItem != default)
                {
                    FreeEntitas(id, ref mEntitasItem, out statu);
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

        public bool Asynced { get; private set; }
        public int ID { get; private set; } = int.MaxValue;
    }
}