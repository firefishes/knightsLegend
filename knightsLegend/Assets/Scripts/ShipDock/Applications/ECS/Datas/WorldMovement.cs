using System;
using UnityEngine;

namespace ShipDock.Applications
{
    public class WorldMovement
    {
        private Vector3 mMoveDirection;
        private Vector3 mTrackingPosition;

        public WorldMovement()
        {
        }

        public Vector3 TrackingPosition
        {
            get
            {
                return mTrackingPosition;
            }
            set
            {
                mTrackingPosition = value;
                MoveDirection = value - Position;
            }
        }

        public Vector3 MoveDirection
        {
            get
            {
                return mMoveDirection.normalized;
            }
            set
            {
                mMoveDirection = value;
            }
        }

        public bool IsPathing { get; private set; }
        public bool IsArrivedPathEnd { get; set; }
        public bool HasBlockInFront { get; set; }
        public bool Invalid { get; set; }
        public float MoveSpeed { get; set; }
        public float SpeedMax { get; set; }
        public float SpeedRevert { get; private set; }
        public float MoveSpeedRatio { get; private set; }
        public Vector3 ClusteringDirection { get; set; }
        public Vector3 ClusteringPosition { get; set; }
        public Vector3 LocalPosition { get; set; }
        public Vector3 Position { get; protected set; }
        public Vector3 InitPosition { get; set; }
        public Quaternion Rotation { get; protected set; }

        /// <summary>
        /// 设置旋转
        /// </summary>
        public virtual void SetRotation(Quaternion value)
        {
            Rotation = value;
        }

        /// <summary>
        /// 设置坐标
        /// </summary>
        public virtual void SetPosition(Vector3 value)
        {
            Position = value;
        }

        /// <summary>
        /// 检测是否抵达寻路目的地
        /// </summary>
        public bool CheckingArrival(float limit = 0.1f, bool isCheckPathingEnd = true)
        {
            float remaining = DistanceBetween(TrackingPosition);
            bool result = remaining <= limit;
            if (result && isCheckPathingEnd)
            {
                PathingEnd();
            }
            return result;
        }

        /// <summary>
        /// 获取与给定点的距离
        /// </summary>
        public float DistanceBetween(Vector3 target)
        {
            float dist = Mathf.Abs(Vector3.Distance(Position, target));
            return dist;
        }

        /// <summary>
        /// 是否可以寻路
        /// </summary>
        public bool ShouldTracking()
        {
            return !IsPathing;
        }

        /// <summary>
        /// 设置寻路启动标记
        /// </summary>
        public void SetStartPathingFlag()
        {
            IsPathing = true;
            IsArrivedPathEnd = false;
        }

        /// <summary>
        /// 寻路结束
        /// </summary>
        public virtual void PathingEnd()
        {
            IsPathing = false;
            IsArrivedPathEnd = true;
        }

        /// <summary>
        /// 重置中止移动时保存的移动数据
        /// </summary>
        public void ResetSpeedRevert()
        {
            SpeedRevert = 0f;
        }

        /// <summary>
        /// 中止移动，保存当前移动速度或数据，等待恢复
        /// </summary>
        public void StopInAbeyance()
        {
            if (MoveSpeed > 0f)
            {
                SpeedRevert = MoveSpeed;
            }
            MoveSpeed = 0f;
        }

        /// <summary>
        /// 检测移动是否被中止过
        /// </summary>
        public void RevertFromAbeyance()
        {
            if (SpeedRevert > 0f)
            {
                MoveSpeed = SpeedRevert;
                SpeedRevert = 0f;
            }
        }

        /// <summary>
        /// 设置移动步幅
        /// </summary>
        public void SetMoveStepLength(float moveStepLength)
        {
            MoveSpeedRatio = SpeedMax / moveStepLength;
        }
    }
}