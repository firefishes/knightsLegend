using ShipDock.Tools;
using UnityEngine;

namespace ShipDock.FSM
{
    public class TriggerValueItem : ValueItem
    {
        public TriggerValueItem() : base(BOOL)
        {
            DampTime = -1f;
        }
    }

    /// <summary>
    /// 动画参数
    /// </summary>
    public class AnimatorParamer
    {
        private KeyValueList<string, ValueItem> mFieldMapper;

        /// <summary>即将播放的动画遮罩层索引</summary>
        public int AniMaskLayerWillPlay { get; private set; }
        /// <summary>动画所属的状态机状态</summary>
        public int MotionState { get; private set; }
        /// <summary>动画重复次数</summary>
        public int MotionRepeate { get; private set; } = 1;
        /// <summary>参数是否生效</summary>
        public bool IsValid { get; private set; }
        /// <summary>动画名</summary>
        public string MotionName { get; private set; }
        /// <summary>当前动画是否正在播放</summary>
        public bool IsMotionPlaying { get; private set; }
        /// <summary>动画开关</summary>
        public string MotionTrigger { get; set; }
        /// <summary>动画组件</summary>
        public Animator Owner { get; private set; }
        /// <summary>播放动画的起始时间</summary>
        public float NormalizedTime { get; private set; } = 0f;
        public bool IsSetWillPlay { get; private set; }

        public AnimatorParamer()
        {
            mFieldMapper = new KeyValueList<string, ValueItem>();
        }

        public void Dispose()
        {
            mFieldMapper?.Dispose();

            Owner = default;
        }

        public void SetAnimator(ref Animator animator)
        {
            Owner = animator;
        }

        public void Clear(bool isTrimExcess = false)
        {
            mFieldMapper?.Clear(isTrimExcess);
        }

        /// <summary>
        /// 设置浮点型动画参数
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        public void SetFloat(string paramName, float value)
        {
            bool isExist = mFieldMapper.IsContainsKey(paramName);

            ValueItem item = isExist ? mFieldMapper[paramName] : ValueItem.New(paramName, value);
            if (isExist)
            {
                item.Float = value;
            }
            else
            {
                mFieldMapper[paramName] = item;
                item.KeyField = paramName;
            }
        }

        /// <summary>
        /// 设置浮点型动画参数（附带抑制时间）
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="dampTime">抑制时间</param>
        public void SetFloat(string paramName, float value, float dampTime = 0f)
        {
            SetFloat(paramName, value);

            mFieldMapper[paramName].DampTime = dampTime;
        }

        /// <summary>
        /// 设置布尔型动画参数
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="value"></param>
        /// <param name="isTrigger"></param>
        public void SetBool(string paramName, bool value)
        {
            bool isExist = mFieldMapper.IsContainsKey(paramName);

            ValueItem item = isExist ? mFieldMapper[paramName] : ValueItem.New(paramName, value);
            if (isExist)
            {
                item.Bool = value;
            }
            else
            {
                mFieldMapper[paramName] = item;
                item.KeyField = paramName;
            }
        }

        /// <summary>
        /// 设置开关型动画参数
        /// </summary>
        /// <param name="paramName"></param>
        public void SetTrigger(string paramName)
        {
            bool isExist = mFieldMapper.IsContainsKey(paramName);

            TriggerValueItem item = isExist ? (TriggerValueItem)mFieldMapper[paramName] : new TriggerValueItem();
            if (isExist)
            {
                item.Bool = true;
            }
            else
            {
                mFieldMapper[paramName] = item;
                item.KeyField = paramName;
            }
        }

        /// <summary>
        /// 获取一个指定的浮点型动画参数
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public float GetFloat(string paramName)
        {
            return mFieldMapper.IsContainsKey(paramName) ? mFieldMapper[paramName].Float : 0f;
        }

        /// <summary>
        /// 提交动画参数
        /// </summary>
        public void CommitParamToAnimator()
        {
            if (!IsValid)
            {
                return;
            }
            ValueItem item;
            int max = mFieldMapper.Size;
            for (int i = 0; i < max; i++)
            {
                item = mFieldMapper.Values[i];
                if (item == default)
                {
                    continue;
                }
                switch (item.Type)
                {
                    case ValueItem.FLOAT:
                        Owner.SetFloat(item.KeyField, item.Float, item.DampTime > 0f ? item.DampTime : 0f, Time.deltaTime);
                        break;
                    case ValueItem.BOOL:
                        if (item.DampTime < 0f)
                        {
                            item.Bool = false;//开关型动画参数
                            Owner.SetTrigger(item.KeyField);
                        }
                        else
                        {
                            Owner.SetBool(item.KeyField, item.Bool);
                        }
                        break;
                }
            }
            IsValid = false;
        }

        /// <summary>
        /// 标记动画参数为生效状态
        /// </summary>
        public void Valid()
        {
            IsValid = true;
        }

        /// <summary>
        /// 判断动画是否完成
        /// </summary>
        /// <param name="animationName"></param>
        /// <param name="completionLen">判断动画完成的长度界限</param>
        /// <param name="layerIndex"></param>
        /// <returns></returns>
        public bool IsMotionCompleted(string animationName, float completionLen = 1f, int layerIndex = 0, bool isRewind = false)
        {
            AnimatorStateInfo info = GetStateInfo(layerIndex);
            bool flag = isRewind ? (info.normalizedTime <= 0f) : (info.normalizedTime >= completionLen);
            return IsMotion(animationName) && flag;
        }

        /// <summary>
        /// 判断当前动画名是否为参数给定的动画名
        /// </summary>
        /// <param name="animationName"></param>
        /// <param name="layerIndex"></param>
        /// <returns></returns>
        public bool IsMotion(string animationName, int layerIndex = 0)
        {
            AnimatorStateInfo info = GetStateInfo(layerIndex);
            return info.IsName(animationName);
        }

        public void SetMotionWillPlay(string motionName, int state, int aniMaskLayer = 0, float normalizedTime = 0f)
        {
            if (string.IsNullOrEmpty(motionName) || (state == int.MaxValue))
            {
                return;
            }
            AniMaskLayerWillPlay = aniMaskLayer;
            MotionName = motionName;
            MotionState = state;
            NormalizedTime = normalizedTime;

            IsSetWillPlay = true;
        }

        public void SetNormalizedTime(float value)
        {
            NormalizedTime = value;
        }

        public void SetMotionTrigger()
        {
            SetTrigger(MotionTrigger);
        }

        public void ResetMotionWillPlay()
        {
            MotionRepeate = 1;
            AniMaskLayerWillPlay = 0;
            MotionName = string.Empty;

            ResetMotionState();
            
            NormalizedTime = 0f;

            IsSetWillPlay = false;
        }

        public void ResetMotionState()
        {
            MotionState = int.MaxValue;
        }

        public bool CheckMotionPlaying(int layer = 0)
        {
            IsMotionPlaying = IsMotion(MotionName, layer);
            return IsMotionPlaying;
        }

        public void SetMotionCount(int motionCount)
        {
            MotionRepeate = motionCount;
        }

        public AnimatorStateInfo GetStateInfo(int layer = 0)
        {
            return Owner.GetCurrentAnimatorStateInfo(layer);
        }

    }
}