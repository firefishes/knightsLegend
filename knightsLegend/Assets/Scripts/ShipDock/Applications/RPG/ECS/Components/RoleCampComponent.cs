using ShipDock.ECS;
using ShipDock.Notices;
using ShipDock.Tools;
using System.Collections.Generic;

namespace ShipDock.Applications
{
    public abstract class RoleCampComponent : ShipDockComponent
    {
        protected IDataServer mDataServer;
        protected ICommonRole mRoleTarget;
        private ICommonRole mRoleEntitas;
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

            mRoleTarget = target as ICommonRole;
            int id;
            int max = mAllRoles.Count;
            for (int i = 0; i < max; i++)
            {
                id = mAllRoles[i];
                mRoleEntitas = GetEntitas(id) as ICommonRole;
                if ((mRoleEntitas != default) && (mRoleTarget != default))
                {
                    if (!IsIgnoreCheckEnemyByCamp() && IsAIControllingTarget() && HasEnemySet() && CheckCamp())
                    {
                        mRoleTarget.FindingPath = true;
                        mRoleTarget.EnemyMainLockDown = mRoleEntitas;
                        break;
                    }
                }
            }
        }

        protected virtual bool IsIgnoreCheckEnemyByCamp()
        {
            return false;
        }

        protected virtual bool IsAIControllingTarget()
        {
            return !mRoleTarget.IsUserControlling;
        }

        protected virtual bool HasEnemySet()
        {
            return (mRoleTarget.EnemyMainLockDown == default) && (mRoleTarget != mRoleEntitas);
        }

        protected virtual bool CheckCamp()
        {
            return mRoleEntitas.Camp != mRoleTarget.Camp;
        }

        public ICommonRole RoleCreated { get; private set; }
        public abstract string DataServerName { get; }
        public abstract string AddCampRoleResovlerName { get; }
        public abstract string CampRoleCreatedAlias { get; }
    }
}