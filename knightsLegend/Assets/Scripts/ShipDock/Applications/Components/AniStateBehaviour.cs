#define _G_LOG

using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Tools;
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System;
using UnityEngine;

namespace ShipDock.Applications
{
    public class AniStateBehaviour : StateMachineBehaviour
    {
        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("是否应用程序状态机")]
#endif
        private bool m_ApplyFSM;

        [SerializeField]
#if ODIN_INSPECTOR
        [ShowIf("@this.m_ApplyFSM == true")]
#endif
        private int m_MotionCompleted;

        [SerializeField]
#if ODIN_INSPECTOR
        [ShowIf("@this.m_ApplyFSM == true")]
#endif
        private int m_MotionCompleteCheckMax = 1;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("动画状态参数"), Indent(1)]
#endif
        private AnimationSubgroup m_AniSubgroup = new AnimationSubgroup();

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("动画状态特效")]
#endif
        private AniStateFX[] m_AniStateEffects;

        private int mNoticeName;
        private string[] mParamName;
        private KeyValueList<string, int> mActivedEffectMapper;
        private IParamNotice<AniStateBehaviour> mNotice;

        public AnimationSubgroup Subgroup
        {
            get
            {
                return m_AniSubgroup;
            }
        }

        public AniStateFX[] AniStateFX
        {
            get
            {
                return m_AniStateEffects;
            }
        }

        public bool IsDuringState { get; private set; }

        public AniStateFX GetAniStateFX(string FXName)
        {
            AniStateFX result = default;

            if (mActivedEffectMapper != default && mActivedEffectMapper.ContainsKey(FXName))
            {
                int index = mActivedEffectMapper[FXName];
                result = m_AniStateEffects[index];
            }
            else { }

            return result;
        }

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            IsDuringState = true;

            mNoticeName = animator.GetInstanceID();
            mNotice = Pooling<ParamNotice<AniStateBehaviour>>.From();

            mNotice.ParamValue = this;

            if (m_ApplyFSM)
            {
                mParamName = m_AniSubgroup.ParamName;
            }
            else { }

            m_MotionCompleted = 0;

            BroadcastAniStateNotice();
            StateEffectsEntered();

        }

        private void StateEffectsEntered()
        {
            if (HasAniStateEffect())
            {
                bool isInitEffectMapper = mActivedEffectMapper == default;
                if (isInitEffectMapper)
                {
                    mActivedEffectMapper = new KeyValueList<string, int>();
                }
                else { }

                string name;
                AniStateFX item;
                int m = m_AniStateEffects.Length;
                for (int i = 0; i < m; i++)
                {
                    item = m_AniStateEffects[i];
                    item.Init();

                    if (isInitEffectMapper)
                    {
                        name = item.FXName;
                        mActivedEffectMapper[name] = i;
                    }
                    else { }
                }

                int max = m_AniStateEffects.Length;
                for (int i = 0; i < max; i++)
                {
                    m_AniStateEffects[i].UpdateFX();
                }
            }
            else { }
        }

        private bool HasAniStateEffect()
        {
            return m_AniStateEffects != default && m_AniStateEffects.Length > 0;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            bool isFXChecked = false;
            if (HasAniStateEffect())
            {
                bool flag = false;
                int FXFinished = 0;
                int max = m_AniStateEffects.Length;
                for (int i = 0; i < max; i++)
                {
                    m_AniStateEffects[i].UpdateFX();
                    flag = m_AniStateEffects[i].IsCreated();

                    if (flag)
                    {
                        FXFinished++;
                    }
                    else { }
                }

                isFXChecked = FXFinished >= max;

                if (!isFXChecked)
                {
                    return;
                }
                else { }
            }
            else { }

            IsDuringState = stateInfo.IsName(m_AniSubgroup.MotionName) && stateInfo.normalizedTime < 1f;

            if (!IsDuringState)
            {
                if (mNotice == default)
                {
                    IsDuringState = false;
                    return;
                }
                else { }

                if (m_MotionCompleteCheckMax > 0)
                {
                    m_MotionCompleted++;
                    if (m_MotionCompleted >= m_MotionCompleteCheckMax)
                    {
                        BroadcastAniStateNotice();
                    }
                    else { }
                }
                else
                {
                    BroadcastAniStateNotice();
                }

                ReleaseNotice();
            }
        }

        private void BroadcastAniStateNotice()
        {
            if (m_ApplyFSM && !IsDuringState)
            {
                "AniStateBehaviour finish".Log(m_AniSubgroup.StateName.ToString());
            }
            else { }

            mNoticeName.Broadcast(mNotice);
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            int max = m_AniStateEffects.Length;
            for (int i = 0; i < max; i++)
            {
                m_AniStateEffects[i].CheckFXAutoClean();
            }

            ReleaseNotice();
        }

        private void ReleaseNotice()
        {
            mNotice?.ToPool();
            mNotice = default;
        }

        // OnStateMove is called right after Animator.OnAnimatorMove()
        //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that processes and affects root motion
        //}

        // OnStateIK is called right after Animator.OnAnimatorIK()
        //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        //{
        //    // Implement code that sets up animation IK (inverse kinematics)
        //}

        public override void OnStateMachineExit(Animator animator, int stateMachinePathHash)
        {
            base.OnStateMachineExit(animator, stateMachinePathHash);
        }

        public void Clean()
        {
            mActivedEffectMapper?.Clear();
            mActivedEffectMapper = default;

            if (HasAniStateEffect())
            {
                int max = m_AniStateEffects.Length;
                for (int i = 0; i < max; i++)
                {
                    m_AniStateEffects[i].Release();
                }
                m_AniStateEffects = default;
            }
            else { }

            ReleaseNotice();
        }
    }

    [Serializable]
    public class AniStateFX
    {
        [SerializeField, Tooltip("用于识别同一个动画状态下的不同特效")]
#if ODIN_INSPECTOR
        [LabelText("特效名")]
#endif
        private string m_FXName;

        [SerializeField, Tooltip("是否启用此特效")]
#if ODIN_INSPECTOR
        [LabelText("启用")]
#endif
        private bool m_Enabled = true;

        [SerializeField, Tooltip("特效资源母本，用于复制实例")]
#if ODIN_INSPECTOR
        [LabelText("特效资源"), ShowIf("@!string.IsNullOrEmpty(this.m_FXName) && m_Enabled == true")]
#endif
        private GameObject m_FX;

        [SerializeField, Tooltip("在特效池中循环使用的特效实例总数")]
#if ODIN_INSPECTOR
        [LabelText("特效数量上限"), ShowIf("@!string.IsNullOrEmpty(this.m_FXName) && this.m_FX != null && m_Enabled == true")]
#endif
        private int m_FXTotal = 1;

        [SerializeField, Tooltip("延后启动的时间，用于创建需要投掷飞出的特效")]
#if ODIN_INSPECTOR
        [LabelText("延后创建时间"), ShowIf("@!string.IsNullOrEmpty(this.m_FXName) && this.m_FX != null && this.m_FXTotal > 0 && m_Enabled == true"), Indent(2)]
#endif
        private float m_StartTime;

        [SerializeField, Tooltip("计时完成后，特效自动回收到对象池，下次循环使用")]
#if ODIN_INSPECTOR
        [LabelText("回收时间"), ShowIf("@!string.IsNullOrEmpty(this.m_FXName) && this.m_FX != null && this.m_FXTotal > 0 && m_Enabled == true"), Indent(2)]
#endif
        private float m_CollectTime;

        [SerializeField, Tooltip("设置为已保存在角色身上的场景节点引用，用于特效跟随指定的角色部位")]
#if ODIN_INSPECTOR
        [LabelText("父级节点名"), ShowIf("@!string.IsNullOrEmpty(this.m_FXName) && this.m_FX != null && this.m_FXTotal > 0 && m_Enabled == true"), Indent(1)]
#endif
        private string m_ParentNodeName;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("启用坐标修正"), ShowIf("@!string.IsNullOrEmpty(this.m_FXName) && this.m_FX != null && this.m_FXTotal > 0 && m_Enabled == true"), Indent(1)]
#endif
        private bool m_LocalPositionOffsetEnabled;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText(""), ShowIf("@!string.IsNullOrEmpty(this.m_FXName) && this.m_FX != null && this.m_FXTotal > 0 && m_Enabled == true && m_LocalPositionOffsetEnabled == true"), Indent(2)]
#endif
        private Vector3 m_LocalPositionOffset;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("启用旋转修正"), ShowIf("@!string.IsNullOrEmpty(this.m_FXName) && this.m_FX != null && this.m_FXTotal > 0 && m_Enabled == true"), Indent(1)]
#endif
        private bool m_LocalRotationOffsetEnabled;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText(""), ShowIf("@!string.IsNullOrEmpty(this.m_FXName) && this.m_FX != null && this.m_FXTotal > 0 && m_Enabled == true && m_LocalRotationOffsetEnabled == true"), Indent(2)]
#endif
        private Vector3 m_LocalRotationOffset;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("启用縮放修正"), ShowIf("@!string.IsNullOrEmpty(this.m_FXName) && this.m_FX != null && this.m_FXTotal > 0 && m_Enabled == true"), Indent(1)]
#endif
        private bool m_ScaleOffsetEnabled;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText(""), ShowIf("@!string.IsNullOrEmpty(this.m_FXName) && this.m_FX != null && this.m_FXTotal > 0 && m_Enabled == true && m_ScaleOffsetEnabled == true"), Indent(2)]
#endif
        private Vector3 m_ScaleOffset = Vector3.one;

        [SerializeField, Tooltip("自身特效会在动画状态退出时自动停止")]
#if ODIN_INSPECTOR
        [LabelText("自身特效"), ShowIf("@!string.IsNullOrEmpty(this.m_FXName) && this.m_FX != null && this.m_FXTotal > 0 && m_Enabled == true"), Indent(1)]
#endif
        private bool m_IsFXSelf;

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("音效"), ShowIf("@!string.IsNullOrEmpty(this.m_FXName) && this.m_FX != null && this.m_FXTotal > 0 && m_Enabled == true"), Indent(1)]
#endif
        private string m_SoundFXName;

        [SerializeField, Tooltip("是否外部参数控制")]
#if ODIN_INSPECTOR
        [LabelText("参数模式控制"), Indent(1)]
#endif
        private bool m_HasParamControll;

        private bool mIsInited;
        private bool mIsReleased;
        private Effects mEffects;
        private TimeGapper mDelayStartTimer;
        private GameObject mFXInstance;
        private ParticalsCollecter mFXCollecter;

        public float CollectTime
        {
            get
            {
                return m_CollectTime;
            }
        }

        public string FXName
        {
            get
            {
                return m_FXName;
            }
        }

        public string ParentNode
        {
            get
            {
                return m_ParentNodeName;
            }
        }

        public string SoundFXName
        {
            get
            {
                return m_SoundFXName;
            }
        }

        public int PoolID { get; private set; } = int.MaxValue;
        public Action<AniStateFX, GameObject> OnFXCreated { get; set; }

        public void CheckFXAutoClean()
        {
            if (m_IsFXSelf)
            {
                mFXInstance?.SetActive(false);
            }
            else { }

            mFXInstance = default;
        }

        public void Init()
        {
            if (m_Enabled)
            {
                if (mIsInited)
                {
                    mDelayStartTimer.totalTime = m_StartTime;
                }
                else
                {
                    mDelayStartTimer = new TimeGapper
                    {
                        totalTime = m_StartTime,
                        isAnew = true,
                    };

                    if (m_FX != default)
                    {
                        mEffects = ShipDockApp.Instance.Effects;
                        PoolID = m_FX.GetInstanceID();
                        Debug.Log("Pool id is " + PoolID);
                        mEffects?.CreateSource(PoolID, ref m_FX, m_FXTotal);
                    }
                    else { }

                    mIsInited = true;
                }

                mDelayStartTimer.Reset();
                mDelayStartTimer.Start();
            }
            else { }
        }

        public void Release()
        {
            mIsReleased = true;

            if (int.MaxValue != PoolID)
            {
                mEffects?.RemoveEffectRaw(PoolID);
                mEffects = default;
            }
            else { }

            m_FX = default;
            OnFXCreated = default;
            mFXInstance = default;
        }

        public bool IsCreated()
        {
            return mFXInstance != default;
        }

        public void UpdateFX()
        {
            if (!mIsReleased && m_Enabled)
            {
                if (mFXInstance == default)
                {
                    if (mDelayStartTimer.totalTime > 0f)
                    {
                        if (!mDelayStartTimer.IsFinised)
                        {
                            bool flag = mDelayStartTimer.TimeAdvanced(Time.deltaTime);
                            if (flag)
                            {
                                CreateFX();//延后创建计时完毕，创建特效对象
                            }
                            else { }
                        }
                        else { }
                    }
                    else
                    {
                        CreateFX();//无延时，直接创建特效对象
                    }
                }
                else { }
            }
            else { }
        }

        private void CreateFX()
        {
            mEffects?.CreateEffect(PoolID, out mFXInstance, true, false);

            mFXInstance?.SetActive(!m_HasParamControll);

            AfterFXCreated();
        }

        private void AfterFXCreated()
        {
            if (mFXInstance != default)
            {
                if (m_CollectTime > 0f)
                {
                    mFXCollecter = mFXInstance.GetComponent<ParticalsCollecter>();
                    mFXCollecter?.SetCollectTime(m_CollectTime);
                }
                else { }

                OnFXCreated?.Invoke(this, mFXInstance);
            }
            else { }
        }

        public void ApplyOffset(GameObject target)
        {
            Transform trans = target.transform;
            if (m_LocalPositionOffsetEnabled)
            {
                trans.localPosition = m_LocalPositionOffset;
            }
            else { }

            if (m_LocalRotationOffsetEnabled)
            {
                trans.localRotation = Quaternion.Euler(m_LocalRotationOffset);
            }
            else { }

            if (m_ScaleOffsetEnabled)
            {
                trans.localScale = m_ScaleOffset;
            }
            else { }
        }
    }
}