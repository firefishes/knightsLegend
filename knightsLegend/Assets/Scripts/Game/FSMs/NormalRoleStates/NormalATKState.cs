using ShipDock.Applications;
using ShipDock.Notices;
using ShipDock.Pooling;
using UnityEngine;

namespace KLGame
{
    public class NormalATKState : KLAnimatorState<NormalATKStateParam>, IAssailableCommiter
    {
        public NormalATKState(int name) : base(name)
        {
        }

        protected override void OnEnter(ref NormalATKStateParam param)
        {
            base.OnEnter(ref param);

            var input = param.Inpunts;
            int sValue = input.Dequeue();

            if (sValue == 1)
            {
                mStateParam = param;
                mRole = mStateParam.KLRole;
                ReadyMotion(mStateParam.CurrentSkillID, mStateParam.SkillMapper, true);
            }
        }

        protected override bool ShouldParamEnqueue(ref NormalATKStateParam param)
        {
            bool result = base.ShouldParamEnqueue(ref param) || IsHit;
            if (!result && param != default)
            {
                Pooling<NormalATKStateParam>.To(param);
            }
            return result;
        }

        protected override void SpeedFrozenEnd()
        {
            base.SpeedFrozenEnd();

            switch (StateFeedback)
            {
                case 0:
                    DeActiveCollider();
                    break;
                case 1:
                    DeActiveCollider();
                    Finish(true);
                    break;
            }
        }

        protected override bool BeforeFinish(bool checkInputWhenFinish)
        {
            bool result = base.BeforeFinish(checkInputWhenFinish);
            if(result)
            {
                mStateParam.RoleSceneComp.MoveBlock = false;
            }
            else
            {
                IsHit = false;
            }
            return result;
        }

        override protected bool Finish(bool checkInputWhenFinish)
        {
            bool result = base.Finish(checkInputWhenFinish);

            if(result)
            {
                IsHit = false;
                GetFSM().ChangeState(NormalRoleStateName.GROUNDED);
            }

            return result;
        }

        protected override void RevertStateParam()
        {
            if (mStateParam != default)
            {
                Pooling<NormalATKStateParam>.To(mStateParam);
            }
        }

        protected override void RevertAllStateParams()
        {
            foreach (var item in mStateParamQueue)
            {
                Pooling<NormalATKStateParam>.To(item);
            }
        }

        protected override bool ReadyMotion(int skillID, SkillMotionsMapper mapper, bool isCombo)
        {
            bool result = base.ReadyMotion(skillID, mapper, isCombo);
            if (result)
            {
                IsFeedbackChecked = false;
                mAnimationName = "NormalAttack_".Append(mComboMotion.CurrentCombo.ToString());

                KLConsts.S_KL.DeliveParam<KLServer, bool>("True", "Bool", OnTriggerRoleActive);
            }
            return result;
        }

        private void OnTriggerRoleActive(ref IParamNotice<bool> target)
        {
            target.SetNoticeName(KLConsts.N_TRIGGER_ROLE_ACTIVE);
            target.NotifcationSender = mRole;
            mRole.Broadcast(target);

            KLConsts.S_KL.Revert<KLServer>("Bool", target);
        }

        public virtual bool HitCommit()
        {
            if (mStateParam == default)
            {
                return false;
            }
            mStateParam.FillValues();

            PlayerHit hit = Pooling<PlayerHit>.From();
            hit.Reinit(mRole);

            hit.AfterProcessing = OnATKHit;
            hit.HitInfoScope.validAngle = 120f;
            hit.HitInfoScope.minDistance = 2.5f;
            hit.HitInfoScope.startPos = mStateParam.StartPos;
            hit.HitInfoScope.startRotation = mStateParam.StartRotation;
            hit.HitInfoScope.Draws();

            return mRole.Processing.AddProcess(hit);
        }

        protected void OnATKHit()
        {
            IsHit = true;
            StartFeedbackTime(0, 2f, 0.3f);

            Debug.Log("hit");
        }

        private void DeActiveCollider()
        {
            var notice = Pooling<ParamNotice<bool>>.From();
            notice.SetNoticeName(KLConsts.N_TRIGGER_ROLE_ACTIVE);
            notice.NotifcationSender = mRole;
            notice.ParamValue = false;
            mRole.Broadcast(notice);
            Pooling<ParamNotice<bool>>.To(notice);
        }

        private bool IsHit { get; set; }
    }
}