using ShipDock.ECS;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{
    public interface ICommonRole : IPathFindable, ICollidableRole, IStatesRole, IEntitasComponentable
    {
        void SetRoleData(IRoleData data);
        void AddCollidingPos(int cid, Vector3 pos);
        float GetDistFromMainLockDown();
        List<RoleColldingPos> ColldingList(int cid);
        IRoleInput RoleInput { get; set; }
        IRoleData RoleDataSource { get; }
        ICommonRole TargetTracking { get; set; }
        CommonRoleMustSubgroup RoleMustSubgroup { get; set; }
        CommonRoleAnimatorInfo RoleAnimatorInfo { get; }
        Vector3 GroundNormal { get; set; }
        Vector3 CameraForward { get; set; }
        float TrackViewField { get; set; }
        string Name { get; set; }
        bool IsUserControlling { get; set; }
        bool IsGrounded { get; set; }
        bool IsGroundedAndCrouch { get; set; }
        bool IsStartTrancking { get; set; }
        int Camp { get; }
    }
}
