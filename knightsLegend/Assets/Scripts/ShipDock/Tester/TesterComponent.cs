#define G_LOG

using System;
using UnityEngine;

namespace ShipDock.Tools
{
    public class TesterComponent : MonoBehaviour
    {
        [SerializeField]
        private TesterBroker[] m_TesterBrokers;

        private void Awake()
        {
            Testers.Tester.Instance.SetTestBrokerHandler(OnTestBrokerHandler);
        }

        private void OnTestBrokerHandler(string logID, string[] args)
        {
            TesterBroker item;
            int max = m_TesterBrokers.Length;
            for (int i = 0; i < max; i++)
            {
                item = m_TesterBrokers[i];
                if (item.isValid && (logID == item.logID))
                {
                    if (item.applyArgBreak)
                    {
                        if ((args.Length > item.argIndex) && (args[item.argIndex] == item.testValue))
                        {
                            "log".Log("Tester said: value = ".Append(args[item.argIndex]));
                        }
                    }
                    else
                    {
                        "log".Log("Tester broken: ".Append(item.testValue));
                    }
                }
            }
        }
    }

    [Serializable]
    public class TesterBroker
    {
        public bool isValid;
        public string logID;
        public bool applyArgBreak;
        public int argIndex;
        public string testValue;
    }

}