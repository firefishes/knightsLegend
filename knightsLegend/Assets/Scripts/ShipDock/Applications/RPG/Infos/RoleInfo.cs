using System;
using UnityEngine;

namespace ShipDock.Applications
{
    [Serializable]
    public class RoleInfo
    {

        public const float KHalf = 0.5f;

        [SerializeField]
        private float m_Hp;
        [SerializeField]
        private float m_Speed;
        [SerializeField]
        private float m_RunCycleLegOffset = 0.2f;
        [SerializeField]
        private float m_AnimSpeedMultiplier = 1f;
        [SerializeField]
        private float m_MoveSpeedMultiplier = 1f;
        
        public float GetHp()
        {
            return m_Hp;
        }

        public void SetHp(float v)
        {
            m_Hp = v;
        }

        public float GetSpeed()
        {
            return m_Speed;
        }

        public void SetSpeed(float v)
        {
            m_Speed = v;
        }

        public float RunCycleLegOffset()
        {
            return m_RunCycleLegOffset;
        }

        public void SetRunCycleLegOffset(float v)
        {
            m_RunCycleLegOffset = v;
        }

        public float AnimSpeedMultiplier()
        {
            return m_AnimSpeedMultiplier;
        }

        public void SetAnimSpeedMultiplier(float v)
        {
            m_AnimSpeedMultiplier = v;
        }

        public float MoveSpeedMultiplier()
        {
            return m_MoveSpeedMultiplier;
        }

        public void SetMoveSpeedMultiplier(float v)
        {
            m_MoveSpeedMultiplier = v;
        }
    }
}
