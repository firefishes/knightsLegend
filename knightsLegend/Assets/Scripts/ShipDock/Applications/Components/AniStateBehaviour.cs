#define _G_LOG

using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Tools;
using UnityEngine;

namespace ShipDock.Applications
{
    public class AniStateBehaviour : StateMachineBehaviour
    {
        [SerializeField]
        private AnimationSubgroup m_AniSubgroup = new AnimationSubgroup();
        [SerializeField]
        private int m_MotionCompleted;
        [SerializeField]
        private int m_MotionCompleteCheckMax = 1;

        private int mNoticeName;
        private string[] mParamName;
        private IParamNotice<AnimationSubgroup> mNotice;

        // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
        override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mNoticeName = animator.GetInstanceID();
            mNotice = Pooling<ParamNotice<AnimationSubgroup>>.From();
            mParamName = m_AniSubgroup.ParamName;

            m_MotionCompleted = 0;
        }

        // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
        override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if (stateInfo.IsName(m_AniSubgroup.MotionName) && stateInfo.normalizedTime > 1f)
            {
                if (mNotice == default)
                {
                    return;
                }

                if (m_MotionCompleteCheckMax > 0)
                {
                    m_MotionCompleted++;
                    if(m_MotionCompleted >= m_MotionCompleteCheckMax)
                    {
                        AniStatePlayFinish();
                    }
                }
                else
                {
                    AniStatePlayFinish();
                }
            }
        }

        private void AniStatePlayFinish()
        {
            "AniStateBehaviour finish".Log(m_AniSubgroup.StateName.ToString());

            mNotice.ParamValue = m_AniSubgroup;
            mNoticeName.Broadcast(mNotice);

            mNotice?.ToPool();
            mNotice = default;
        }

        // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
        override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            mNotice?.ToPool();
            mNotice = default;

            //int max = mParamName.Length;
            //for (int i = 0; i < max; i++)
            //{
            //    animator.SetFloat(mParamName[i], 0f);
            //}
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
    }
}