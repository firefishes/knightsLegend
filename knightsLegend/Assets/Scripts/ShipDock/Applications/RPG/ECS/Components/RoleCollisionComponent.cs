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
            }
        }

        public override void Execute(int time, ref IShipDockEntitas target)
        {
            base.Execute(time, ref target);

            int blockID;
            mRole = target as ICommonRole;

            bool isGetEnemy = false;
            mRoleColliding = mRole.CollidingRoles;
            int max = mRoleColliding.Count;
            if (max > 0)
            {
                for (int i = 0; i < max; i++)
                {
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
        }
    }
}

