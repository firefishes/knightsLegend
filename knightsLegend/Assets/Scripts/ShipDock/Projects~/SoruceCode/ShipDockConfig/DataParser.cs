using ShipDock.Tools;
using UnityEngine;

namespace ShipDock.Config
{
    public static class DataParser
    {
        private const string ARR_SIGN_LEFT = "[";
        private const string ARR_SIGN_RIGHT = "]";
        private const string TD_ARR_SIGN_MID = "],[";
        private const string TD_ARR_SIGN_EMPTY_END = ",[]";
        private const string TD_ARR_SIGN_EMPTY_START = "[],";
        private const string TD_ARR_HYPHENS = "/";
        private const char TD_CHAR_HYPHENS = '/';

        private static void ReplaceArrFeature(ref string value, bool isTwoDimension = false)
        {
            if (!string.IsNullOrEmpty(value))
            {
                int len = 2;
                value = value.Length > len ? 
                    value.Substring(1, value.Length - len) : value.Substring(1, value.Length - 1);//去掉外层特征
                if (isTwoDimension)
                {
                    //替换二维数组配置数据的字符串特征
                    //temp = temp.Replace(TD_ARR_SIGN_EMPTY_START, TD_ARR_HYPHENS);//TODO 之后可能用得上
                    value = value.Replace(TD_ARR_SIGN_EMPTY_END, TD_ARR_HYPHENS);
                    value = value.Replace(TD_ARR_SIGN_MID, TD_ARR_HYPHENS);
                }

                //替换常规数组配置数据的字符串特征
                value = value.Replace(ARR_SIGN_LEFT, string.Empty);
                value = value.Replace(ARR_SIGN_RIGHT, string.Empty);
            }
        }

        public static int[] ParseParamToInts(ref string data)
        {
            string temp = data;

            ReplaceArrFeature(ref temp);

            string item;
            string[] splits = temp.Split(StringUtils.SPLIT_CHAR);
            int max = splits.Length;
            int[] result = new int[max];
            for (int i = 0; i < max; i++)
            {
                item = splits[i];
                if (!string.IsNullOrEmpty(item))
                {
                    result[i] = int.Parse(item);
                }
            }
            return result;
        }

        public static string[] ParseParamToStrings(ref string data)
        {
            string temp = data;

            ReplaceArrFeature(ref temp);

            string[] result = temp.Split(StringUtils.SPLIT_CHAR);
            return result;
        }

        public static float[] ParseParamToFloats(ref string data, float multiplying = -1f)
        {
            string temp = data;

            ReplaceArrFeature(ref temp);

            string[] splits = temp.Split(StringUtils.SPLIT_CHAR);
            int max = splits.Length;
            int value;
            float[] result = new float[max];
            for (int i = 0; i < max; i++)
            {
                if (multiplying >= 0f)
                {
                    value = int.Parse(splits[i]);
                    result[i] = value * multiplying;
                }
                else
                {
                    result[i] = float.Parse(splits[i]);
                }
            }
            return result;
        }

        public static TDData ParseParamToTD(ref string data)
        {
            string temp = data;

            ReplaceArrFeature(ref temp, true);

            string[] item;
            string[] splits = temp.Split(TD_CHAR_HYPHENS);
            int max = splits.Length;

            TDData td = new TDData();
            for (int i = 0; i < max; i++)
            {
                temp = splits[i];
                if (!string.IsNullOrEmpty(temp))
                {
                    item = temp.Split(StringUtils.SPLIT_CHAR);
                    td.Add(ref item);
                }
                else { }
            }
            return td;
        }

        public static void ParseAttributes(ref string data, out int[] keys, out int[] values)
        {
            TDData temp = ParseParamToTD(ref data);
            int row = temp.Source.Count;
            keys = new int[row];
            values = new int[row];
            string[] vs;
            for (int i = 0; i < row; i++)
            {
                vs = temp.Source[i];
                keys[i] = int.Parse(vs[0]);
                values[i] = int.Parse(vs[1]);
            }
        }

        public static Vector3 ParseParamToVector3(ref string data, float multiplying = -1f)
        {
            float[] values = ParseParamToFloats(ref data, multiplying);
            return new Vector3(values[0], values[1], values[2]);
        }
    }

}