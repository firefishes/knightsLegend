using ShipDock.Notices;
using ShipDock.Pooling;
using UnityEngine;

namespace KLGame
{

    public class UnderAttackState : KLAnimatorState<KLRoleFSMStateParam>
    {

        public UnderAttackState(int name) : base(name)
        {
            AnimationName = "UnderAttack";
        }

        protected override bool ShouldEnter(ref KLRoleFSMStateParam param)
        {
            RoleSceneComp = param.RoleSceneComp;
            return true;
        }

        protected override void OnEnter(ref KLRoleFSMStateParam param)
        {
            base.OnEnter(ref param);

            mStateParam = param;
            if (mStateParam != default)
            {
                if (mRole == default)
                {
                    mRole = mStateParam.KLRole;
                }
            }
            ReadyMotion(0, mStateParam.SkillMapper, false);
        }

        protected override void OnParamEnqueue(ref KLRoleFSMStateParam param)
        {
            InitMotion(false, Animator);
            Animator.SetBool("IsAtkedCombo", true);
        }

        protected override void DuringState(int time)
        {
            base.DuringState(time);
            
            if (Animator.GetBool("IsAtkedCombo"))
            {
                Animator.SetBool("IsAtkedCombo", false);
            }
            else if(mAniUpdater != default && mAniUpdater.HasCompleted)
            {
                Finish();
            }
        }

        protected override bool CheckBeforeFinish()
        {
            bool flag = !Animator.GetBool("IsAtkedCombo");

            if (flag)
            {
                Animator.SetFloat("Atked", 0f);
                Notice notice = Pooling<Notice>.From();
                RoleSceneComp.Broadcast(KLConsts.N_AFTER_UNDER_ATTACK, notice);
                notice.ToPool();
            }
            return flag;
        }
        
        protected override bool Finish()
        {
            bool result = base.Finish();
            
            if (result)
            {
                GetFSM().ChangeState(NormalRoleStateName.GROUNDED);
            }

            return result;
        }
    }
}