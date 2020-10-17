using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace ShipDock.Applications
{
    public static class DataSerialize
    {
        public const string mSuffix = ".bin";
        public static string mReadWritePath = Application.streamingAssetsPath + "/res_data/configs/";
        
        public static T Deserialize<T>(string key) where T : class
        {
            byte[] byteArray = GetLocalData(key);
            if (byteArray == null)
            {
                return null;
            }

            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(byteArray, 0, byteArray.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = (T)binForm.Deserialize(memStream);
                return obj;
            }
        }

        public static byte[] GetLocalData(string key)
        {
            mReadWritePath += key;
            mReadWritePath += mSuffix;
            if (!File.Exists(mReadWritePath))
            {
                return null;
            }
            try
            {
                FileStream fs = new FileStream(mReadWritePath, FileMode.Open, FileAccess.Read);
                byte[] buffur = new byte[fs.Length];
                fs.Read(buffur, 0, buffur.Length);
                fs.Close();
                return buffur;
            }
            catch(Exception error)
            {
                return default;
            }
        }

        public static void Serialize<T>(string name, T data) where T : class
        {
            FileStream fs = new FileStream(mReadWritePath, FileMode.OpenOrCreate);
            BinaryFormatter bf = new BinaryFormatter();
            bf.Serialize(fs, data);
            fs.Close();
        }

    }
}
