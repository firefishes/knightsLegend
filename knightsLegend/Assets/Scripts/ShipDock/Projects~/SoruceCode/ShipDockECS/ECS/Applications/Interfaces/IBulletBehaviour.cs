
using UnityEngine;

namespace ShipDock.Applications
{
    public interface IBulletBehaviour
    {
        void AfterDoHit<T>(T data) where T : BulletData;
        void BulletFinish();
        Ray Ray { get; }
        RaycastHit RayHit { get; }
        GameObject HitTarget { get; set; }
    }
}