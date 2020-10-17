using UnityEngine;

namespace ShipDock.Applications
{
    public class WorldMovement
    {
        private Vector3 mMoveDirection;
        private Vector3 mTrackingPosition;

        public virtual void SetRotation(Quaternion value)
        {
            Rotation = value;
        }

        public virtual void SetPosition(Vector3 value)
        {
            Position = value;
        }

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

        public float DistanceBetween(Vector3 target)
        {
            float dist = Mathf.Abs(Vector3.Distance(Position, target));
            return dist;
        }

        public bool ShouldTracking()
        {
            return !IsPathing;
        }

        public void SetStartPathingFlag()
        {
            IsPathing = true;
            IsArrivedPathEnd = false;
        }

        public virtual void PathingEnd()
        {
            IsPathing = false;
            IsArrivedPathEnd = true;
        }

        public void StopInAbeyance()
        {
            if (MoveSpeed > 0f)
            {
                SpeedRevert = MoveSpeed;
            }
            MoveSpeed = 0f;
        }

        public void CheckAbeyanceSpeed()
        {
            if (SpeedRevert > 0f)
            {
                MoveSpeed = SpeedRevert;
                SpeedRevert = 0f;
            }
        }

        public void SetMoveStepLength(float moveStepLength)
        {
            MoveSpeedRatio = SpeedMax / moveStepLength;
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

        public bool IsPathing { get; set; }
        public bool IsArrivedPathEnd { get; set; }
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
    }
}