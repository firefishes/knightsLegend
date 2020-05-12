using ShipDock.Interfaces;
using ShipDock.Tools;
using System.Collections.Generic;

namespace ShipDock.Datas
{
    public abstract class FieldableData : IFieldableData, IDataUnit, IDispose
    {
        private int[] mIntValues;
        private float[] mFloatValues;
        private string[] mStringValues;

        public FieldableData()
        {
        }

        public virtual void Dispose()
        {
            Utils.Reclaim(ref mIntValues);
            Utils.Reclaim(ref mFloatValues);
            Utils.Reclaim(ref mStringValues);
        }

        public abstract List<int> GetIntFieldSource();
        public abstract List<float> GetFloatFieldSource();
        public abstract List<string> GetStringFieldSource();

        protected void FillValues()
        {
            FillValuesByFields(IntFieldNames, ref mIntValues, GetIntFieldSource());
            FillValuesByFields(FloatFieldNames, ref mFloatValues, GetFloatFieldSource());
            FillValuesByFields(StringFieldNames, ref mStringValues, GetStringFieldSource());
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
            values[index] = value;
        }

        public T GetData<T>(int fieldName, List<int> fieldNames, ref T[] values)
        {
            int index = fieldNames.IndexOf(fieldName);
            return values[index];
        }

        public virtual List<int> IntFieldNames { get; protected set; }
        public virtual List<int> FloatFieldNames { get; protected set; }
        public virtual List<int> StringFieldNames { get; protected set; }
    }
}