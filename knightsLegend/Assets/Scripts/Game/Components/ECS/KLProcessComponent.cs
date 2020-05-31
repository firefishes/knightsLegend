#define G_LOG

using ShipDock.Applications;
using ShipDock.ECS;
using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Tools;
using System;
using System.Collections.Generic;

namespace KLGame
{
    public class KLProcessComponent : ShipDockComponent, IProcessingComponent
    {
        private ServerRelater mRelater;
        private List<IRoleProcessing> mWillDeleteds;
        private MethodProcessing mProcessingMethod;
        private Queue<MethodProcessing> mProcessingQueue;
        private KeyValueList<int, IKLRole> mRoleProcessingMapper;
        private KeyValueList<int, List<IRoleProcessing>> mHitsMapper;

        public override void Init()
        {
            base.Init();

            mProcessingQueue = new Queue<MethodProcessing>();
            mWillDeleteds = new List<IRoleProcessing>();
            mHitsMapper = new KeyValueList<int, List<IRoleProcessing>>();
            mRoleProcessingMapper = new KeyValueList<int, IKLRole>();

            RoleCollisionComp = GetRelatedComponent<RoleColliderComponent>(KLConsts.C_ROLE_COLLIDER);
            RoleMustComp = GetRelatedComponent<RoleMustComponent>(KLConsts.C_ROLE_MUST);

            mRelater = new ServerRelater
            {
                DataNames = new int[]
                {
                    KLConsts.D_BATTLE
                }
            };
            mRelater.CommitRelate();
        }

        public override void Dispose()
        {
            base.Dispose();

            Utils.Reclaim(ref mProcessingQueue);

            mProcessingMethod = default;
        }

        public bool AddProcess(Action<IProcessing> method)
        {
            bool result = default;
            if (mProcessingQueue != default)
            {
                result = true;
                MethodProcessing item = Pooling<MethodProcessing>.From();
                item.Reinit(method);
                mProcessingQueue.Enqueue(item);
            }
            return result;
        }

        private bool CheckAndInitRoleProcessingHandler(ref IRoleProcessing item)
        {
            var initiator = item.Initiator;
            if (initiator.WillDestroy)
            {
                if (mRoleProcessingMapper.ContainsKey(initiator.ID))
                {
                    ShipDockApp.Instance.Notificater.Remove(initiator, OnProcessingHandler);
                    mRoleProcessingMapper.Remove(initiator.ID);
                }
                return false;
            }

            if (!mRoleProcessingMapper.ContainsKey(initiator.ID))
            {
                ShipDockApp.Instance.Notificater.Add(initiator, OnProcessingHandler);//然后添加流程事件处理
                mRoleProcessingMapper[initiator.ID] = initiator;
            }
            return true;
        }

        private void CheckAndAddHit(ref IRoleProcessing item, out bool flag)
        {
            ProcessHit hit = item as ProcessHit;
            IKLRole enmeyRole = hit.EnemyKLRole;
            flag = enmeyRole != default;
            if (flag)
            {
                int colliderID = enmeyRole.RoleMustSubgroup.roleColliderID;
                if (colliderID != hit.HitColliderID)
                {
                    UnityEngine.Debug.Log("hit process droped, colliderID = " + colliderID + " hit colliderID = " + hit.HitColliderID);
                    hit.ToPool();
                    flag = false;
                    return;
                }
                if (!mHitsMapper.ContainsKey(colliderID))
                {
                    mHitsMapper[colliderID] = new List<IRoleProcessing>();
                }
                var hitList = mHitsMapper[colliderID];
                hitList.Add(item);//加入攻击结算的队列
            }
            else
            {
                item.ToPool();
            }
        }

        public bool AddRoleProcess(IRoleProcessing item)
        {
            bool flag = default;
            if(!CheckAndInitRoleProcessingHandler(ref item))
            {
                return flag;
            }

            item.ProcessingReady();

            switch(item.Type)
            {
                case ProcessingType.HIT:
                    CheckAndAddHit(ref item, out flag);
                    break;
            }
            return flag;
        }

        private void OnProcessingHandler(INoticeBase<int> param)
        {
            ProcessingNotice notice = param as ProcessingNotice;

            if (notice == default)
            {
                return;
            }
            switch(notice.ProcessingType)
            {
                case ProcessingType.HIT:
                    ProcessHit(ref notice);
                    break;
            }
        }

        private void ProcessHit(ref ProcessingNotice notice)
        {
            int entitasID = notice.HitInfo.entitasID;
            if (mRoleProcessingMapper.ContainsKey(entitasID))
            {
                IKLRole role = mRoleProcessingMapper[entitasID];
                if (role.WillDestroy)
                {
                    return;
                }

                int colliderID = notice.HitInfo.hitColliderID;
                var hitList = mHitsMapper[colliderID];

                if (hitList != default)
                {
                    IRoleProcessing item;
                    int max = hitList.Count;
                    for (int i = 0; i < max; i++)
                    {
                        item = hitList[i];
                        if (!item.Finished && (item.Initiator == role))
                        {
                            item.Finished = true;
                            item.OnProcessing();
                            mWillDeleteds.Add(item);
                        }
                    }
                    max = mWillDeleteds.Count;
                    if (max > 0)
                    {
                        foreach (var d in mWillDeleteds)
                        {
                            hitList.Remove(d);
                        }
                        hitList.Clear();
                    }
                }
            }
        }

        public override void Execute(int time, ref IShipDockEntitas target)
        {
            base.Execute(time, ref target);

            if (mProcessingQueue.Count > 0)
            {
                mProcessingMethod = mProcessingQueue.Peek();
                mProcessingMethod.OnProcessing();
                if(mProcessingMethod.Finished)
                {
                    mProcessingQueue.Dequeue();
                    mProcessingMethod.AfterProcessing?.Invoke();
                    mProcessingMethod.ToPool();
                    mProcessingMethod = default;
                }
            }
        }

        public KLBattleData BattleData
        {
            get
            {
                return mRelater.DataRef<KLBattleData>(KLConsts.D_BATTLE);
            }
        }

        public RoleColliderComponent RoleCollisionComp { get; private set; }
        public RoleMustComponent RoleMustComp { get; private set; }
    }

}