using UnityEngine;

namespace Spine.Unity
{
    public class SkeletonAnimationIgnoreTimeScale : SkeletonAnimation
    {
        protected override void Update()
        {
            Update(Time.unscaledDeltaTime);
        }
    }
}