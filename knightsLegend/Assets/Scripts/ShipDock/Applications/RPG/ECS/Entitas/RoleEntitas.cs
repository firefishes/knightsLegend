using ShipDock.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{
    public abstract class RoleEntitas : EntitasComponentable, ICommonRole
    {

        public override void InitComponents()
        {
            base.InitComponents();

            RoleAnimatorInfo = new CommonRoleAnimatorInfo();
            SetRoleInputInfo();
        }

        protected virtual void SetRoleInputInfo()
        {
            RoleInput = CreateRoleInputInfo();
        }

        protected virtual IRoleInput CreateRoleInputInfo()
        {
            return new RoleInputInfo(this);
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

        public float GetDistFromMainLockDown()
        {
            return EnemyMainLockDown != default ? Vector3.Distance(Position, EnemyMainLockDown.Position) : float.MaxValue;
        }

        public virtual void AfterGetStopDistance(float dist, Vector3 entitasPos)
        {
        }

        public virtual float GetStopDistance()
        {
            return 2f;
        }

        public void AddCollidingPos(int cid, Vector3 pos)
        {
            List<RoleColldingPos> list;
            if(CollidingPos.ContainsKey(cid))
            {
                list = CollidingPos[cid];
            }
            else
            {
                list = new List<RoleColldingPos>();
                CollidingPos[cid] = list;
            }
            RoleColldingPos colldingPos = new RoleColldingPos
            {
                colldingID = cid,
                pos = pos
            };
            list.Add(colldingPos);
        }

        public List<RoleColldingPos> ColldingList(int cid)
        {
            return CollidingPos[cid];
        }

        public abstract void CollidingChanged(int colliderID, bool isTrigger, bool isCollided);

        public bool Gravity { get; set; }
        public bool IsGrounded { get; set; }
        public bool IsGroundedAndCrouch { get; set; }
        public bool IsUserControlling { get; set; }
        public bool PositionEnabled { get; set; } = true;
        public bool FindngPath { get; set; }
        public int Camp { get; set; }
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
        public KeyValueList<int, List<RoleColldingPos>> CollidingPos { get; } = new KeyValueList<int, List<RoleColldingPos>>();
        public ICommonRole EnemyMainLockDown { get; set; }
        public IRoleData RoleDataSource { get; private set; }
        public IRoleInput RoleInput { get; set; }
        public Action<int, int, bool, bool> CollidingChanger { get; set; }
        public CommonRoleMustSubgroup RoleMustSubgroup { get; set; }
        public CommonRoleAnimatorInfo RoleAnimatorInfo { get; private set; }

        protected override int[] ComponentIDs { get; } = default;
    }

    public class RoleColldingPos
    {
        public int id;
        public int colldingID;
        public Vector3 pos;
    }
}


