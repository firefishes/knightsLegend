using ShipDock.Interfaces;
using ShipDock.Tools;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{

    public class AnimatorParamer : KeyValueList<string, ValueItem>
    {
        private Queue<ValueItem> mParamValues;

        public AnimatorParamer()
        {
            mParamValues = new Queue<ValueItem>();
        }

        public override void Dispose()
        {
            base.Dispose();

            Animator = default;

            Utils.Reclaim(ref mParamValues, true, true);
        }

        public void SetAnimator(ref Animator animator)
        {
            Animator = animator;
        }

        public override void Clear(bool isTrimExcess = false)
        {
            base.Clear(isTrimExcess);
            
            Utils.Reclaim(ref mParamValues, false, true);
        }

        public void SetFloat(string paramName, float value)
        {
            bool isExist = IsContainsKey(paramName);

            ValueItem item = isExist ? this[paramName] : ValueItem.New(paramName, value);
            if (!isExist)
            {
                item.KeyField = paramName;
                this[paramName] = item;
            }
            mParamValues.Enqueue(item);

        }

        public void SetFloat(string paramName, float value, float dampTime = 0f)
        {
            SetFloat(paramName, value);

            this[paramName].DampTime = dampTime;
        }

        public float GetFloat(string paramName)
        {
            return IsContainsKey(paramName) ? this[paramName].Float : 0f;
        }

        public void CommitParamToAnimator()
        {
            ValueItem item;
            while (mParamValues.Count > 0)
            {
                item = mParamValues.Dequeue();
                switch (item.Type)
                {
                    case ValueItem.FLOAT:
                        Animator.SetFloat(item.KeyField, item.Float, item.DampTime > 0f ? item.DampTime : 0f, Time.deltaTime);
                        break;
                }
            }
            IsValid = false;
        }

        public void Valid()
        {
            IsValid = true;
        }

        public bool IsMotionCompleted(string animationName)
        {
            return (StateInfo.normalizedTime > 1f) && StateInfo.IsName(animationName);
        }

        public bool IsMotion(string animationName)
        {
            return StateInfo.IsName(animationName);
        }
        
        public AnimatorStateInfo StateInfo
        {
            get
            {
                return Animator.GetCurrentAnimatorStateInfo(0);
            }
        }

        public bool IsValid { get; private set; }
        public Animator Animator { get; private set; }
    }
}