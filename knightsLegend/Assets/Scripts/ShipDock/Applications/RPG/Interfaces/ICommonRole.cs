using ShipDock.ECS;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{
    public interface ICommonRole : IPathFindable, ICollidableRole, IStatesRole, IShipDockEntitas
    {
        void SetRoleData(IRoleData data);
        void SetSourceID(int id);
        float GetDistFromMainLockDown();
        bool IsUserControlling { get; set; }
        bool IsGrounded { get; set; }
        bool IsGroundedAndCrouch { get; set; }
        void AddCollidingPos(int cid, Vector3 pos);
        List<RoleColldingPos> ColldingList(int cid);
        int SourceID { get; }
        int Camp { get; }
        string Name { get; set; }
        IRoleInput RoleInput { get; set; }
        IRoleData RoleDataSource { get; }
        ICommonRole EnemyMainLockDown { get; set; }
        Vector3 GroundNormal { get; set; }
        Vector3 CameraForward { get; set; }
        CommonRoleMustSubgroup RoleMustSubgroup { get; set; }
        CommonRoleAnimatorInfo RoleAnimatorInfo { get; }
    }
}
