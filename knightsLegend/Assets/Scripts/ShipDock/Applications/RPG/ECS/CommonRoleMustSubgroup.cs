using System;
using UnityEngine;

namespace ShipDock.Applications
{
    [Serializable]
    public struct CommonRoleMustSubgroup
    {
        public int roleColliderID;
        public int rigidbodyID;
        public int animatorID;
        public float capsuleHeight;
        public Vector3 capsuleCenter;
        public float origGroundCheckDistance;

        public void Init(ref CapsuleCollider target)
        {
            capsuleHeight = target.height;
            capsuleCenter = target.center;
        }
    }
}


