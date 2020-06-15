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

        public virtual void SetRoleData(IRoleData data)
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
            return EnemyTracking != default ? Vector3.Distance(Position, EnemyTracking.Position) : float.MaxValue;
        }

        public virtual bool AfterGetStopDistance(float dist, Vector3 entitasPos)
        {
            return true;
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
        public bool FindingPath { get; set; }
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
        public ICommonRole EnemyTracking { get; set; }
        public IRoleData RoleDataSource { get; private set; }
        public IRoleInput RoleInput { get; set; }
        /// <summary>
        /// 角色的碰撞触发回调函数
        /// </summary>
        /// <param name="entitasID">角色实体id</param>
        /// <param name="colliderID">角色检测到的碰撞体id</param>
        /// <param name="isTrigger">是否为触发器</param>
        /// <param name="isCollided">如果为触发器，其值是否为已触发</param>
        public Action<int, int, bool, bool> CollidingChanger { get; set; }
        public CommonRoleMustSubgroup RoleMustSubgroup { get; set; }
        public CommonRoleAnimatorInfo RoleAnimatorInfo { get; private set; }
        public bool AfterGetStopDistChecked { get; set; }
        public virtual float TrackViewField { get; set; }
        protected override int[] ComponentNames { get; } = default;
    }

    public class RoleColldingPos
    {
        public int id;
        public int colldingID;
        public Vector3 pos;
    }
}


