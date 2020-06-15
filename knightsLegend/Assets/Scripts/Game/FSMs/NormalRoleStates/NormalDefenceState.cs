using ShipDock.Pooling;
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
            if (mRole is IAIRole role)
            {
                Animator.SetFloat("DefenceType", 1f);

                TimingTaskNotice notice = Pooling<TimingTaskNotice>.From();
                float defenceTime = role.AISensor.GetDefCancelThikingTime();
                notice.ReinitForStart(KLConsts.T_AI_THINKING, KLConsts.T_AI_THINKING_TIME_TASK_AI_WAITED, defenceTime);
                role.Dispatch(KLConsts.N_ROLE_TIMING, notice);
            }
            mRole.DefenceType = (int)Animator.GetFloat("DefenceType");
        }

        protected override void DuringState(int time)
        {
            bool flag = mAniUpdater.Update(this);
            Finish();
        }

        protected override bool CheckBeforeFinish()
        {
            float defenceType = Animator.GetFloat("DefenceType");
            bool result = defenceType < 1f;
            if (result)
            {
                mRole.DefenceType = (int)Animator.GetFloat("DefenceType");
            }
            return result;
        }
    }
}
