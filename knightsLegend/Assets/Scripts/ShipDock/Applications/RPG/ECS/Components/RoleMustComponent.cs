using ShipDock.ECS;
using ShipDock.Tools;

namespace ShipDock.Applications
{
    public class RoleMustComponent : ShipDockComponent
    {

        private ICommonRole mRoleItem = default;
        private KeyValueList<ICommonRole, int> mRigidbodySubgourp;
        private KeyValueList<ICommonRole, int> mRoleColliderSubgourp;
        private KeyValueList<ICommonRole, int> mRoleColliderScanedSubgourp;
        private KeyValueList<ICommonRole, int> mAnimatorSubgourp;

        public override int SetEntitas(IShipDockEntitas target)
        {
            int id = base.SetEntitas(target);

            if (id >= 0)
            {
                mRoleItem = target as ICommonRole;

                CommonRoleMustSubgroup subgroup = mRoleItem.RoleMustSubgroup;
                SetSubgroupMap(ref mRigidbodySubgourp, subgroup.rigidbodyID);
                SetSubgroupMap(ref mRoleColliderSubgourp, subgroup.roleColliderID);
                SetSubgroupMap(ref mAnimatorSubgourp, subgroup.animatorID);
                SetSubgroupMap(ref mRoleColliderScanedSubgourp, subgroup.roleScanedColliderID);
            }

            return id;
        }

        protected override void FreeEntitas(int mid, ref IShipDockEntitas entitas, out int statu)
        {
            base.FreeEntitas(mid, ref entitas, out statu);

            mRoleItem = entitas as ICommonRole;

            RemoveSubgroupMap(ref mRigidbodySubgourp, ref mRoleItem);
            RemoveSubgroupMap(ref mRoleColliderSubgourp, ref mRoleItem);
            RemoveSubgroupMap(ref mAnimatorSubgourp, ref mRoleItem);
            RemoveSubgroupMap(ref mRoleColliderScanedSubgourp, ref mRoleItem);
            
        }

        private void SetSubgroupMap(ref KeyValueList<ICommonRole, int> mapper, int mid)
        {
            if(mapper == default)
            {
                mapper = new KeyValueList<ICommonRole, int>();
            }
            if(!mapper.ContainsKey(mRoleItem))
            {
                mapper[mRoleItem] = mid;
            }
        }

        private void RemoveSubgroupMap(ref KeyValueList<ICommonRole, int> mapper, ref ICommonRole target)
        {
            if (mapper.ContainsKey(mRoleItem))
            {
                mapper.Remove(target);
            }
        }

        public int GetRigidbody(ref ICommonRole target)
        {
            return mRigidbodySubgourp.ContainsKey(target) ? mRigidbodySubgourp[target] : default;
        }

        public int GetCollider(ref ICommonRole target)
        {
            return mRoleColliderSubgourp.ContainsKey(target) ? mRoleColliderSubgourp[target] : default;
        }

        public int GetAnimator(ref ICommonRole target)
        {
            return mAnimatorSubgourp.ContainsKey(target) ? mAnimatorSubgourp[target] : default;
        }

        public int GetColliderScaned(ref ICommonRole target)
        {
            return mRoleColliderScanedSubgourp.ContainsKey(target) ? mRoleColliderScanedSubgourp[target] : default;
        }
    }

}
