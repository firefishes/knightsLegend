using UnityEngine;

namespace KLGame
{
    public class NormalDefenceState : KLAnimatorState<KLRoleFSMStateParam>
    {
        public NormalDefenceState(int name) : base(name)
        {
            mAnimationName = "Defence_1";
        }

        protected override bool ShouldEnter(ref KLRoleFSMStateParam param)
        {
            mStateParam = param;
            mRole = mStateParam.KLRole;
            RoleSceneComp = mStateParam.RoleSceneComp;

            return base.ShouldEnter(ref param);
        }

        protected override void OnEnter(ref KLRoleFSMStateParam param)
        {
            base.OnEnter(ref param);

            ReadyMotion(mStateParam.CurrentSkillID, mStateParam.SkillMapper, false);
        }

        protected override void DuringState(int time)
        {
            bool flag = mAniUpdater.Update(this);
            Finish();
        }

        protected override bool CheckBeforeFinish()
        {
            return Animator.GetFloat("DefenceType") < 1f;
        }
    }
}
