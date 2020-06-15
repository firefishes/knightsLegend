using ShipDock.ECS;
using ShipDock.Notices;
using ShipDock.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{
    public abstract class RoleCampComponent : ShipDockComponent
    {
        protected IDataServer mDataServer;
        protected ICommonRole mRoleExecuting;
        protected ICommonRole mRoleTarget;
        private List<int> mAllRoles;
        private KeyValueList<int, List<int>> mCampRoles;

        public override void Init()
        {
            base.Init();

            mAllRoles = new List<int>();
            mCampRoles = new KeyValueList<int, List<int>>();
            mDataServer = DataServerName.GetServer<IDataServer>();
        }

        public override int SetEntitas(IShipDockEntitas target)
        {
            int id = base.SetEntitas(target);
            if (id >= 0)
            {
                RoleCreated = target as ICommonRole;
                int campID = RoleCreated.Camp;
                List<int> list;
                if (mCampRoles.IsContainsKey(campID))
                {
                    list = mCampRoles[campID];
                }
                else
                {
                    list = new List<int>();
                    mCampRoles[campID] = list;
                }
                list.Add(id);
                if (!mAllRoles.Contains(id))
                {
                    mAllRoles.Add(id);
                }
                mDataServer.Delive<IParamNotice<ICommonRole>>(AddCampRoleResovlerName, CampRoleCreatedAlias);
                RoleCreated = default;
            }
            return id;
        }

        protected override void FreeEntitas(int mid, ref IShipDockEntitas entitas, out int statu)
        {
            base.FreeEntitas(mid, ref entitas, out statu);
            if (statu == 0)
            {
                ICommonRole role = entitas as ICommonRole;
                int campID = role.Camp;
                List<int> list;
                if (mCampRoles.IsContainsKey(campID))
                {
                    list = mCampRoles[campID];
                    list.Remove(mid);
                }
                if (mAllRoles.Contains(mid))
                {
                    mAllRoles.Remove(mid);
                }
            }
        }

        public override void Execute(int time, ref IShipDockEntitas target)
        {
            base.Execute(time, ref target);

            mRoleExecuting = target as ICommonRole;
            int id;
            int max = mAllRoles.Count;
            for (int i = 0; i < max; i++)
            {
                id = mAllRoles[i];
                mRoleTarget = GetEntitas(id) as ICommonRole;

                if (WillWalkForRoleEnemyTarget())
                {
                    BeforeAITargetEnemyCheck();
                    if (WillCheckAIRoleEnemyTarget())
                    {
                        mRoleExecuting.FindingPath = true;
                        mRoleExecuting.EnemyTracking = mRoleTarget;
                        AfterAITargetEnemyCheck();
                        break;
                    }
                }
            }
            AfterAITargetEnemyCheck();
        }

        protected abstract void BeforeAITargetEnemyCheck();
        protected abstract void AfterAITargetEnemyCheck();

        private bool WillCheckAIRoleEnemyTarget()
        {
            return !IsIgnoreCampCheck() && 
                    IsAIControllingTarget() &&
                    CheckTrackView() &&
                    HasEnemySet() && 
                    CheckCamp();
        }

        private bool WillWalkForRoleEnemyTarget()
        {
            return (mRoleTarget != default) && (mRoleExecuting != default) && (mRoleTarget.ID != mRoleExecuting.ID);
        }

        /// <summary>
        /// 是否忽略敌对阵营目标的检测检测
        /// </summary>
        protected virtual bool IsIgnoreCampCheck()
        {
            return false;
        }

        /// <summary>
        /// 是否为人工智能控制的角色
        /// </summary>
        protected virtual bool IsAIControllingTarget()
        {
            return !mRoleExecuting.IsUserControlling;
        }

        /// <summary>
        /// 是否设置过敌对目标
        /// </summary>
        protected virtual bool HasEnemySet()
        {
            return (mRoleExecuting.EnemyTracking == default) && (mRoleExecuting != mRoleTarget);
        }

        /// <summary>
        /// 检测角色阵营
        /// </summary>
        protected virtual bool CheckCamp()
        {
            return mRoleTarget.Camp != mRoleExecuting.Camp;
        }

        /// <summary>
        /// 检测目标距离是否在视野内
        /// </summary>
        protected virtual bool CheckTrackView()
        {
            float distance = Vector3.Distance(mRoleTarget.Position, mRoleExecuting.Position);
            return distance <= mRoleExecuting.TrackViewField;
        }

        public ICommonRole RoleCreated { get; private set; }
        public abstract string DataServerName { get; }
        public abstract string AddCampRoleResovlerName { get; }
        public abstract string CampRoleCreatedAlias { get; }
    }
}