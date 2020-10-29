#define G_LOG

using ShipDock.ECS;
using ShipDock.Notices;
using ShipDock.Tools;
using System;
using System.Collections.Generic;

namespace ShipDock.Applications
{
    public struct AroundsCheckedInfo
    {
        public int checkingAroundID;
        public float distanceBetween;
        public float distanceCrowding;
        public BehaviourIDs ids;
    }

    public abstract class WorldSceneSystem : SystemComponent
    {
        protected WorldMovement mMovement;
        protected WorldInteracter mWorldItem;
        protected Queue<INotice> mWorldEventNotices;
        protected Queue<WorldInteracter> mEventItems;

        private bool mHasBehaviourIDs;
        private bool mBehaviourIdsCompChecked;
        private INotice mItemNotice;
        private WorldInteracter mEventItem;
        private ClusteringData mClusteringData;
        protected KeyValueList<int, WorldInteracter> mWorldItemMapper;
        private KeyValueList<int, ClusteringData> mGroupsMapper;
        private KeyValueList<int, WorldMovement> mAroundMapper;

        protected ClusteringComponent ClusteringComp { get; private set; }
        protected BehaviourIDsComponent BehaviourIDsComp { get; private set; }
        protected WorldComponent WorldComp { get; set; }
        protected abstract int WorldComponentName { get; }

        public bool ShouldWorldGroupable { get; private set; }

        public override void Init(IShipDockComponentContext context)
        {
            base.Init(context);

            mWorldEventNotices = new Queue<INotice>();
            mEventItems = new Queue<WorldInteracter>();

            mWorldItemMapper = new KeyValueList<int, WorldInteracter>();
            mGroupsMapper = new KeyValueList<int, ClusteringData>();
            mAroundMapper = new KeyValueList<int, WorldMovement>();

            WorldComp = GetRelatedComponent<WorldComponent>(WorldComponentName);
            BehaviourIDsComp = context.RefComponentByName(WorldComp.BehaviaourIDsComponentName) as BehaviourIDsComponent;
            ClusteringComp = context.RefComponentByName(WorldComp.WorldGroupComponentName) as ClusteringComponent;

            ShouldWorldGroupable = ClusteringComp != default;
        }

        public override int DropEntitas(IShipDockEntitas target, int entitasID)
        {
            mWorldItem = WorldComp.GetEntitasData(ref target);

            if (mWorldItem != default)
            {
                if (mWorldItem.worldItemID == int.MaxValue)
                {
                    if (mWorldItemMapper.ContainsKey(mWorldItem.worldItemID))
                    {
                        DropWorldItem(ref target);
                    }
                }
            }

            return base.DropEntitas(target, entitasID);
        }

        protected virtual void DropWorldItem(ref IShipDockEntitas target)
        {
            int worldItemID = mWorldItem.worldItemID;
            mWorldItemMapper.Remove(worldItemID);

            mGroupsMapper.Remove(mWorldItem.groupID);
            mAroundMapper.Remove(mWorldItem.aroundID);

            List<int> list = BehaviourIDsComp.GetAroundList(ref target);
            list?.Clear();

            BehaviourIDs ids = BehaviourIDsComp.GetEntitasData(ref target);
            ids.willClear = true;
            BehaviourIDsComp.FillEntitasData(ref target, ids);

            mWorldItem.WorldItemDispose?.Invoke();
            mWorldItem.WorldItemDispose = default;
            mWorldItem.isDroped = true;
        }

        private bool ShouldAddToWorldItems()
        {
            if (mWorldItem == default)
            {
                return false;
            }
            int id = mWorldItem.worldItemID;
            return IsWorldItemValid(ref mWorldItem) && !mWorldItemMapper.ContainsKey(id);
        }

        private bool IsWorldItemValid(ref WorldInteracter item)
        {
            return (item != default) && !item.isDroped;
        }

        public override void Execute(int time, ref IShipDockEntitas target)
        {
            base.Execute(time, ref target);

            mMovement = GetMovmentData(ref target);

            CheckWorldItemCaches(ref target);
            if (ShouldWorldGroupable)
            {
                SetClusteringPosition(ref target);
                CheckClustering(ref target);
            }
            CheckAround(ref target);
            CheckWorldEvents();
        }

        /// <summary>
        /// 检测物体四周目标
        /// </summary>
        private void CheckAround(ref IShipDockEntitas target)
        {
            if (!mBehaviourIdsCompChecked)
            {
                mBehaviourIdsCompChecked = true;
                mHasBehaviourIDs = BehaviourIDsComp != default;
            }

            if (mHasBehaviourIDs && IsWorldItemValid(ref mWorldItem))
            {
                BehaviourIDs ids = BehaviourIDsComp.GetEntitasData(ref target);
                int aroundID = ids.gameItemID;
                if (mAroundMapper.ContainsKey(aroundID))
                {
                    WalkEachAroundItem(ref target, ids);
                }
                else
                {
                    mWorldItem.aroundID = aroundID;
                    mAroundMapper[aroundID] = mMovement;
                }
            }
        }

        private void WalkEachAroundItem(ref IShipDockEntitas target, BehaviourIDs ids)
        {
            bool flag;
            int id, aroundID = ids.gameItemID;
            float distance;
            AroundsCheckedInfo info;
            WorldMovement itemMovement;
            List<int> list = BehaviourIDsComp.GetAroundList(ref target);
            int max = (list != default) ? list.Count : 0;
            if (max > 0)
            {
                for (int i = 0; i < max; i++)
                {
                    id = list[i];
                    itemMovement = GetAroundTargetMovement(id);
                    if (itemMovement != default && !itemMovement.Invalid)
                    {
                        distance = mMovement.DistanceBetween(itemMovement.Position);
                        info = new AroundsCheckedInfo
                        {
                            ids = ids,
                            checkingAroundID = id,
                            distanceBetween = distance,
                            distanceCrowding = GetCrowdingDistance(),
                        };
                        flag = CheckingAround(ref target, aroundID, info, ref itemMovement);
                        if (!flag)
                        {
                            break;
                        }
                    }
                }
                BehaviourIDsComp.PhysicsCheckReset(aroundID);
            }
            AroundsChecked(ref target, aroundID);
        }

        protected virtual float GetCrowdingDistance()
        {
            return 1f;
        }

        protected virtual void AroundsChecked(ref IShipDockEntitas target, int aroundID)
        {
        }

        protected WorldMovement GetAroundTargetMovement(int aroundID)
        {
            return mAroundMapper?.GetValue(aroundID);
        }

        /// <summary>
        /// 检测世界交换物体的缓存
        /// </summary>
        private void CheckWorldItemCaches(ref IShipDockEntitas target)
        {
            if (WorldComp.IsDataValid(ref target))
            {
                mWorldItem = WorldComp.GetEntitasData(ref target);
                if (ShouldAddToWorldItems())
                {
                    mWorldItemMapper.Put(mWorldItem.worldItemID, mWorldItem);
                    AfterWorldItemCached(ref target);
                }
            }
        }

        protected WorldInteracter GetWorldItemFromCache(int worldItemID)
        {
            return mWorldItemMapper[worldItemID];
        }

        /// <summary>
        /// 检测群聚
        /// </summary>
        private void CheckClustering(ref IShipDockEntitas target)
        {
            if ((mMovement != default) && IsWorldItemValid(ref mWorldItem))
            {
                int id = mWorldItem.worldItemID;
                
                mClusteringData = ClusteringComp.GetEntitasData(ref target);
                if ((id != int.MaxValue) && (mClusteringData != default))
                {
                    if (mClusteringData.IsGroupCached)
                    {
                        mClusteringData.ClusteringMag = mMovement.ClusteringDirection.magnitude;
                        "todo".Log("开发群聚功能");
                    }
                    else
                    {
                        if (!mGroupsMapper.ContainsKey(id))
                        {
                            mWorldItem.groupID = id;
                            mClusteringData.IsGroupCached = true;
                            mGroupsMapper[id] = mClusteringData;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 检测世界交换物体的事件
        /// </summary>
        private void CheckWorldEvents()
        {
            if (mWorldEventNotices.Count > 0)
            {
                mItemNotice = mWorldEventNotices.Dequeue();
                if (mEventItems.Count > 0)
                {
                    mEventItem = mEventItems.Dequeue();
                    if (IsEventItemValid())
                    {
                        mEventItem.Dispatch(mItemNotice);//派发世界物体消息
                        mItemNotice.ToPool();
                    }
                }
            }
        }

        private bool IsEventItemValid()
        {
            return IsWorldItemValid(ref mEventItem) && 
                !mEventItem.isDroped && 
                (mItemNotice != default);
        }

        protected abstract WorldMovement GetMovmentData(ref IShipDockEntitas target);
        protected abstract void AfterWorldItemCached(ref IShipDockEntitas target);
        protected abstract void SetClusteringPosition(ref IShipDockEntitas target);
        protected abstract bool CheckingAround(ref IShipDockEntitas target, int aroundID, AroundsCheckedInfo aroundInfo, ref WorldMovement movement);

    }
}