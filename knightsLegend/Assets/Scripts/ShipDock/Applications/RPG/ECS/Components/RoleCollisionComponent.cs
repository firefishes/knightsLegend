#define G_LOG
#define LOG_TRY_CATCH_TIRGGER

using System.Collections.Generic;
using ShipDock.ECS;
using ShipDock.Tools;

namespace ShipDock.Applications
{
    public class RoleColliderComponent : ShipDockComponent
    {
        private ICommonRole mRole;
        private ICommonRole mRoleCollidingTarget;
        private List<int> mRoleColliding;
        private KeyValueList<int, ICommonRole> mRoleColliderMapper;

        public override void Init()
        {
            base.Init();

            mRoleColliderMapper = new KeyValueList<int, ICommonRole>();
        }

        public override int SetEntitas(IShipDockEntitas target)
        {
            int id = base.SetEntitas(target);
            if(id >= 0)
            {
                ICommonRole role = target as ICommonRole;
                int subgroupID = role.RoleMustSubgroup.roleColliderID;
                mRoleColliderMapper[subgroupID] = role;

                subgroupID = role.RoleMustSubgroup.roleScanedColliderID;
                mRoleColliderMapper[subgroupID] = role;
            }
            return id;
        }

        protected override void FreeEntitas(int mid, ref IShipDockEntitas entitas, out int statu)
        {
            base.FreeEntitas(mid, ref entitas, out statu);

            if(statu == 0)
            {
                ICommonRole role = entitas as ICommonRole;
                int colliderID = role.RoleMustSubgroup.roleColliderID;
                mRoleColliderMapper.Remove(colliderID);

                colliderID = role.RoleMustSubgroup.roleScanedColliderID;
                mRoleColliderMapper.Remove(colliderID);
            }
        }

        public void RefRoleByColliderID(int blockID, out ICommonRole result)
        {
            result = mRoleColliderMapper[blockID];
        }

        public override void Execute(int time, ref IShipDockEntitas target)
        {
            base.Execute(time, ref target);

            if (IsUpdating)
            {
                return;
            }
            IsUpdating = true;

            int blockID;
            mRole = target as ICommonRole;

            bool isGetEnemy = false;
#if LOG_TRY_CATCH_TIRGGER && UNITY_EDITOR
            try
            {
#endif
            mRoleColliding = mRole.CollidingRoles;
#if LOG_TRY_CATCH_TIRGGER && UNITY_EDITOR
            }
            catch (System.Exception error)
            {
                Testers.Tester.Instance.Log(TesterRPG.Instance, TesterRPG.LOG, "error: role is null");
            }
#endif
            int max = mRoleColliding != default ? mRoleColliding.Count : 0;
            if (max > 0)
            {
                for (int i = 0; i < max; i++)
                {
                    if(i >= mRoleColliding.Count)
                    {
                        break;
                    }
                    blockID = mRoleColliding[i];
                    mRoleCollidingTarget = mRoleColliderMapper[blockID];
                    if (mRoleCollidingTarget != default)
                    {
                        if(mRoleCollidingTarget == mRole.EnemyMainLockDown)
                        {
                            isGetEnemy = true;
                        }
                    }
                }
                if(isGetEnemy)
                {
                    mRole.FindngPath = false;
                    mRole.SpeedCurrent = 0;
                }
                //else
                //{
                //    mRole.EnemyMainLockDown = default;
                //    mRole.FindngPath = true;//TODO 锁定最近的敌人
                //    mRole.SpeedCurrent = mRole.Speed;
                //}
            }
            IsUpdating = false;
        }
    }
}

