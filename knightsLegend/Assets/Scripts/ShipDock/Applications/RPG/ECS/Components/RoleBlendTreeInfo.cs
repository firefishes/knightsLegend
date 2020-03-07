using System;
using UnityEngine;

namespace ShipDock.Applications
{

    [Serializable]
    public class RoleBlendTreeInfo
    {
        [SerializeField]
        private string m_MainBlendTreeName = "Grounded";
        [SerializeField]
        private string m_MoveMotionName = "Forward";
        [SerializeField]
        private string m_TurnMotionName = "Turn";
        [SerializeField]
        private bool m_ApplyJumpMotion = true;
        [SerializeField]
        private string m_JumpMotionName = "Jump";
        [SerializeField]
        private string m_JumpLegMotionName = "JumpLeg";
        [SerializeField]
        private bool m_ApplyCrouchMotion = true;
        [SerializeField]
        private string m_CrouchParamName = "Crouch";
        [SerializeField]
        private string m_OnGroundParamName = "OnGround";

        public string MainBlendTreeName
        {
            get
            {
                return m_MainBlendTreeName;
            }
        }

        public string MoveMotionName
        {
            get
            {
                return m_MoveMotionName;
            }
        }

        public string TurnMotionName
        {
            get
            {
                return m_TurnMotionName;
            }
        }

        public string JumpMotionName
        {
            get
            {
                return m_JumpMotionName;
            }
        }

        public string OnGroundParamName
        {
            get
            {
                return m_OnGroundParamName;
            }
        }

        public string CrouchParamName
        {
            get
            {
                return m_CrouchParamName;
            }
        }

        public string JumpLegMotionName
        {
            get
            {
                return m_JumpLegMotionName;
            }
        }

        public bool ApplyJumpMotion
        {
            get
            {
                return m_ApplyJumpMotion;
            }
        }

        public bool ApplyCrouchMotion
        {
            get
            {
                return m_ApplyCrouchMotion;
            }
        }
    }
}