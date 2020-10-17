using ShipDock.ECS;
using ShipDock.Notices;
using ShipDock.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{
    public abstract class WorldSceneSystem : SystemComponent
    {
        protected WorldInteracter mWorldItem;
        protected Queue<INotice> mWorldEventNotices;
        protected Queue<WorldInteracter> mEventItems;
        protected KeyValueList<int, WorldInteracter> mWorldItems;
        protected KeyValueList<int, ClusteringData> mWorldGroups;

        private INotice mItemNotice;
        private WorldInteracter mEventItem;
        private WorldMovement mMovment;
        private ClusteringData mClusteringData;

        public override void Init(IShipDockComponentContext manager)
        {
            base.Init(manager);

            mWorldEventNotices = new Queue<INotice>();
            mEventItems = new Queue<WorldInteracter>();
            mWorldItems = new KeyValueList<int, WorldInteracter>();
            mWorldGroups = new KeyValueList<int, ClusteringData>();

            WorldComp = GetRelatedComponent<WorldComponent>(WorldComponentName);
        }

        public override int DropEntitas(IShipDockEntitas target, int entitasID)
        {
            mWorldItem = WorldComp.GetEntitasData(ref target);

            if (mWorldItem != default)
            {
                if (mWorldItem.worldItemID == int.MaxValue)
                {
                    if (mWorldItems.ContainsKey(mWorldItem.worldItemID))
                    {
                        mWorldItem.WorldItemDispose?.Invoke();
                        mWorldItem.WorldItemDispose = default;
                        mWorldItems.Remove(mWorldItem.worldItemID);
                        mWorldItem.isDroped = true;
                    }
                }
            }

            return base.DropEntitas(target, entitasID);
        }

        private bool ShouldAddToWorldItems()
        {
            return (mWorldItem != default) &&
                !mWorldItem.isDroped &&
                !mWorldItems.ContainsKey(mWorldItem.worldItemID);
        }

        public override void Execute(int time, ref IShipDockEntitas target)
        {
            base.Execute(time, ref target);

            CheckWorldItemCaches(ref target);

            if (WorldComp.ShouldWorldGroupable)
            {
                CheckClustering(ref target);
            }

            CheckWorldEvents();
        }

        private void CheckWorldItemCaches(ref IShipDockEntitas target)
        {
            if (WorldComp.IsDataValid(ref target))
            {
                mWorldItem = WorldComp.GetEntitasData(ref target);
                if (ShouldAddToWorldItems())
                {
                    mWorldItems.Put(mWorldItem.worldItemID, mWorldItem);
                    AfterWorldItemCached(ref target);
                }
            }
        }

        private void CheckClustering(ref IShipDockEntitas target)
        {
            mClusteringData = WorldComp.GetWorldGroupData(ref target);
            if (mClusteringData != default)
            {
                int id = target.ID;
                if (id != int.MaxValue)
                {
                    if (mClusteringData.IsGroupCached)
                    {
                        mMovment = GetMovmentData(ref target);
                        mClusteringData.ClusteringMag = mMovment.ClusteringDirection.magnitude;
                    }
                    else
                    {
                        if (!mWorldGroups.ContainsKey(id))
                        {
                            mClusteringData.IsGroupCached = true;
                            mWorldGroups[id] = mClusteringData;
                        }
                    }
                }
            }
        }

        private void CheckWorldEvents()
        {
            if (mWorldEventNotices.Count > 0)
            {
                mItemNotice = mWorldEventNotices.Dequeue();
                if (mEventItems.Count > 0)
                {
                    mEventItem = mEventItems.Dequeue();
                    if (!mEventItem.isDroped && (mItemNotice != default))
                    {
                        mEventItem.Dispatch(mItemNotice);//派发子弹命中消息
                        mItemNotice.ToPool();
                    }
                }
            }
        }

        protected virtual void AfterWorldItemCached(ref IShipDockEntitas target)
        {
        }

        protected abstract WorldMovement GetMovmentData(ref IShipDockEntitas target);
        protected abstract Vector3 GetClusteringStandarPosition();

        private WorldComponent WorldComp { get; set; }

        protected abstract int WorldComponentName { get; }
    }
}