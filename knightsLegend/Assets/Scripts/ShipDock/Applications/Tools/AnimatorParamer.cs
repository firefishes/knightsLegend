using ShipDock.Tools;
using UnityEngine;

namespace ShipDock.Applications
{

    public class AnimatorParamer : KeyValueList<string, ValueItem>
    {
        public AnimatorParamer()
        {
        }

        public override void Dispose()
        {
            base.Dispose();

            Animator = default;
        }

        public void SetAnimator(ref Animator animator)
        {
            Animator = animator;
        }

        public override void Clear(bool isTrimExcess = false)
        {
            base.Clear(isTrimExcess);
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
            else
            {
                item.Float = value;
            }
        }

        internal void SetBool(string paramName, bool value)
        {
            bool isExist = IsContainsKey(paramName);

            ValueItem item = isExist ? this[paramName] : ValueItem.New(paramName, value);
            if (isExist)
            {
                item.KeyField = paramName;
                this[paramName] = item;
            }
            else
            {
                item.Bool = value;
            }
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
            int max = Size;
            for (int i = 0; i < max; i++)
            {
                item = Values[i];
                if(item == default)
                {
                    continue;
                }
                switch (item.Type)
                {
                    case ValueItem.FLOAT:
                        Animator.SetFloat(item.KeyField, item.Float, item.DampTime > 0f ? item.DampTime : 0f, Time.deltaTime);
                        break;
                    case ValueItem.BOOL:
                        Animator.SetBool(item.KeyField, item.Bool);
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
            return IsMotion(animationName) && (StateInfo.normalizedTime > 1f);
        }

        public bool IsMotion(string animationName)
        {
            return StateInfo.IsName(animationName);
        }

        public void ConfirmPlayingMotion(string motionName, int aniMaskLayer = 0)
        {
            AniMaskLayer = aniMaskLayer;
            MotionName = motionName;
        }

        public void SetCurrentMotion(string motionName)
        {
            CurrentMotion = motionName;
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
        public string MotionName { get; private set; }
        public int AniMaskLayer { get; private set; }
        public string CurrentMotion { get; private set; }
    }
}