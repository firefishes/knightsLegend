
#define G_LOG
#define _ASSERT_TESTER

using ShipDock.Tools;
using System.Collections.Generic;
using UnityEngine;
#if ASSERT_TESTER
using NUnit.Framework;
#endif

namespace ShipDock.Testers
{
    public interface ITester
    {
        string Name { get; set; }
    }

    public class Asserter
    {
        public bool canIgnore;
        public string target;
        public string content;
    }

    public class LogItem
    {
        public string format;
        public string logColor;
    }

    public class Tester : Singletons<Tester>
    {
        
        private int mLogCount;
        private ITester mDefaultTester;
        private Object mLogSignTarget;
        private KeyValueList<string, int> mTesterIndexs = new KeyValueList<string, int>();
        private KeyValueList<string, List<Asserter>> mTesterMapper = new KeyValueList<string, List<Asserter>>();
        private KeyValueList<string, KeyValueList<int, LogItem>> mLoggerMapper = new KeyValueList<string, KeyValueList<int, LogItem>>();

        public void Init<T>(T defaultTester) where T : ITester
        {
            SetDefaultTester(defaultTester);
            Application.logMessageReceived += OnLogMessageReceived;
        }

        public void Clean()
        {
            Application.logMessageReceived -= OnLogMessageReceived;
        }

        public void SetDefaultTester(ITester tester)
        {
            if(mDefaultTester == null)
            {
                mDefaultTester = tester;
                AddTester(mDefaultTester);
            }
        }

        /// <summary>接收日志消息的回调函数</summary>
        private void OnLogMessageReceived(string condition, string stackTrace, LogType type)
        {
            switch (type)
            {
                case LogType.Assert:
                    break;
                case LogType.Error:
                    break;
            }
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void AddTester(ITester tester)
        {
            if (tester == mDefaultTester)
            {
                return;
            }
            string name = StringUtils.GetQualifiedClassName(tester);
            if (!mTesterMapper.IsContainsKey(name))
            {
                mTesterMapper[name] = new List<Asserter>();
                mLoggerMapper[name] = new KeyValueList<int, LogItem>();
                mTesterIndexs[name] = 0;
                tester.Name = name;
            }
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void AddLogger(ITester tester, int logID, string format, string logColor = "")
        {
            KeyValueList<int, LogItem> list = mLoggerMapper[tester.Name];
            list[logID] = new LogItem { format = format, logColor = logColor };
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void Log(int logID, Object logSignTarget, params string[] args)
        {
            mLogSignTarget = logSignTarget;
            Log(logID, args);
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void Log(ITester tester, int logID, Object logSignTarget, params string[] args)
        {
            mLogSignTarget = logSignTarget;
            Log(tester, logID, args);
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void Log(int logID, bool logFilters, params string[] args)
        {
            if(logFilters)
            {
                Log(logID, args);
            }
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void Log(ITester tester, int logID, bool logFilters, params string[] args)
        {
            if (logFilters)
            {
                Log(tester, logID, args);
            }
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void Log(int logID, params string[] args)
        {
            LogFromTester(null, logID, args);
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void Log(ITester tester, int logID, params string[] args)
        {
            LogFromTester(tester, logID, args);
        }

        [System.Diagnostics.Conditional("G_LOG")]
        private void LogFromTester(ITester tester, int logID, params string[] args)
        {
            ITester target = tester ?? mDefaultTester;
            if (target == null)
            {
                return;
            }

            KeyValueList<int, LogItem> list = mLoggerMapper[target.Name];
            if ((list != null) && list.IsContainsKey(logID))
            {
                LogItem logger = list[logID];
                string log = string.Format(logger.format, args).Append("   (", mLogCount.ToString(), ")");
                if (mLogSignTarget != null)
                {
                    DebugUtils.LogInColorAndSignIt(mLogSignTarget, log);
                    mLogSignTarget = null;
                }
                else
                {
                    if (string.IsNullOrEmpty(logger.logColor))
                    {
                        DebugUtils.LogInColor(log);
                    }
                    else
                    {
                        DebugUtils.LogInColor(logger.logColor, log);
                    }
                }
                mLogCount++;
            }
        }

        public void AddTestContent(ITester tester, string content, bool canIgnore = false)
        {
            List<Asserter> list = mTesterMapper[tester.Name];
            list.Add(new Asserter { content = content, canIgnore = canIgnore });
        }

        public void AddNullOrEmptyCheck(ITester tester, bool isNullOrEmpty, string logIfNull, string logIfNotNull, bool canIgnore = false)
        {
            List<Asserter> list = mTesterMapper[tester.Name];
            list.Add(new Asserter
            {
                content = isNullOrEmpty ? logIfNull : logIfNotNull,
                canIgnore = canIgnore
            });
        }

        public void Asserts(string target, string hitSuccess = "", string hitFailed = "")
        {
            Asserts(mDefaultTester, hitSuccess, hitFailed);
        }

        public void Asserts(ITester tester, bool assertResult, string hitSuccess = "", string hitFailed = "")
        {
        }

        public void Asserts(ITester tester, string target, string hitSuccess = "", string hitFailed = "")
        {
            if (!mTesterMapper.IsContainsKey(tester.Name))
            {
                return;
            }

            int index = mTesterIndexs[tester.Name];
            List<Asserter> list = mTesterMapper[tester.Name];

            Asserter asserter = list[index];
            string content = asserter.content;
#if ASSERT_TESTER
            Assert.AreEqual(target, content);
#else
            bool result = target != content;
            if (result && !asserter.canIgnore)
            {
                string exceptionContent = string.Format("Tester do not pass in {0} : {1}", tester.Name, index);
                DebugUtils.LogInColor("warning: Tester should be \"", target, "\", but in fact it is \"", content, "\".");
                if (!string.IsNullOrEmpty(hitFailed))
                {
                    DebugUtils.LogInColor(hitFailed);
                }
            }
#endif
            string hited;
            if (!asserter.canIgnore)
            {
                hited = string.Format("[{0}] Target hit {1}/{2}. {3}", tester.Name, index + 1, list.Count, content);
                DebugUtils.LogInColor("#7FE939", "Tester: ", hited);
            }

            if (!string.IsNullOrEmpty(hitSuccess))
            {
                DebugUtils.LogInColor(hitSuccess);
            }

            index++;
            mTesterIndexs.Put(tester.Name, index);
            if (index >= list.Count)
            {
                hited = string.Format("Tester {0} All hit！", tester.Name);
                DebugUtils.LogInColor("#48DD22", hited);

                mTesterMapper.Remove(tester.Name);
                mTesterIndexs.Remove(tester.Name);
            }
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void InjectTestValue<T>(ref T raw, T value, bool apply = false)
        {
            if(apply)
            {
                raw = value;
            }
        }
    }
}