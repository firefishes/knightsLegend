
#define G_LOG
#define _ASSERT

using ShipDock.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;
#if ASSERT
using UnityEngine.Assertions;
#endif
using AsserterMapper = ShipDock.Tools.KeyValueList<string, System.Collections.Generic.List<ShipDock.Testers.Asserter>>;
using LogsMapper = ShipDock.Tools.KeyValueList<string, ShipDock.Tools.KeyValueList<string, ShipDock.Testers.LogItem>>;
using TesterIndxMapper = ShipDock.Tools.KeyValueList<string, int>;
using TesterMapper = ShipDock.Tools.KeyValueList<string, ShipDock.Testers.ITester>;

namespace ShipDock.Testers
{
    public interface ITester
    {
        string Name { get; set; }
    }

    public class Asserter
    {
        public string title;
        public string content;
    }

    public class LogItem
    {
        public string format;
        public string logColor;
        public System.Action<string[]> onLoged;
    }

    public class Tester : Singletons<Tester>
    {

        public bool isShowLogCount;

        private int mLogCount;
        private ITester mDefaultTester;
        private UnityEngine.Object mLogSignTarget;

        private TesterIndxMapper mTesterIndexs;
        private AsserterMapper mAsserterMapper;
        private LogsMapper mLoggerMapper;
        private TesterMapper mTesterMapper;

        public Tester()
        {
#if G_LOG
            mTesterIndexs = new TesterIndxMapper();
            mAsserterMapper = new AsserterMapper();
            mLoggerMapper = new LogsMapper();
            mTesterMapper = new TesterMapper();
#endif
        }

        public void Init<T>(T defaultTester) where T : ITester
        {
            SetDefaultTester(defaultTester);
            Application.logMessageReceived += OnLogMessageReceived;
        }

        public void Clean()
        {
            Application.logMessageReceived -= OnLogMessageReceived;
            TestBrokers = default;
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void SetDefaultTester(ITester tester)
        {
            if(mDefaultTester == null)
            {
                mDefaultTester = tester;
                AddTester(mDefaultTester);

                AddLogger(mDefaultTester, "ast do not pass", "error: Asserter {0} do not pass in {1}");
                AddLogger(mDefaultTester, "ast not correct", "error: Tester correct is \"{0}\", it do not \"{1}\".");
                AddLogger(mDefaultTester, "tester hited", "Tester: [{0}] Assert coincide {1}/{2}. correct is {3}", "#7FE939");
                AddLogger(mDefaultTester, "all tester hited", "Tester: {0} All hit！", "#48DD22");
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
            bool hasName = string.IsNullOrEmpty(tester.Name);
            string name = !hasName ? StringUtils.GetQualifiedClassName(tester) : tester.Name;
            if (!mAsserterMapper.IsContainsKey(name))
            {
                mAsserterMapper[name] = new List<Asserter>();
                mLoggerMapper[name] = new KeyValueList<string, LogItem>();
                if (!hasName)
                {
                    tester.Name = name;
                }
            }
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void AddLogger(ITester tester, string logID, string format, string logColor = "", System.Action<string[]> onLogedMethod = null)
        {
            if (!mTesterMapper.ContainsKey(logID))
            {
                mTesterMapper[logID] = tester;
            }
            KeyValueList<string, LogItem> list = mLoggerMapper[tester.Name];
            list[logID] = new LogItem { format = format, logColor = logColor, onLoged = onLogedMethod };
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void Log(UnityEngine.Object logSignTarget, params string[] args)
        {
            mLogSignTarget = logSignTarget;
            Log(string.Empty, args);
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void Log(string logID, UnityEngine.Object logSignTarget, params string[] args)
        {
            mLogSignTarget = logSignTarget;
            Log(logID, args);
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void Log(bool logFilters, params string[] args)
        {
            if(logFilters)
            {
                Log(string.Empty, args);
            }
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void Log(string logID, bool logFilters, params string[] args)
        {
            if (logFilters)
            {
                Log(logID, args);
            }
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void Log(params string[] args)
        {
            LogFromTester(string.Empty, args);
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void LogAndAssert(string logID, string title, string assertTarget, params string[] args)
        {
            if (mAsserterMapper.IsContainsKey(title))
            {
                LogFromTester(logID, args);
                Asserting(title, assertTarget);
            }
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void Log(string logID, params string[] args)
        {
            LogFromTester(logID, args);
        }

        [System.Diagnostics.Conditional("G_LOG")]
        private void LogFromTester(string logID, params string[] args)
        {
            ITester tester = mTesterMapper[logID];
            ITester target = tester ?? mDefaultTester;
            LogItem logger;
            if (target == null)
            {
                if (logID.Contains("{0}"))
                {
                    logger = new LogItem
                    {
                        format = logID,
                    };
                }
                else
                {
                    string log = logID.Append(" - ");
                    int max = args.Length;
                    for (int i = 0; i < max; i++)
                    {
                        log = log.Append(args[i]);
                    }
                    Debug.Log(log);
                    return;
                }
            }
            else
            {
                KeyValueList<string, LogItem> list = mLoggerMapper[target.Name];
                bool hasLogger = (list != null) && list.IsContainsKey(logID);
                if (hasLogger)
                {
                    logger = list[logID];
                }
                else
                {
                    if (logID.Contains("{0}"))
                    {
                        logger = new LogItem
                        {
                            format = logID,
                        };
                    }
                    else
                    {
                        return;
                    }
                }
            }
            CheckLogger(ref logID, ref logger, ref args);
        }

        private void CheckLogger(ref string logID, ref LogItem logger, ref string[] args)
        {
            string log;
            logger.onLoged?.Invoke(args);
            if (isShowLogCount)
            {
                log = string.Format(logger.format, args).Append("(", mLogCount.ToString(), ")");
            }
            else
            {
                log = string.Format(logger.format, args);
            }
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
            TestBrokers?.Invoke(logID, args);
            mLogCount++;
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void AddAsserter(string title, bool isIgnore, params string[] content)
        {
            if (isIgnore)
            {
                return;
            }

            List<Asserter> list;
            if (mAsserterMapper.ContainsKey(title))
            {
                list = mAsserterMapper[title];
            }
            else
            {
                list = new List<Asserter>();
                mAsserterMapper[title] = list;
                mTesterIndexs[title] = 0;
            }
            Asserter result;
            int max = content.Length;
            for (int i = 0; i < max; i++)
            {
                result = new Asserter { title = title, content = content[i] };
                list.Add(result);
            }
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void Asserting(string title, string target, bool moveNext = true)
        {
            if (mAsserterMapper.IsContainsKey(title))
            {
                List<Asserter> list = mAsserterMapper[title];
                int max = list.Count;
                if (max != 0)
                {
                    int index = mTesterIndexs[title];
                    Asserter asserter = list[index];
                    string correct = asserter.content;
#if ASSERT && UNITY_EDITOR
                    try
                    {
                        Assert.AreEqual(target, correct);
                        MoveNextAsserter(ref index, max, moveNext, ref title);
                    }
                    catch (System.Exception _) { }
#else
                    bool result = target != correct;
                    if (result)
                    {
                        "ast do not pass".Log(asserter.title, index.ToString());
                        "ast not correct".Log(correct, target);
                    }
                    else
                    {
                        int next = index + 1;
                        "tester hited".Log(asserter.title, next.ToString(), max.ToString(), target);
                        ShouldMoveNextAsserter(ref index, max, moveNext, ref title);
                    }
#endif
                }
            }
        }

        [System.Diagnostics.Conditional("G_LOG")]
        private void ShouldMoveNextAsserter(ref int index, int max, bool moveNext, ref string title)
        {
            if (moveNext)
            {
                index++;
                mTesterIndexs.Put(title, index);
            }
            if (index >= max)
            {
                "all tester hited".Log(title);
                mAsserterMapper.Remove(title);
                mTesterIndexs.Remove(title);
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

        [System.Diagnostics.Conditional("G_LOG")]
        public void SetTestBrokerHandler(Action<string, string[]> handler)
        {
            TestBrokers += handler;
        }

        private Action<string, string[]> TestBrokers { get; set; }
    }
}