using System;
using UnityEngine;
using System.Collections.Generic;

namespace ShipDock.Tools
{
    [Serializable]
    public class LogsSubgroup
    {
#if G_LOG
        [SerializeField]
        private List<string> m_LoggerName;
#endif

        [System.Diagnostics.Conditional("G_LOG")]
        public void UpdateLogs(string logID)
        {
            //if ()
            //{

            //}
        }
    }

    [Serializable]
    public class LogSubgroupItem
    {
        public string name;
    }

}