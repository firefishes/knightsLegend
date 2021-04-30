using ShipDock.Interfaces;
using ShipDock.Tools;
using System;
using System.Collections.Generic;

namespace ShipDock.Datas
{
    public abstract class FieldableData : IFieldableData, IDataUnit, IDispose
    {
        private int[] mIntValues;
        private float[] mFloatValues;
        private string[] mStringValues;
        private List<bool> mHasChanges;
        private List<int> mAllFields;
        private List<Action> mOnValuesChanged;

        public FieldableData()
        {
        }

        public virtual void Dispose()
        {
            Utils.Reclaim(ref mIntValues);
            Utils.Reclaim(ref mFloatValues);
            Utils.Reclaim(ref mStringValues);
            Utils.Reclaim(ref mHasChanges);
            Utils.Reclaim(ref mAllFields);
            Utils.Reclaim(ref mOnValuesChanged);
        }

        public abstract List<int> GetIntFieldSource();
        public abstract List<float> GetFloatFieldSource();
        public abstract List<string> GetStringFieldSource();

        protected void FillValues()
        {
            FillValuesByFields(IntFieldNames, ref mIntValues, GetIntFieldSource());
            FillValuesByFields(FloatFieldNames, ref mFloatValues, GetFloatFieldSource());
            FillValuesByFields(StringFieldNames, ref mStringValues, GetStringFieldSource());
            
            int max = 0;
            AddFieldsToAllFieldNames(IntFieldNames, ref max);
            AddFieldsToAllFieldNames(FloatFieldNames, ref max);
            AddFieldsToAllFieldNames(StringFieldNames, ref max);
            
            mHasChanges = new List<bool>(max);
            for (int i = 0; i < max; i++)
            {
                mHasChanges.Add(false);
            }

            mOnValuesChanged = new List<Action>(max);
            for (int i = 0; i < max; i++)
            {
                mOnValuesChanged.Add(default);
            }
        }

        private void AddFieldsToAllFieldNames(List<int> list, ref int totalFieldCount)
        {
            if (mAllFields == default)
            {
                mAllFields = new List<int>();
            }
            int count = list != default ? list.Count : 0;
            if(count > 0)
            {
                totalFieldCount += count;
                mAllFields.Contact(list);
            }
        }

        protected void FillValuesByFields<T>(List<int> fields, ref T[] values, List<T> willFillIn)
        {
            int max = fields != default ? fields.Count : 0;
            if (max > 0)
            {
                values = new T[max];
                for (int i = 0; i < max; i++)
                {
                    values[i] = willFillIn != null ? willFillIn[i] : default;
                }
            }
        }

        public float GetIntData(int fieldName)
        {
            return GetData(fieldName, IntFieldNames, ref mIntValues);
        }

        public void SetIntData(int fieldName, int value)
        {
            SetData(fieldName, value, IntFieldNames, ref mIntValues);
        }

        public float GetFloatData(int fieldName)
        {
            return GetData(fieldName, FloatFieldNames, ref mFloatValues);
        }

        public void SetFloatData(int fieldName, float value)
        {
            SetData(fieldName, value, FloatFieldNames, ref mFloatValues);
        }

        public string GetStringData(int fieldName)
        {
            return GetData(fieldName, StringFieldNames, ref mStringValues);
        }

        public void SetStringData(int fieldName, string value)
        {
            SetData(fieldName, value, StringFieldNames, ref mStringValues);
        }

        private void SetData<T>(int fieldName, T value, List<int> fieldNames, ref T[] values)
        {
            int index = fieldNames.IndexOf(fieldName);

            if (ApplyAutoChangedCheck)
            {
                T old = values[index];
                if (!old.Equals(value))
                {
                    values[index] = value;
                    index = mAllFields.IndexOf(fieldName);
                    mHasChanges[index] = true;
                }
            }
            else
            {
                values[index] = value;
            }
        }

        public T GetData<T>(int fieldName, List<int> fieldNames, ref T[] values)
        {
            int index = fieldNames.IndexOf(fieldName);
            return values[index];
        }

        public bool HasFieldChanged(int fieldName, bool applyReset = true)
        {
            if (!ApplyAutoChangedCheck)
            {
                return true;
            }

            int index = mAllFields.IndexOf(fieldName);
            bool result = mHasChanges[index];
            if (applyReset && result)
            {
                mHasChanges[index] = false;
            }
            return result;
        }

        public void SetValueChanged(params Action[] actions)
        {
            if (!ApplyAutoChangedCheck)
            {
                return;
            }

            int max = actions.Length;
            for (int i = 0; i < max; i++)
            {
                mOnValuesChanged[i] = actions[i];
            }
        }

        public void SetValueChanged(int fieldName, Action method)
        {
            if (!ApplyAutoChangedCheck)
            {
                return;
            }

            int index = mAllFields.IndexOf(fieldName);
            mOnValuesChanged[index] = method;
        }

        public void AfterValueChange(int index)
        {
            if (!ApplyAutoChangedCheck)
            {
                return;
            }

            mOnValuesChanged[index]?.Invoke();
        }

        public List<int> AllFieldNames
        {
            get
            {
                return mAllFields;
            }
        }

        public bool ApplyAutoChangedCheck { get; set; } = true;
        public virtual List<int> IntFieldNames { get; protected set; }
        public virtual List<int> FloatFieldNames { get; protected set; }
        public virtual List<int> StringFieldNames { get; protected set; }

    }
}