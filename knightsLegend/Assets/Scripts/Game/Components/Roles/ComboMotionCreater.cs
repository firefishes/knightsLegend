using ShipDock.Interfaces;
using ShipDock.Tools;
using System;
using UnityEngine;
using UnityEngine.Events;

namespace ShipDock.Applications
{
    [Serializable]
    public class MotionCompletionEvent : UnityEvent { }

    [Serializable]
    public class MotionSceneInfo : IDispose
    {
#if UNITY_EDITOR
        public string skillName;
#endif

        public bool isCombo;
        public int ID;
        public float checkComboTime;
        public int[] indexsForID;
        public MotionCompletionEvent motionCompletionEvent = new MotionCompletionEvent();

        public void Dispose()
        {
            Utils.Reclaim(Motion);
            Utils.Reclaim(ComboMotion);

            motionCompletionEvent?.RemoveAllListeners();
            ComboMotion = default;
            Motion = default;
            motionCompletionEvent = default;
            MotionSkillInfo = default;
        }
        
        public ComboMotionCreater ComboMotion { get; set; }
        public SkillInfo MotionSkillInfo { get; set; }
        public AnimationInfoUpdater Motion { get; set; }
    }

    public class ComboMotionCreater : IDispose
    {
        private int mCurrentCombo;
        private bool mHasComboInput;
        private bool mWillCacheCombo;
        private float mAllowComboTime;
        private ValueItem mValueItem;
        private AnimationInfoUpdater mAniUpdater;

        public ComboMotionCreater(int comboMax, ValueItem[] motionTriggerParam, ValueItem[] motionTransParam, Action onMotionCompletion = default)
        {
            ComboMotionMax = comboMax;

            MotionTransParam = motionTransParam;
            MotionTriggerParam = motionTriggerParam;
            MotionCompletion = onMotionCompletion;

            AllowComboInput();
            mAniUpdater = new AnimationInfoUpdater();
        }

        public void Dispose()
        {
            AllowComboInput();
            Utils.Reclaim(mAniUpdater);
            mAniUpdater = default;

            MotionCompletion = default;
            MotionCompletionEvent = default;
        }

        private void AllowComboInput()
        {
            mCurrentCombo = 0;
            mWillCacheCombo = false;
            ActiveComboCheck();
        }

        private void ActiveComboCheck(bool isActiveComboTime = true)
        {
            if(isActiveComboTime)
            {
                mAllowComboTime = 0f;
            }
            ShouldAddCombo = true;
            IsComboChecking = true;
        }

        public void SetCheckComboTime(float value)
        {
            CheckComboTime = value;
        }

        private void StartCheckCombo()
        {
            ActiveComboCheck();
        }

        public bool AddComboMotion(ref Animator animator)
        {
            if (!ShouldAddCombo)
            {
                return false;
            }
            if (IsComboChecking && ShouldCombo())
            {
                mHasComboInput = true;
                CheckAnimator(ref animator);
                return true;
            }
            return false;
        }

        public void CheckAnimator(ref Animator animator)
        {
            if (animator == default)
            {
                return;
            }
            if(mHasComboInput)
            {
                ResetAllowCombo();
                
                if(mCurrentCombo == 0)
                {
                    int max = MotionTriggerParam.Length;
                    for (int i = 0; i < max; i++)
                    {
                        mValueItem = MotionTriggerParam[i];
                        animator.SetBool(mValueItem.KeyField, mValueItem.Bool);
                    }
                    CreateCombo(ref animator, true);
                }
                else if(ShouldCombo())
                {
                    if (IsFastest)
                    {
                        CreateCombo(ref animator, false);
                    }
                    else
                    {
                        mWillCacheCombo = true;
                    }
                }
            }
            CountComboTime(ref animator);
            mHasComboInput = false;
        }

        public void CountComboTime(ref Animator animator)
        {
            if (ShouldAddCombo)
            {
                if (IsComboChecking && (mCurrentCombo > 0))
                {
                    mAllowComboTime += Time.deltaTime;
                    if (mAllowComboTime >= CheckComboTime && ShouldCombo())
                    {
                        ResetAllowCombo();
                        ComboFinish(ref animator);
                    }
                    else if(mAllowComboTime < CheckComboTime && mHasComboInput)
                    {
                        mWillCacheCombo = true;
                    }
                    else if (!ShouldCombo() && mAniUpdater.HasCompleted)
                    {
                        if (animator.GetCurrentAnimatorStateInfo(0).IsName("ResetAtk1"))
                        {
                            ComboFinish(ref animator);
                        }
                    }
                }
            }
        }

        private bool ShouldCombo()
        {
            return mCurrentCombo < ComboMotionMax;
        }

        private void CreateCombo(ref Animator animator, bool isFirstCreate)
        {
            mValueItem = MotionTransParam[mCurrentCombo];
            if (!isFirstCreate)
            {
                mAniUpdater.Dispose();
                mAniUpdater = new AnimationInfoUpdater();
            }

            mAniUpdater.Start(animator, 0f, MotionCompleted, mValueItem);
            if(ShouldCombo())
            {
                mAniUpdater.AddTimingCallback(CheckComboTime, StartCheckCombo);
                mCurrentCombo++;
            }
        }

        private void ResetAllowCombo()
        {
            mAllowComboTime = 0f;
            ShouldAddCombo = false;
            IsComboChecking = false;
        }

        private void ComboFinish(ref Animator animator)
        {
            mAniUpdater.Stop();
            mAniUpdater.ResetAllTiming();
            AllowComboInput();

            int max = MotionTriggerParam.Length;
            for (int i = 0; i < max; i++)
            {
                mValueItem = MotionTriggerParam[i];
                animator.SetBool(mValueItem.KeyField, !mValueItem.Bool);
            }
            max = MotionTransParam.Length;
            for (int i = 0; i < max; i++)
            {
                mValueItem = MotionTransParam[i];
                animator.SetFloat(mValueItem.KeyField, 0f);
            }
            MotionCompletion?.Invoke();
            MotionCompletionEvent?.Invoke();
        }
        
        private void MotionCompleted(Animator animator)
        {
            if (mWillCacheCombo)
            {
                CreateCombo(ref animator, false);
            }
            else
            {
                ActiveComboCheck(false);
            }
            mWillCacheCombo = false;
        }

        public void AddMotionCompletion(Action method)
        {
            MotionCompletion = method;
        }

        public bool IsMotionsFinish
        {
            get
            {
                return (mCurrentCombo == ComboMotionMax);
            }
        }

        public Action MotionCompletion { get; private set; }
        public MotionCompletionEvent MotionCompletionEvent { get; set; }
        public int ComboMotionMax { get; private set; }
        public ValueItem[] MotionTransParam { get; private set; }
        public ValueItem[] MotionTriggerParam { get; private set; }
        public bool ShouldAddCombo { get; private set; } = true;
        public bool IsComboChecking { get; private set; }
        public float CheckComboTime { get; private set; } = 1.0f;
        public bool IsFastest { get; set; }
        public int ID { get; set; }
    }
}