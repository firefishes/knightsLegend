using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{
    public class RoleEntitas : EntitasComponentable, ICommonRole
    {
        public override void InitComponents()
        {
            base.InitComponents();

            RoleAnimatorInfo = new CommonRoleAnimatorInfo();
            SetRoleInputInfo();
        }

        protected virtual void SetRoleInputInfo()
        {
            RoleInput = new RoleInputInfo(this);
        }

        public void SetRoleData(IRoleData data)
        {
            RoleDataSource = data;
            Speed = RoleDataSource.Speed;
        }

        public void SetPahterTarget(Vector3 value)
        {
            PatherTargetPosition = value;
        }

        public void SetSourceID(int id)
        {
            SourceID = id;
        }

        public float GetDistFromMainLockDown()
        {
            return EnemyMainLockDown != default ? Vector3.Distance(Position, EnemyMainLockDown.Position) : float.MaxValue;
        }

        public bool Gravity { get; set; }
        public bool IsGrounded { get; set; }
        public bool IsGroundedAndCrouch { get; set; }
        public bool IsUserControlling { get; set; }
        public bool PositionEnabled { get; set; } = true;
        public bool FindngPath { get; set; }
        public int SourceID { get; private set; }
        public int[] States { get; private set; }
        public float Speed { get; set; }
        public float SpeedCurrent { get; set; }
        public string Name { get; set; }
        public Vector3 Direction { get; set; }
        public Vector3 Position { get; set; }
        public Vector3 PostionTarget { get; set; }
        public Vector3 GroundNormal { get; set; }
        public Vector3 PatherTargetPosition { get; private set; }
        public Vector3 CameraForward { get; set; }
        public List<int> CollidingRoles { get; } = new List<int>();
        public ICommonRole EnemyMainLockDown { get; set; }
        public IRoleData RoleDataSource { get; private set; }
        public IRoleInput RoleInput { get; set; }
        public CommonRoleMustSubgroup RoleMustSubgroup { get; set; }
        public CommonRoleAnimatorInfo RoleAnimatorInfo { get; private set; }

        protected override int[] ComponentIDs { get; } = default;
    }

}


