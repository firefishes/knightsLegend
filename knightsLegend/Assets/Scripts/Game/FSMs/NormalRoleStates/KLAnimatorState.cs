#define G_LOG

using System.Collections.Generic;
using ShipDock.Applications;
using ShipDock.FSM;
using ShipDock.Testers;
using ShipDock.Tools;
using UnityEngine;

namespace KLGame
{
    public abstract class KLAnimatorState<P> : AnimatorState where P : IStateParam
    {
        protected IKLRole mRole;
        protected P mStateParam;
        protected int mCurrentSkillID = -1;
        protected SkillMotionsMapper mSkillMapper;
        protected Queue<P> mStateParamQueue;
        protected ComboMotionCreater mComboMotion;
        protected AnimationInfoUpdater mAniUpdater;
        protected TimingTasker mFeedbackTime;

        private MethodUpdater mMethodUpdater;
        protected TimingTaskEntitas mRoleStateTimings;

        public KLAnimatorState(int name) : base(name)
        {
            mMethodUpdater = new MethodUpdater()
            {
                FixedUpdate = DuringState
            };
            mStateParamQueue = new Queue<P>();
            mRoleStateTimings = TimingTaskEntitas.Create();
            mRoleStateTimings.CreateMapper();
            mRoleStateTimings.AddTiming(KLConsts.T_ROLE_STATE_FEED_BACK, 0);

            mFeedbackTime = mRoleStateTimings.GetTimingTasker(KLConsts.T_ROLE_STATE_FEED_BACK, 0);
            mFeedbackTime.TotalCount = 1;
        }

        public override void Dispose()
        {
            base.Dispose();

            mRoleStateTimings?.ToPool();

            UpdaterNotice.RemoveSceneUpdater(mMethodUpdater);

            RevertAllStateParams();
            RevertStateParam();

            Utils.Reclaim(mMethodUpdater);
            Utils.Reclaim(ref mStateParamQueue);
            Utils.Reclaim(mComboMotion);

            mRole = default;
            mStateParam = default;
            mSkillMapper = default;
            mComboMotion = default;
            mAniUpdater = default;
            mRoleStateTimings = default;
        }

        public override void InitState(IStateParam param = null)
        {
            base.InitState(param);

            StateFeedback = RoleAnimationFeedBackConsts.FEED_BACK_DEFAULT;

            if (param is P item)
            {
                OnEnter(ref item);
            }
        }
        
        protected virtual void OnEnter(ref P param)
        {

        }

        public override void SetStateParam(IStateParam param)
        {
            base.SetStateParam(param);

            if (param is P item)
            {
                OnParamEnqueue(ref item);
            }
        }

        protected virtual void OnParamEnqueue(ref P param)
        {

            bool result = ShouldParamEnqueue(ref param);
            if (result)
            {
                mStateParamQueue.Enqueue(param);
            }
            else
            {
                if (mComboMotion != default && mComboMotion.ShouldCombo())
                {
                    mComboMotion.Reset(true);

                    RevertStateParam();
                    mStateParam = param;

                    ReadyMotion(mCurrentSkillID, mSkillMapper, true);
                }
                else
                {
                    CancelParamEnqueue(ref param);
                }
            }
        }

        protected virtual void CancelParamEnqueue(ref P param)
        {
            RevertStateParam();
            mStateParam = param;
            RevertStateParam();
        }

        protected virtual bool ShouldParamEnqueue(ref P param)
        {
            bool result = default;
            if (mAniUpdater != default)
            {
                if (mComboMotion != default)
                {
                    result = mComboMotion.ShouldCombo();
                }
                else
                {
                    result = true;
                }
            }
            return result;
        }

        private void StartFromNextParam(bool isCombo)
        {
            RevertStateParam();
            mStateParam = mStateParamQueue.Dequeue();
            ReadyMotion(mCurrentSkillID, mSkillMapper, isCombo);
        }

        protected virtual void DuringState(int time)
        {
            if (mAniUpdater != default)
            {
                if (ShouldUpdateMotion(time))
                {
                    bool flag = mAniUpdater.Update(this);
                    if (flag)
                    {
                        bool isCombo = mComboMotion != default;
                        if (mStateParamQueue.Count > 0)
                        {
                            StartFromNextParam(isCombo);
                        }
                        else
                        {
                            Finish(false);
                        }
                    }
                }
            }
        }

        protected virtual void StartFeedbackTime(int feedback, float time, float speed = 1f)
        {
            StateFeedback = feedback;
            mFeedbackTime.Start(time);
            Animator.SetFloat("Speed", speed);

            Debug.Log("StartFeedbackTime");
        }

        protected virtual bool ShouldUpdateMotion(int time)
        {
            return !FrameFrozen(time);
        }

        protected virtual bool FrameFrozen(int time)
        {
            bool result = StateFeedback >= RoleAnimationFeedBackConsts.FEED_BACK_BY_HIT;
            if (result)
            {
                if (mFeedbackTime.IsFinish)
                {
                    Debug.Log("FrameFrozen finish");
                    SpeedFrozenEnd();
                    mFeedbackTime.Reset();
                    StateFeedback = RoleAnimationFeedBackConsts.FEED_BACK_DEFAULT;
                    result = false;
                }
            }
            return result;
        }

        protected virtual void SpeedFrozenEnd()
        {
            Animator.SetFloat("Speed", 1f);
        }

        protected void InitMotion(bool isCombo, Animator animator)
        {
            if (isCombo)
            {
                mComboMotion = mSkillMapper.GetComboMotion(mCurrentSkillID, ref animator);
                mAniUpdater = mComboMotion.AniUpdater;
            }
            else
            {
                mAniUpdater = mSkillMapper.GetMotion(mCurrentSkillID, ref animator);
            }
        }

        protected virtual bool ReadyMotion(int skillID, SkillMotionsMapper mapper, bool isCombo)
        {
            Animator animator = Animator;

            bool isBeginning = mComboMotion == default && mAniUpdater == default;
            bool shouldCombo = mComboMotion != default && mComboMotion.ShouldCombo();

            bool result = isBeginning || shouldCombo;
            
            if (isBeginning)
            {
                mCurrentSkillID = skillID;
                mSkillMapper = mapper;

                InitMotion(isCombo, animator);//重新开始

                UpdaterNotice.AddSceneUpdater(mMethodUpdater);
            }
            else
            {
                if(shouldCombo)
                {
                    mComboMotion.StartComboMotion(ref animator);//开始下一个连续动画
                }
                else
                {
                    Finish(false);
                }
            }
            return result;
        }
        
        protected virtual bool BeforeFinish(bool checkInputWhenFinish)
        {
            return true;
        }

        protected virtual bool Finish(bool checkInputWhenFinish)
        {
            mFeedbackTime.Stop();

            bool result = BeforeFinish(checkInputWhenFinish);
            if (result)
            {
                RevertStateParam();
                RevertAllStateParams();
                Utils.Reclaim(ref mStateParamQueue, false);
                
                mComboMotion?.Reset(true);

                StateFeedback = RoleAnimationFeedBackConsts.FEED_BACK_DEFAULT;
                mStateParam = default;
                mComboMotion = default;
                mAniUpdater = default;
                mRole = default;

                UpdaterNotice.RemoveSceneUpdater(mMethodUpdater);
            }
            return result;
        }

        protected abstract void RevertAllStateParams();
        protected abstract void RevertStateParam();

        protected bool IsFeedbackChecked { get; set; }
        protected int StateFeedback { get; private set; }

    }
}