using System;
using UnityEngine;

namespace ShipDock.FSM
{
    public class AnimatorState : FState, IAnimatorState
    {
        protected float mTotalNormalizedTime;
        protected string mAnimationName;
        protected AnimatorStateMachine mFSM;
        protected ParamerFields mParamerFields;

        private string mMotionNameTemp;

        public virtual string AnimationName
        {
            get
            {
                return mAnimationName;
            }
            set
            {
                mAnimationName = value;
            }
        }

        public bool ShouldPlay
        {
            get
            {
                return !string.IsNullOrEmpty(mAnimationName);
            }
        }

        public Animator Animator { get; private set; }
        public int MotionRepeates { get; protected set; }
        public int LayerIndex { get; set; }
        public bool IsAniPlaying { get; protected set; }
        public bool IsRewind { get; protected set; }
        public bool RevertParamsAfterRewind { get; protected set; } = true;
        public Action OnAniCompleted { get; set; }

        public AnimatorState(int name) : base(name) { }

        public override void Dispose()
        {
            base.Dispose();

            OnAniCompleted = default;
            Animator = null;
            mAnimationName = string.Empty;
        }

        public override void SetStateParam(IStateParam param)
        {
            base.SetStateParam(param);

            CheckStateAniDuringSetStateParam(ref param);
        }

        private void CheckFSMNotEmpty()
        {
            if (mFSM == default)
            {
                mFSM = GetFSM() as AnimatorStateMachine;
            }
        }

        public void CheckStateAniDuringSetStateParam(ref IStateParam param)
        {
            CheckFSMNotEmpty();
            CheckStateAnimation(mFSM.AniParamer);
        }

        public void CheckStateAnimation(AnimatorParamer paramer)
        {
            if (ShouldPlay && Animator != default)
            {
                if (!IsAniPlaying)
                {
                    IsAniPlaying = true;
                    if (MotionRepeates > 0)
                    {
                        mTotalNormalizedTime = MotionRepeates * 1f;
                        paramer.SetMotionCount(MotionRepeates);
                    }
                    else
                    {
                        paramer.SetMotionCount(1);
                    }

                    if (!paramer.IsSetWillPlay)
                    {
                        paramer.SetMotionWillPlay(AnimationName, StateName, LayerIndex);//同步状态数据到参数器中
                    }

                    PlayMotion(ref paramer);
                }
            }
        }

        protected virtual void PlayMotion(ref AnimatorParamer paramer)
        {
            if (mParamerFields == default)
            {
                mParamerFields = new ParamerFields();
            }
            if (IsRewind)//是否倒播
            {
                paramer.SetNormalizedTime(1f);

                mParamerFields.speedRevert = Animator.GetFloat(mParamerFields.speedField);
                Animator.SetFloat(mParamerFields.speedField, -1f);
            }
            
            mMotionNameTemp = paramer.MotionName;//设置动画名
            int layerIndex = paramer.AniMaskLayerWillPlay;//设置遮罩层索引
            float normalizedTime = paramer.NormalizedTime;//设置播放起点的标准化时间

            Animator.Play(mMotionNameTemp, layerIndex, normalizedTime);//播放动画
        }

        public virtual void SetAnimator(ref Animator target)
        {
            Animator = target;
        }

        public void CheckMotionComplete(AnimatorParamer paramer)
        {
            if (IsAniPlaying)
            {
                CheckFSMNotEmpty();

                if (MotionRepeates > 0)
                {
                    MotionCompletionRepeatable(ref paramer);
                }
                else
                {
                    if (IsAnimationComplete(ref paramer))
                    {
                        IsAniPlaying = false;
                    }
                }
                if (!IsAniPlaying)
                {
                    mTotalNormalizedTime = 0f;
                    AfterMotionCompleted();
                    OnAniCompleted?.Invoke();

                    paramer.ResetMotionState();
                    paramer.ResetMotionWillPlay();
                }
            }
        }

        private void MotionCompletionRepeatable(ref AnimatorParamer paramer)
        {
            bool isCompleted = IsAnimationComplete(ref paramer);
            if (isCompleted)
            {
                mTotalNormalizedTime = MotionRepeates * 1f - paramer.GetStateInfo(LayerIndex).normalizedTime;
                if (isCompleted && mTotalNormalizedTime <= 0f)
                {
                    IsAniPlaying = false;
                }
            }
        }

        protected virtual void AfterMotionCompleted()
        {
            if (IsRewind)
            {
                if (RevertParamsAfterRewind)
                {
                    Animator.SetFloat(mParamerFields.speedField, mParamerFields.speedRevert);
                }
            }
        }

        protected virtual bool IsAnimationComplete(ref AnimatorParamer paramer)
        {
            return paramer.IsMotionCompleted(AnimationName, 1f, LayerIndex, IsRewind);
        }
    }
}