#define G_LOG

using ShipDock.Applications;
using ShipDock.ECS;
using ShipDock.Notices;
using ShipDock.Tools;
using System.Collections.Generic;

namespace KLGame
{
    public class KLProcessComponent : ShipDockComponent, IProcessingComponent
    {

        private IGameProcessing mProcessItem;
        private List<IGameProcessing> mWillDeleteds;
        private KeyValueList<int, IKLRole> mRoleProcessingList;
        private KeyValueList<int, List<IGameProcessing>> mHitsMapper;

        public override void Init()
        {
            base.Init();

            mWillDeleteds = new List<IGameProcessing>();
            mHitsMapper = new KeyValueList<int, List<IGameProcessing>>();
            mRoleProcessingList = new KeyValueList<int, IKLRole>();

            RoleCollisionComp = GetRelatedComponent<RoleColliderComponent>(KLConsts.C_ROLE_COLLIDER);
            RoleMustComp = GetRelatedComponent<RoleMustComponent>(KLConsts.C_ROLE_MUST);

        }

        public bool AddProcess(IGameProcessing item)
        {
            var initiator = item.Initiator;
            if (initiator.WillDestroy)
            {
                if(mRoleProcessingList.ContainsKey(initiator.ID))
                {
                    ShipDockApp.Instance.Notificater.Remove(initiator, OnProcessingNotice);
                    mRoleProcessingList.Remove(initiator.ID);
                }
                return false;
            }

            if (!mRoleProcessingList.ContainsKey(initiator.ID))
            {
                ShipDockApp.Instance.Notificater.Add(initiator, OnProcessingNotice);
                mRoleProcessingList[initiator.ID] = initiator;
            }

            item.ProcessingReady();

            bool flag = default;
            switch(item.Type)
            {
                case ProcessingType.HIT:
                    ProcessHit hit = item as ProcessHit;
                    IKLRole enmeyRole = hit.EnemyKLRole;
                    flag = enmeyRole != default;
                    if (flag)
                    {
                        int colliderID = enmeyRole.RoleMustSubgroup.roleColliderID;
                        if (!mHitsMapper.ContainsKey(colliderID))
                        {
                            mHitsMapper[colliderID] = new List<IGameProcessing>();
                        }
                        var hitList = mHitsMapper[colliderID];
                        hitList.Add(item);
                    }
                    else
                    {
                        item.ToPooling();
                    }
                    break;
            }
            return flag;
        }

        private void OnProcessingNotice(INoticeBase<int> param)
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
            if (mRoleProcessingList.ContainsKey(entitasID))
            {
                IKLRole role = mRoleProcessingList[entitasID];
                if (!role.WillDestroy)
                {
                    int colliderID = notice.HitInfo.hitColliderID;
                    var hitList = mHitsMapper[colliderID];

                    if (hitList != default)
                    {
                        IGameProcessing item;
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
                            foreach(var d in mWillDeleteds)
                            {
                                hitList.Remove(d);
                            }
                            hitList.Clear();
                        }
                    }
                }
            }
        }

        public override void Execute(int time, ref IShipDockEntitas target)
        {
            base.Execute(time, ref target);


        }

        public RoleColliderComponent RoleCollisionComp { get; private set; }
        public RoleMustComponent RoleMustComp { get; private set; }
    }

}