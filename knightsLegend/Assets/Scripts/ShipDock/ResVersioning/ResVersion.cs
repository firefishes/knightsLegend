using System;

namespace ShipDock.Versioning
{
    [Serializable]
    public class ResVersion
    {
        public string name;
        public int version;

        public string Url { get; set; }
    }

    [Serializable]
    public class ResUpdating : ResVersion
    {
    }
}