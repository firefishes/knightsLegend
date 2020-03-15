using ShipDock.ECS;
using UnityEngine;

namespace ShipDock.Applications
{
    public interface ICommonRole : IPathFindable, ICollidableRole, IStatesRole
    {
        void SetRoleData(IRoleData data);
        void SetSourceID(int id);
        float GetDistFromMainLockDown();
        bool IsUserControlling { get; set; }
        bool IsGrounded { get; set; }
        bool IsGroundedAndCrouch { get; set; }
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
