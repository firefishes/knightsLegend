using ShipDock.Interfaces;
using ShipDock.Tools;
using System;
using System.Collections.Generic;

namespace ShipDock.Datas
{
    public class Data : IData, IDispose
    {
        private List<IDataExtracter> mDataHandlers;
        private Action<IData, int> mOnDataChanged;

        public Data(int dataName)
        {
            DataName = dataName;
            mDataHandlers = new List<IDataExtracter>();
        }

        public virtual void Dispose()
        {
            Utils.Reclaim(ref mDataHandlers);
            mOnDataChanged = default;
        }

        public void DataChanged(params int[] keys)
        {
            int keyName;
            int max = keys.Length;
            for (int i = 0; i < max; i++)
            {
                keyName = keys[i];
                mOnDataChanged?.Invoke(this, keyName);
            }
        }

        public void Register(IDataExtracter dataHandler)
        {
            if((dataHandler == default) || mDataHandlers.Contains(dataHandler))
            {
                return;
            }
            mDataHandlers.Add(dataHandler);
            mOnDataChanged += dataHandler.OnDataChanged;
        }

        public void Unregister(IDataExtracter dataHandler)
        {
            if((dataHandler == default) || !mDataHandlers.Contains(dataHandler))
            {
                return;
            }
            mDataHandlers.Remove(dataHandler);
            mOnDataChanged -= dataHandler.OnDataChanged;
        }

        public int DataName { get; private set; }
    }

    public class DataStorager : Data
    {
        private KeyValueList<int, IDataUnit> mStorager;

        public DataStorager(int dataName) : base(dataName)
        {
            mStorager = new KeyValueList<int, IDataUnit>();
        }

        public bool HasDataUnit(int key)
        {
            return mStorager.ContainsKey(key);
        }

        public void SetDataUnit(int key, IDataUnit data)
        {
            if (data == default)
            {
                mStorager.Remove(key);
            }
            else
            {
                mStorager.Put(key, data);
            }
        }

        public IDataUnit GetDataUnit<T>(int key) where T : IDataUnit
        {
            IDataUnit result = mStorager[key];
            return result == default ? default : (T)result;
        }
    }

    public abstract class FieldableData : IFieldableData, IDataUnit
    {
        private int[] mIntValues;
        private float[] mFloatValues;
        private string[] mStringValues;

        public FieldableData()
        {
            int max = IntFieldNames.Count;
            mFloatValues = new float[max];
            for (int i = 0; i < max; i++)
            {
                mFloatValues[i] = 0f;
            }
        }

        public abstract List<int> GetIntFieldSource();
        public abstract List<float> GetFloatFieldSource();
        public abstract List<string> GetStringFieldSource();

        public void FillValues()
        {
        }

        protected void FillValuesByFields<T>(List<T> fields, ref T[] values, T[] willFillIn)
        {
            int max = fields.Count;
            values = new T[max];
            for (int i = 0; i < max; i++)
            {
                values[i] = willFillIn != null ? willFillIn[i] : default;
            }
        }

        public float GetIntData(int fieldName)
        {
            int index = IntFieldNames.IndexOf(fieldName);
            return mIntValues[index];
        }

        public void SetIntData(int fieldName, int value)
        {
            int index = IntFieldNames.IndexOf(fieldName);
            mIntValues[index] = value;
        }

        public float GetFloatData(int fieldName)
        {
            int index = FloatFieldNames.IndexOf(fieldName);
            return mFloatValues[index];
        }

        public void SetFloatData(int fieldName, float value)
        {
            int index = FloatFieldNames.IndexOf(fieldName);
            mFloatValues[index] = value;
        }

        public string GetStringData(int fieldName)
        {
            int index = StringFieldNames.IndexOf(fieldName);
            return mStringValues[index];
        }

        public void SetStringData(int fieldName, string value)
        {
            int index = StringFieldNames.IndexOf(fieldName);
            mStringValues[index] = value;
        }

        public List<int> IntFieldNames { get; protected set; }
        public List<int> FloatFieldNames { get; protected set; }
        public List<int> StringFieldNames { get; protected set; }
    }

    public interface IDataUnit
    {

    }

    public interface IFieldableData
    {
        float GetIntData(int fieldName);
        void SetIntData(int fieldName, int value);
        float GetFloatData(int fieldName);
        void SetFloatData(int fieldName, float value);
        string GetStringData(int fieldName);
        void SetStringData(int fieldName, string value);
        List<int> IntFieldNames { get; }
        List<int> FloatFieldNames { get; }
        List<int> StringFieldNames { get; }
    }

    public interface ISourceData
    {

    }
}
