using ShipDock.Interfaces;
using ShipDock.Tools;
using UnityEngine;

namespace ShipDock.Applications
{
    public class BulletData : IDispose
    {
        private Ray mRay;
        private RaycastHit mRayHit;

        public bool IsHit { get; set; }
        public bool IsShowHit { get; set; }
        public bool OutOfRange { get; private set; }
        public int RayMask { get; set; }
        public int HitLayer { get; set; }
        public int HitID { get; set; }
        public float MuzzleVelocity { get; set; }
        public float ShotRange { get; set; } = 100f;
        public float RayDistance { get; set; }
        public Vector3 HitDirection { get; set; }
        public Vector3 InitPosition { get; set; }
        public Vector3 NextFramePos { get; set; }
        public Vector3 HitPosition { get; set; }
        public ICommonBulletChecker CommonBulletChecker { get; set; }

        public void Dispose()
        {
            CommonBulletChecker = default;
        }

        public void UpdateBulletData(float time, ref WorldMovement Movement)
        {
            if (Movement != default)
            {
                Vector3 currentPos = Movement.Position;
                Vector3 nextFramePos = Movement.Position + (Movement.MoveDirection * MuzzleVelocity * time);
                RayDistance = Vector3.Magnitude(nextFramePos - currentPos);

                if (RayDistance > 0)
                {
                    if (IsHit)
                    {
                        Movement.SetPosition(HitPosition);
                    }
                    else
                    {
                        NextFramePos = nextFramePos;
                        Movement.SetPosition(nextFramePos);
                    }
                }
                else
                {
                    "Log: Bullet is hit, position is ({0})".Log(HitPosition.ToString());
                }
            }
        }

        public void CheckRaycast(ref Transform transform, ref WorldMovement Movement, ref IBulletBehaviour bulletBehaviour)
        {
            OutOfRange = false;

            if ((RayDistance > 0f) && !IsHit && !IsShowHit)
            {
                transform.position = Movement.Position;
                OutOfRange = Vector3.Distance(Movement.Position, InitPosition) > ShotRange;
            }

            if (IsHit || OutOfRange || IsShowHit)
            {
                bulletBehaviour.BulletFinish();
            }
            else
            {
                mRay = bulletBehaviour.Ray;
                mRayHit = bulletBehaviour.RayHit;
                bool isHit = Utils.Raycast(Movement.Position, Movement.MoveDirection, out mRay, out mRayHit, RayDistance, RayMask);
                if (isHit)
                {
                    bulletBehaviour.HitTarget = mRayHit.transform.gameObject;
                    GameObject hitTarget = bulletBehaviour.HitTarget;

                    int hitLayer = hitTarget.layer;
                    int id = hitTarget.GetInstanceID();
                    DoHit(hitLayer, id, mRayHit.point, Movement.MoveDirection);
                    bulletBehaviour?.AfterDoHit(this);
                    if (!IsShowHit)
                    {
                        IsShowHit = true;
                        CommonBulletChecker?.CacheBulletData(this);//处理命中后的操作
                    }
                }
            }
        }

        public void DoHit(int hitLayer, int id, Vector3 point, Vector3 direction)
        {
            IsHit = true;
            HitID = id;
            HitLayer = hitLayer;
            HitPosition = point;
            HitDirection = direction;
        }
    }
}