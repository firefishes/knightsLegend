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

        public void Dispose()
        {
            Utils.Reclaim(Motion);
            Utils.Reclaim(ComboMotion);
            
            ComboMotion = default;
            Motion = default;
            MotionSkillInfo = default;
        }
        
        public ComboMotionCreater ComboMotion { get; set; }
        public SkillInfo MotionSkillInfo { get; set; }
        public AnimationInfoUpdater Motion { get; set; }
    }

    public class ComboMotionCreater : IDispose
    {
        private ValueItem mValueItem;
        private ValueItem mValueItemForRevert;

        public ComboMotionCreater(int comboMax, ValueItem[] motionTriggerParam, ValueItem[] motionTransParam, Action onMotionCompletion = default)
        {
            ComboMotionMax = comboMax;

            MotionTransParam = motionTransParam;
            MotionTriggerParam = motionTriggerParam;
            
            AniUpdater = new AnimationInfoUpdater();
            mValueItemForRevert = ValueItem.New(string.Empty);
        }

        public void Dispose()
        {
            Reset(true);

            Utils.Reclaim(AniUpdater);
            AniUpdater = default;
            mValueItem = default;
        }

        public void StartComboMotion(ref Animator animator)
        {
            if (CurrentCombo == 0)
            {
                int max = MotionTriggerParam.Length;
                for (int i = 0; i < max; i++)
                {
                    mValueItem = MotionTriggerParam[i];
                    if (!string.IsNullOrEmpty(mValueItem.KeyField))
                    {
                        animator.SetBool(mValueItem.KeyField, mValueItem.Bool);
                    }
                }
                var item = MotionTransParam[0];
                string keyField = item.KeyField;
                mValueItemForRevert.KeyField = keyField;
                if (item.IsInt)
                {
                    mValueItemForRevert.Int = animator.GetInteger(keyField);
                }
                else if (item.IsFloat)
                {
                    mValueItemForRevert.Float = animator.GetFloat(keyField);
                }
            }
            CreateCombo(ref animator, true);
        }

        private void CreateCombo(ref Animator animator, bool isFirstCreate)
        {
            mValueItem = MotionTransParam[CurrentCombo];
            if (!isFirstCreate)
            {
                AniUpdater.Dispose();
                AniUpdater = new AnimationInfoUpdater();
            }

            AniUpdater.Start(animator, mValueItem);
            CurrentCombo++;
        }

        public void Reset(bool revertCombo)
        {
            if(revertCombo)
            {
                CurrentCombo = 0;
            }
            int max = MotionTriggerParam.Length;
            for (int i = 0; i < max; i++)
            {
                mValueItem = MotionTriggerParam[i];
                if (!string.IsNullOrEmpty(mValueItem.KeyField))
                {
                    AniUpdater.AnimatorTarget.SetBool(mValueItem.KeyField, !mValueItem.Bool);
                }
            }
            
            string keyField = mValueItemForRevert.KeyField;
            if (mValueItemForRevert.IsInt)
            {
                AniUpdater.AnimatorTarget.SetInteger(keyField, mValueItemForRevert.Int);
            }
            else if (mValueItemForRevert.IsFloat)
            {
                AniUpdater.AnimatorTarget.SetFloat(keyField, mValueItemForRevert.Float);
            }

            Stop();
        }

        public void Stop()
        {
            AniUpdater.Stop();
        }

        public bool ShouldCombo()
        {
            return CurrentCombo < ComboMotionMax;
        }

        public int ID { get; set; }
        public int CurrentCombo { get; private set; }
        public int ComboMotionMax { get; private set; }
        public ValueItem[] MotionTransParam { get; private set; }
        public ValueItem[] MotionTriggerParam { get; private set; }
        public AnimationInfoUpdater AniUpdater { get; private set; }
    }
}