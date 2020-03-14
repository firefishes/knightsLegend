using ShipDock.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{

    public class AnimationInfoUpdater : MethodUpdater
    {
        private int mMax;
        private int mFrame;
        private ValueItem mConfItem;
        private ValueItem[] mParamsNew;
        private KeyValueList<int, TimingCallbacker> mTimings;

        public AnimationInfoUpdater(bool autoDispose = false)
        {
            HasCompleted = true;
            AutoDispose = autoDispose;
            mTimings = new KeyValueList<int, TimingCallbacker>();
        }

        public override void Dispose()
        {
            base.Dispose();

            ResetAllTiming();
            Stop();

            Utils.Reclaim(ref mTimings, true, true);
            Completion = default;
            AnimatorTarget = default;
            ShortNameHash = 0;
        }

        public void ResetAllTiming()
        {
            Utils.Reclaim(ref mTimings, false, true);
        }
        
        public TimingCallbacker AddTimingCallback(float time, Action method)
        {
            TimingCallbacker callbacker;

            int index = Mathf.CeilToInt(Length * 0.8f);
            //Debug.Log("f " + index);
            if (mTimings.ContainsKey(index))
            {
                callbacker = mTimings[index];
            }
            else
            {
                callbacker = new TimingCallbacker
                {
                    timing = time
                };
                mTimings[index] = callbacker;
            }
            callbacker.callback += method;
            return callbacker;
        }

        public void Start(Animator animator, float lengthOffset = 0, params ValueItem[] paramConfs)
        {
            Start(animator, lengthOffset, default, paramConfs);
        }
        
        public void Start(Animator animator, float lengthOffset = 0, Action<Animator> method = default, params ValueItem[] paramConfs)
        {
            HasCompleted = false;
            AnimatorTarget = animator;

            if(AnimatorTarget == default)
            {
                Stop();
                return;
            }

            mFrame = 0;
            mParamsNew = paramConfs;
            RefreshParams(ref mParamsNew);

            AnimatorClipInfo[] clips = AnimatorTarget.GetCurrentAnimatorClipInfo(0);
            ClipInfo = (clips.Length > 0) ? clips[0] : default;
            Length = (clips.Length > 0) ? ClipInfo.weight : 1f;//clip.ofs; //StateInfo.length - lengthOffset;
            Length -= lengthOffset;

            ShortNameHash = StateInfo.shortNameHash;
            Completion = method;

            UpdaterNotice.AddSceneUpdater(this);
            OnUpdate(0);
        }

        private void RefreshParams(ref ValueItem[] paramConfs, bool checkDampTime = false, bool isReset = false)
        {
            if(paramConfs != default)
            {
                mMax = paramConfs.Length;
                for (int i = 0; i < mMax; i++)
                {
                    mConfItem = paramConfs[i];
                    RefreshParamItem(ref mConfItem, ref checkDampTime, ref isReset);
                }
            }
        }

        private void RefreshParamItem(ref ValueItem conf, ref bool checkDampTime, ref bool isReset)
        {
            if (conf.IsBool)
            {
                AnimatorTarget.SetBool(conf.KeyField, conf.Bool);
            }
            else if(conf.IsFloat)
            {
                if (checkDampTime && !isReset && conf.DampTime > 0f)
                {
                    AnimatorTarget.SetFloat(conf.KeyField, conf.Float, conf.DampTime, Time.deltaTime);
                }
                else
                {
                    AnimatorTarget.SetFloat(conf.KeyField, isReset ? 0f : conf.Float);
                }
            }
            else if (conf.IsInt)
            {
                AnimatorTarget.SetInteger(conf.KeyField, conf.Int);
            }
        }

        public void Stop()
        {
            mFrame = 0;
            UpdaterNotice.RemoveSceneUpdater(this);
        }

        public override void OnUpdate(int dTime)
        {
            base.OnUpdate(dTime);

            Length -= Time.deltaTime;
            if (Length <= 0f)
            {
                Completion?.Invoke(AnimatorTarget);
                //if (mParamsNew != default)
                //    Debug.Log(mParamsNew[0].Float + " set completion true");
                HasCompleted = true;

                if (AutoDispose)
                {
                    Dispose();
                }
                else
                {
                    Stop();
                }
            }
            else
            {
                if(mTimings != default && mTimings.ContainsKey(mFrame))
                {
                    //Debug.Log("frame " + mFrame);
                    mTimings[mFrame].callback?.Invoke();
                }
            }
            mFrame++;
        }

        public AnimatorStateInfo StateInfo
        {
            get
            {
                return AnimatorTarget.GetCurrentAnimatorStateInfo(0);
            }
        }

        public AnimatorClipInfo ClipInfo { get; private set; }
        public Action<Animator> Completion { get; set; }
        public Animator AnimatorTarget { get; private set; }
        public int ShortNameHash { get; private set; }
        public float Length { get; private set; }
        public bool HasCompleted { get; private set; }
        public bool AutoDispose { get; private set; }
        public bool AutoStop { get; set; }
    }
}