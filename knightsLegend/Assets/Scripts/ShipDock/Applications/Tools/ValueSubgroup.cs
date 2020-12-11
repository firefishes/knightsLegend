using ShipDock.Tools;
using System;
using UnityEngine;

namespace ShipDock.Applications
{

    [Serializable]
    public class ValueSubgroup
    {
        public string keyField;
        public ValueItemType valueType;

        [SerializeField]
        public string m_Str;
        [SerializeField]
        public float m_FloatValue;
        [SerializeField]
        public double m_DoubleValue;
        [SerializeField]
        public float m_DampTime;
        [SerializeField]
        public bool m_TriggerValue;
        [SerializeField]
        public Vector3 m_Vector;
        [SerializeField]
        private Color m_Color;
        [SerializeField]
        private LayerMask m_LayerMask;

        private ValueItem mCached;

#if UNITY_EDITOR
        public void Sync()
        {
            //m_Str = str;
            //m_FloatValue = floatValue;
            //m_DoubleValue = doubleValue;
            //m_DampTime = dampTime;
            //m_TriggerValue = triggerValue;
        }
#endif

        public ValueItem GetFloat()
        {
            return ValueItem.New(keyField, m_FloatValue).SetDampTime(m_DampTime);
        }

        public ValueItem GetBool()
        {
            return ValueItem.New(keyField, m_TriggerValue).SetDampTime(m_DampTime);
        }

        public ValueItem GetString()
        {
            return ValueItem.New(keyField, m_Str).SetDampTime(m_DampTime);
        }

        private ValueItem GetInt()
        {
            return ValueItem.New(keyField, (int)m_FloatValue).SetDampTime(m_DampTime);
        }

        private ValueItem GetDouble()
        {
            return ValueItem.New(keyField, m_DoubleValue).SetDampTime(m_DampTime);
        }
        
        public ValueItem Result(bool isRefresh = false)
        {
            if (isRefresh)
            {
                Clean();
            }
            if (mCached == default)
            {
                ValueItem result;
                int type = (int)valueType;
                switch (type)
                {
                    case ValueItem.STRING:
                        result = GetString();
                        break;
                    case ValueItem.INT:
                        result = GetInt();
                        break;
                    case ValueItem.DOUBLE:
                        result = GetDouble();
                        break;
                    case ValueItem.BOOL:
                        result = GetBool();
                        break;
                    case ValueItem.FLOAT:
                        result = GetFloat();
                        break;
                    default:
                        result = ValueItem.New(keyField, string.Empty);
                        break;
                }
                mCached = result;
            }
            return mCached;
        }

        public Vector2 GetV2()
        {
            return valueType == ValueItemType.VECTOR_2 ? new Vector2(m_Vector.x, m_Vector.y) : Vector2.zero;
        }

        public Vector3 GetV3()
        {
            return valueType == ValueItemType.VECTOR_3 ? m_Vector : Vector3.zero;
        }

        public Color GetColor()
        {
            return valueType == ValueItemType.COLOR ? m_Color : Color.clear;
        }

        public LayerMask GetLayerMask()
        {
            return valueType == ValueItemType.LAYER_MASK ? m_LayerMask : default;
        }

        public void Clean()
        {
            mCached = default;
        }
    }
}