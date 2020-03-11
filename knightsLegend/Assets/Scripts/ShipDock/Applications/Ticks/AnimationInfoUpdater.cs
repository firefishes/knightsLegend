using System;
using UnityEngine;

namespace ShipDock.Applications
{
    public class AnimationInfoUpdater : MethodUpdater
    {

        private float mLength;

        public AnimationInfoUpdater()
        {
            HasCompleted = true;
        }

        public override void Dispose()
        {
            base.Dispose();

            Completion = default;
            AnimatorTarget = default;
            ShortNameHash = 0;
        }

        public void Reset(Animator animator, Action<Animator> method = default)
        {
            HasCompleted = false;
            AnimatorTarget = animator;
            mLength = StateInfo.length;
            ShortNameHash = StateInfo.shortNameHash;
            Completion = method;
        }

        public override void OnUpdate(int dTime)
        {
            base.OnUpdate(dTime);

            mLength -= Time.deltaTime;
            if (mLength <= 0f)
            {
                Completion?.Invoke(AnimatorTarget);
                HasCompleted = true;
            }
        }

        public AnimatorStateInfo StateInfo
        {
            get
            {
                return AnimatorTarget.GetCurrentAnimatorStateInfo(0);
            }
        }

        public Action<Animator> Completion { get; set; }
        public Animator AnimatorTarget { get; private set; }
        public int ShortNameHash { get; private set; }
        public bool HasCompleted { get; private set; }
    }
}