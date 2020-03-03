#define G_LOG
#define _LOG_WHEN_ERROR_TYPE
#define _LOG_WHEN_POOLING_CLEAN
#define _LOG_DIR_FROM_DEVICE_LOCAL_CHECKED
#define LOG_FSM

using ShipDock.Tools;

namespace ShipDock.Testers
{
    public class TesterBaseApp : Singletons<TesterBaseApp>, ITester
    {
        public const int LOG = -1;
        public const int LOG0 = 0;
        public const int LOG1 = 1;
        public const int LOG2 = 2;
        public const int LOG3 = 3;
        public const int LOG4 = 4;
        public const int LOG5 = 5;
        public const int LOG6 = 6;
        public const int LOG7 = 7;
        public const int LOG8 = 8;
        public const int LOG9 = 9;

        public TesterBaseApp()
        {
            Tester tester = Tester.Instance;
            tester.AddTester(this);
            tester.AddLogger(this, LOG, "{0}");
#if LOG_WHEN_ERROR_TYPE
            tester.AddLogger(this, LOG0, "error: A wrong pooling revert, instance type is {0}");
#endif
#if LOG_WHEN_POOLING_CLEAN
            tester.AddLogger(this, LOG1, "log: Pooling {0} clear..");
#endif
#if LOG_DIR_FROM_DEVICE_LOCAL_CHECKED
            tester.AddLogger(this, LOG2, "log: Asset is from priv: {0}, path key is: {1}");
            tester.AddLogger(this, LOG3, "log: Asset path is : {0}");
#endif
#if LOG_FSM
            tester.AddLogger(this, LOG4, "{0} FSM state changed : {1} -> {2}");
            tester.AddLogger(this, LOG5, "error: state repeate, FSM name is {0}");
#endif
            tester.AddLogger(this, LOG6, "Writing File: {0}");
            tester.AddLogger(this, LOG7, "log: {0} {1}");
            tester.AddLogger(this, LOG8, "warning: {0} {1}");
            tester.AddLogger(this, LOG9, "error: {0} {1}");
        }

        [System.Diagnostics.Conditional("G_LOG")]
        public void LogWhenPoolingItemDoNotMatch<T>(ref T target)
        {
            if (typeof(T).FullName != target.GetType().FullName)
            {
                Tester.Instance.Log(this, LOG0, target.GetType().FullName);
            }
        }

        public string Name { get; set; }
    }
}