using ShipDock.Applications;
using ShipDock.Interfaces;
using ShipDock.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{

    public struct MotionUnit
    {
        public int index;
    }

    public class ComboMotionCreater : IDispose
    {
        private int mCurrentCombo;
        private Queue<MotionUnit> mMotionComboQueue;
        private AnimationInfoUpdater mAniUpdater;

        public ComboMotionCreater(int comboMax, string motionTransParam, string motionTriggerParam, Action onMotionCompletion = default)
        {
            ComboMotionMax = comboMax;

            mMotionComboQueue = new Queue<MotionUnit>();
            MotionTransParam = motionTransParam;
            MotionTriggerParam = motionTriggerParam;
            MotionCompletion = onMotionCompletion;
        }

        public void Dispose()
        {
            mCurrentCombo = 0;
            mAniUpdater = default;
            Utils.Reclaim(ref mMotionComboQueue);
        }

        public bool AddComboMotion(ref Animator animator)
        {
            if (mMotionComboQueue.Count < ComboMotionMax)
            {
                MotionUnit item = new MotionUnit()
                {
                    index = mCurrentCombo + 1
                };
                mMotionComboQueue.Enqueue(item);
                animator.SetFloat(MotionTransParam, mCurrentCombo + 1);

                if (mCurrentCombo < (ComboMotionMax - 1))
                {
                    mCurrentCombo++;
                }
                mAniUpdater.Reset(animator, MotionCompleted);
                UpdaterNotice.AddSceneUpdater(mAniUpdater);
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
            if (mAniUpdater == default)
            {
                mAniUpdater = new AnimationInfoUpdater();
            }

            if (mAniUpdater.HasCompleted)
            {
                if (mMotionComboQueue.Count > 0)
                {
                    mMotionComboQueue.Dequeue();
                }
            }
        }

        private void MotionCompleted(Animator animator)
        {
            if (mMotionComboQueue.Count == 0)
            {
                mCurrentCombo = 0;
                animator.SetFloat(MotionTransParam, 0);
                animator.SetBool(MotionTriggerParam, false);
                UpdaterNotice.RemoveSceneUpdater(mAniUpdater);
            }
        }

        public bool IsMotonsFinish
        {
            get
            {
                return (mCurrentCombo == ComboMotionMax - 1) || (mMotionComboQueue.Count == 0);
            }
        }

        public Action MotionCompletion { get; private set; }
        public int ComboMotionMax { get; private set; }
        public string MotionTransParam { get; private set; }
        public string MotionTriggerParam { get; private set; }
    }
}