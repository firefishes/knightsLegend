#define _G_LOG

using ShipDock.FSM;
using ShipDock.Interfaces;
using ShipDock.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{

    public class AnimationInfoUpdater : IDispose
    {
        private ValueItem mConfItem;
        private ValueItem[] mParamsNew;
        private List<ValueItem> mParamSet;
        private List<ValueItem> mParamDamp;

        public AnimationInfoUpdater(bool autoDispose = false)
        {
            HasCompleted = true;
            AutoDispose = autoDispose;
            mParamDamp = new List<ValueItem>();
            mParamSet = new List<ValueItem>();
        }

        public void Dispose()
        {
            AnimatorTarget = default;
        }

        public void Stop()
        {
            Utils.Reclaim(ref mParamsNew, false);
            Utils.Reclaim(ref mParamDamp, false);
            Utils.Reclaim(ref mParamSet, false);
        }
        
        public void Start(Animator animator, params ValueItem[] paramConfs)
        {
            HasCompleted = false;
            AnimatorTarget = animator;

            if(AnimatorTarget == default)
            {
                return;
            }

            Frame = 0;
            mParamsNew = paramConfs;
            int max = mParamsNew.Length;
            for (int i = 0; i < max; i++)
            {
                ValueItem item = mParamsNew[i];
                if (item.DampTime > 0f)
                {
                    mParamDamp.Add(item);
                }
                else
                {
                    mParamSet.Add(item);
                }
            }
            RefreshParams(ref mParamSet);
            Length = StateInfo.normalizedTime;
        }

        public bool Update(IAnimatorState state)
        {
            Frame++;
            Testers.Tester.Instance.Log(TesterRPG.Instance, TesterRPG.LOG, string.IsNullOrEmpty(state.AnimationName), "error: ".Append(state.ToString(), " animation name is null."));
            HasCompleted = (StateInfo.normalizedTime > 1f) && StateInfo.IsName(state.AnimationName);
            return HasCompleted;
        }

        private void RefreshParams(ref List<ValueItem> paramConfs, bool checkDampTime = false)
        {
            if(paramConfs != default)
            {
                int max = paramConfs.Count;
                for (int i = 0; i < max; i++)
                {
                    mConfItem = paramConfs[i];
                    RefreshParamItem(ref mConfItem, ref checkDampTime);
                }
            }
        }

        private void RefreshParamItem(ref ValueItem conf, ref bool checkDampTime)
        {
            if (conf.IsBool)
            {
                AnimatorTarget.SetBool(conf.KeyField, conf.Bool);
            }
            else if(conf.IsFloat)
            {
                if (checkDampTime && conf.DampTime > 0f)
                {
                    AnimatorTarget.SetFloat(conf.KeyField, conf.Float, conf.DampTime, DeltaTime);
                }
                else
                {
                    AnimatorTarget.SetFloat(conf.KeyField, conf.Float);
                    Testers.Tester.Instance.Log(TesterRPG.Instance, TesterRPG.LOG, "log: ".Append(mConfItem.KeyField, " = ", mConfItem.Value));
                }
            }
            else if (conf.IsInt)
            {
                AnimatorTarget.SetInteger(conf.KeyField, conf.Int);
            }
        }

        public AnimatorStateInfo StateInfo
        {
            get
            {
                return AnimatorTarget.GetCurrentAnimatorStateInfo(0);
            }
        }

        public float NormalizedTime
        {
            get
            {
                return StateInfo.normalizedTime;
            }
        }

        public Animator AnimatorTarget { get; private set; }
        public bool HasCompleted { get; private set; }
        public bool AutoDispose { get; private set; }
        public bool AutoStop { get; set; }
        public float Length { get; private set; }
        public float DeltaTime { get; set; }
        public int Frame { get; private set; }
    }
}