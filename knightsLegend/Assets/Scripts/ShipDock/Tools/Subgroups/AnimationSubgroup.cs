using System;
using UnityEngine;

namespace SGGame
{
    [Serializable]
    public class AnimationSubgroup
    {
        [SerializeField]
        private int m_StateName;
        [SerializeField]
        private string[] m_ParamName;
        [SerializeField]
        private string m_MotionName;
        
        public int StateName
        {
            get
            {
                return m_StateName;
            }
        }

        public string[] ParamName
        {
            get
            {
                return m_ParamName;
            }
        }

        public string MotionName
        {
            get
            {
                return m_MotionName;
            }
        }
    }

}