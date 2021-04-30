using System;
using System.Collections.Generic;

namespace ShipDock.Config
{
    [Serializable]
    public class TDData
    {
        private string mValue;
        private string[] mItem;

        public TDData()
        {
            Source = new List<string[]>();
        }

        public void Add(ref string[] item)
        {
            Source.Add(item);
        }

        public string GetValue(int row, int col)
        {
            mItem = Source[row];
            return mItem[col];
        }

        public int GetIntValue(int row, int col)
        {
            mValue = GetValue(row, col);
            return int.Parse(mValue);
        }

        public float GetFloatValue(int row, int col)
        {
            mValue = GetValue(row, col);
            return float.Parse(mValue);
        }

        public List<string[]> Source { get; private set; }
    }
}