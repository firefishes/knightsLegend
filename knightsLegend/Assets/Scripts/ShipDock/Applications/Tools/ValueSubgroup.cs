using ShipDock.Tools;
using System;
using UnityEngine;

namespace ShipDock.Applications
{
    [Serializable]
    public class ValueSubgroup
    {
        public string keyField;
        [HideInInspector]
        public int valueType;
        [HideInInspector]
        public string str;
        [HideInInspector]
        public float floatValue;
        [HideInInspector]
        public double doubleValue;
#if UNITY_EDITOR
        public ValueItemType valueTypeInEditor;
        public string valueInEditor;
#endif

        public bool triggerValue;

        public ValueItem GetFloat()
        {
            return ValueItem.New(keyField, floatValue);
        }

        public ValueItem GetBool()
        {
            return ValueItem.New(keyField, triggerValue);
        }

        public ValueItem GetString()
        {
            return ValueItem.New(keyField, str);
        }

        private ValueItem GetInt()
        {
            return ValueItem.New(keyField, (int)floatValue);
        }

        private ValueItem GetDouble()
        {
            return ValueItem.New(keyField, doubleValue);
        }

        public ValueItem GetValue()
        {
            ValueItem result;
            switch (valueType)
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
            return result;
        }
    }
}