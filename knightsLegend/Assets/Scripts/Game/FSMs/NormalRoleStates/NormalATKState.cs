﻿using ShipDock.Applications;
using ShipDock.Notices;
using ShipDock.Pooling;
using UnityEngine;

namespace KLGame
{
    public class NormalATKState : KLAnimatorState<NormalATKStateParam>, IAssailableCommiter
    {
        protected IRoleProcessing mHit;

        public NormalATKState(int name) : base(name)
        {
        }

        protected override bool ShouldEnter(ref NormalATKStateParam param)
        {
            var input = param.Inpunts;
            int sValue = input.Dequeue();
            bool result = sValue == 1;//按下攻击键
            if(result)
            {
                RoleSceneComp = param.RoleSceneComp;
            }
            return result;
        }

        protected override void OnEnter(ref NormalATKStateParam param)
        {
            base.OnEnter(ref param);

            mStateParam = param;
            mRole = mStateParam.KLRole;

            ReadyMotion(mStateParam.CurrentSkillID, mStateParam.SkillMapper, true);
        }

        protected override bool ShouldParamEnqueue(ref NormalATKStateParam param)
        {
            bool result = base.ShouldParamEnqueue(ref param) || IsHit;
            if (result)
            {
                mHit?.ToPool();
                mHit = default;
            }
            else
            {
                param?.ToPool();
            }
            return result;
        }

        protected override void SpeedFrozenEnd()
        {
            base.SpeedFrozenEnd();

            switch (StateFeedback)
            {
                case RoleAnimationFeedBackConsts.FEED_BACK_BY_HIT:
                    DeActiveCollider();
                    break;
                case 1:
                    DeActiveCollider();
                    Finish();
                    break;
            }
        }

        protected override bool CheckBeforeFinish()
        {
            bool result = base.CheckBeforeFinish();
            if (result)
            {
                Animator.SetFloat("Atk1", 0f);
            }
            else
            {
                IsHit = false;
            }
            return result;
        }

        override protected bool Finish()
        {
            bool result = base.Finish();

            if(result)
            {
                IsHit = false;
                RoleSceneComp = default;
                mHit?.ToPool();
                mHit = default;
                GetFSM().ChangeState(NormalRoleStateName.GROUNDED);
            }

            return result;
        }

        protected override bool ReadyMotion(int skillID, SkillMotionsMapper mapper, bool isCombo)
        {
            bool result = base.ReadyMotion(skillID, mapper, isCombo);
            if (result)
            {
                RoleSceneComp?.RoleFSMStateCombo(StateName);

                IsFeedbackChecked = false;
                mAnimationName = "NormalAttack_".Append(mComboMotion.CurrentCombo.ToString());

                KLConsts.S_KL.DeliveParam<KLServer, bool>("True", "Bool", OnTriggerRoleActive);
            }
            return result;
        }

        private void OnTriggerRoleActive(ref IParamNotice<bool> target)
        {
            mRole.Dispatch(KLConsts.N_TRIGGER_ROLE_ACTIVE, target);
            KLConsts.S_KL.Revert<KLServer>("Bool", target);
        }

        public virtual bool HitCommit(int hitCollidID)
        {
            if (mStateParam == default)
            {
                return false;
            }
            mStateParam.FillValues();

            mHit = Pooling<PlayerHit>.From();
            PlayerHit hit = mHit as PlayerHit;
            hit.Reinit(mRole);

            hit.HitColliderID = hitCollidID;
            hit.AfterProcessing = OnATKHit;
            hit.HitInfoScope.validAngle = 120f;
            hit.HitInfoScope.minDistance = 2.5f;
            hit.HitInfoScope.startPos = mStateParam.StartPos;
            hit.HitInfoScope.startRotation = mStateParam.StartRotation;

            return mRole.Processing.AddRoleProcess(hit);
        }

        protected void OnATKHit()
        {
            IsHit = true;
            StartFeedbackTime(RoleAnimationFeedBackConsts.FEED_BACK_BY_HIT, 0.007f, 0f);
        }

        private void DeActiveCollider()
        {
            var notice = Pooling<ParamNotice<bool>>.From();
            notice.ParamValue = false;
            mRole.Dispatch(KLConsts.N_TRIGGER_ROLE_ACTIVE, notice);

            notice.ToPool();
        }

        private bool IsHit { get; set; }
    }
}