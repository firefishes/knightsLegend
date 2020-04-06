using ShipDock.Applications;
using ShipDock.Pooling;
using System;
using UnityEngine;

namespace KLGame
{

    public class ForceMover : IPoolable, IForceMover
    {
        public static ForceMover Create()
        {
            return Pooling<ForceMover>.From();
        }

        public ForceMover() : base()
        {
            Timing = new TimingTaskEntitas();
        }

        public void Dispose()
        {
            RemoveMover();
            Timing?.Dispose();
            Timing = default;
        }

        public void Revert()
        {
            RemoveMover();
        }

        public void SetMover(ICommonRole target, Vector3 v, float time, Action onCompletion = default)
        {
            if (MoveTarget != default || target == default)
            {
                return;
            }
            MoveTarget = target;
            V = v;
            MoveTarget.RoleInput.AddForceMove(this);
            
            TimingTasker = Timing.AddForceMoveTiming(MoveTarget.ID);
            TimingTasker.Start(time);
            if (onCompletion != default)
            {
                TimingTasker.completion += onCompletion;
            }
            TimingTasker.completion += RemoveMover;
        }

        public void RemoveMover()
        {
            if (MoveTarget != default)
            {
                Timing.RemoveForceMoveTiming(MoveTarget.ID, false, true);
                TimingTasker = default;

                V = -V;
                MoveTarget.RoleInput.AddForceMove(this);
                MoveTarget = default;
                V = Vector3.zero;

                if (ApplyPooling)
                {
                    Pooling<ForceMover>.To(this);
                }
                else
                {
                    Dispose();
                }
            }
        }
        
        public Vector3 GetMoverVector()
        {
            return V;
        }
        
        private ICommonRole MoveTarget { get; set; }
        private TimingTasker TimingTasker { get; set; }
        private TimingTaskEntitas Timing { get; set; }
        private Vector3 V { get; set; }

        public bool ApplyPooling { get; set; } = true;

    }
}