using System;

namespace ShipDock.Versioning
{
    [Serializable]
    public class ResVersion
    {
        public static ResVersion CreateNew(string resName, int resVersion, long resFileSize)
        {
            ResVersion result = new ResVersion
            {
                name = resName,
                version = resVersion,
                file_size = resFileSize
            };
            return result;
        }

        public string name;
        public int version;
        public long file_size;

        public string Url { get; set; }
    }

    [Serializable]
    public class ResUpdating : ResVersion
    {
    }
}