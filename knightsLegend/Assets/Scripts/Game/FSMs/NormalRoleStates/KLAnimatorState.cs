using System.Collections.Generic;
using ShipDock.Applications;
using ShipDock.FSM;
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
        protected TimeGapper mDelayFinishTime = new TimeGapper();

        private TimeGapper mFeedbackTime = new TimeGapper();
        private MethodUpdater mMethodUpdater;

        public KLAnimatorState(int name) : base(name)
        {
            mMethodUpdater = new MethodUpdater()
            {
                FixedUpdate = DuringState
            };
            mStateParamQueue = new Queue<P>();
        }

        public override void InitState(IStateParam param = null)
        {
            base.InitState(param);

            StateFeedback = -1;

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
            mDelayFinishTime.Stop();

            bool result = ShouldParamEnqueue(ref param);
            if (result)
            {
                mStateParamQueue.Enqueue(param);
            }
            else
            {
                if(mComboMotion != default && mComboMotion.ShouldCombo())
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
                            RevertStateParam();
                            mStateParam = mStateParamQueue.Dequeue();
                            ReadyMotion(mCurrentSkillID, mSkillMapper, isCombo);
                        }
                        else if (!mDelayFinishTime.isStart && isCombo)
                        {
                            Finish(true);
                        }
                    }
                }
            }
            OnDelayFinish(time);
        }

        protected void StartFeedbackTime(int feedback, float time, float speed = 1f)
        {
            StateFeedback = feedback;
            mFeedbackTime.Start(time);
            Animator.SetFloat("Speed", speed);
        }

        protected virtual bool ShouldUpdateMotion(int time)
        {
            return !FrameFrozen(time);
        }

        protected virtual bool FrameFrozen(int time)
        {
            bool result = StateFeedback >= 0;
            if (result)
            {
                if (mFeedbackTime.isStart)
                {
                    if (mFeedbackTime.TimeAdvanced(time * 0.001f))
                    {
                        SpeedFrozenEnd();
                        StateFeedback = -1;
                        result = false;
                    }
                }
            }
            return result;
        }

        protected virtual void SpeedFrozenEnd()
        {
            Animator.SetFloat("Speed", 1f);
        }

        private void OnDelayFinish(int time)
        {
            if (mDelayFinishTime.isStart)
            {
                if (mDelayFinishTime.TimeAdvanced(time * 0.001f))
                {
                    Finish(false);
                }
            }
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
            bool shouldFinish = true;
            if(mComboMotion != default)
            {
                if (checkInputWhenFinish && mComboMotion.ShouldCombo())//连续动画模式
                {
                    IsFeedbackChecked = false;
                    mDelayFinishTime.Start(0.5f);
                    shouldFinish = false;
                }
            }
            else
            {
                if (mAniUpdater != default && mStateParamQueue.Count > 0)//单一动画循环模式
                {
                    RevertStateParam();
                    mStateParam = mStateParamQueue.Dequeue();
                    shouldFinish = false;
                }
            }
            return shouldFinish;
        }

        protected virtual bool Finish(bool checkInputWhenFinish)
        {
            mFeedbackTime.Stop();
            mDelayFinishTime.Stop();

            bool result = BeforeFinish(checkInputWhenFinish);
            if (result)
            {
                RevertStateParam();
                RevertAllStateParams();
                Utils.Reclaim(ref mStateParamQueue, false);

                mComboMotion?.Reset(true);

                StateFeedback = -1;
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