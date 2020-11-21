using ShipDock.Tools;
using System;

namespace ShipDock.Applications
{

    [Serializable]
    public class ValueSubgroup
    {
        public string keyField;
        public ValueItemType valueType;
        public string str;
        public float floatValue;
        public double doubleValue;
        public float dampTime;
        public bool triggerValue;

        private ValueItem mCached;

        public ValueItem GetFloat()
        {
            return ValueItem.New(keyField, floatValue).SetDampTime(dampTime);
        }

        public ValueItem GetBool()
        {
            return ValueItem.New(keyField, triggerValue).SetDampTime(dampTime);
        }

        public ValueItem GetString()
        {
            return ValueItem.New(keyField, str).SetDampTime(dampTime);
        }

        private ValueItem GetInt()
        {
            return ValueItem.New(keyField, (int)floatValue).SetDampTime(dampTime);
        }

        private ValueItem GetDouble()
        {
            return ValueItem.New(keyField, doubleValue).SetDampTime(dampTime);
        }
        
        public ValueItem GetValue(bool isRefresh = false)
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

        public void Clean()
        {
            mCached = default;
        }
    }
}