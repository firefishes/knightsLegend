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
                Finish(false);
            }
        }

        protected override bool BeforeFinish(bool checkInputWhenFinish)
        {
            bool isAtkedCombo = Animator.GetBool("IsAtkedCombo");
            bool flag = base.BeforeFinish(checkInputWhenFinish) && !isAtkedCombo;
            
            if(flag)
            {
                Animator.SetFloat("Atked", 0f);
                //try
                //{
                //mRole.RoleInput.SetInputPhase(UserInputPhases.ROLE_INPUT_PHASE_AFTER_MOVE, false);
                //mStateParam.RoleSceneComp.MoveBlock = false;
                //}
                //catch(System.Exception e)
                //{
                //    Debug.Log(e.Message);
                //}
                Notice notice = Pooling<Notice>.From();
                RoleSceneComp.Broadcast(KLConsts.N_AFTER_UNDER_ATTACK, notice);
                notice.ToPool();
            }
            return flag;
        }

        protected override bool Finish(bool checkInputWhenFinish)
        {
            bool result = base.Finish(checkInputWhenFinish);
            
            if (result)
            {
                GetFSM().ChangeState(NormalRoleStateName.GROUNDED);
            }

            return result;
        }

        protected override void RevertAllStateParams()
        {
            foreach (var item in mStateParamQueue)
            {
                item.ToPool();
            }
        }

        protected override void RevertStateParam()
        {
            if (mStateParam != default)
            {
                mStateParam.ToPool();
            }
        }
    }
}