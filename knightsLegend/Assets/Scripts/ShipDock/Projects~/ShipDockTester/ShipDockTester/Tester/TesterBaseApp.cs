#define G_LOG
#define _LOG_NOTICE
#define LOG_LOADER
#define LOG_LOADER_FAILED
#define _LOG_LOADER_SUCCESS
#define LOG_LOADER_DEPS
#define LOG_LOADER_LOAD
#define LOG_POOLING
#define LOG_WHEN_ERROR_POOL_TYPE
#define _LOG_DIR_FROM_DEVICE_LOCAL_CHECKED
#define LOG_FSM


namespace ShipDock.Testers
{
    public class TesterBaseApp : ITester
    {
        public TesterBaseApp()
        {
            Tester tester = Tester.Instance;
            tester.AddTester(this);
            tester.AddLogger(this, "debug", "{0}");
#if LOG_NOTICE
            tester.AddLogger(this, "notice add", "log: {0}- Notice {1} registered");
            tester.AddLogger(this, "notice send", "log: {0}- Send notice {1} ");
            tester.AddLogger(this, "notice rm", "log: {0}- Remove notice {1}");
#endif
#if LOG_POOLING
#if LOG_WHEN_ERROR_POOL_TYPE
            tester.AddLogger(this, "pool type error", "error: A wrong pooling revert, instance type is {0}");
#endif
#endif
#if LOG_DIR_FROM_DEVICE_LOCAL_CHECKED
            tester.AddLogger(this, LOG2, "log: Asset is from priv: {0}, path key is: {1}");
            tester.AddLogger(this, LOG3, "log: Asset path is : {0}");
#endif
#if LOG_LOADER
#if LOG_LOADER_FAILED
            tester.AddLogger(this, "loader failed", "error: Loader load failed, Url is {0}");
#endif
#if LOG_LOADER_SUCCESS
            tester.AddLogger(this, "loader success", "log: Loader successed: {0}");
#endif
#if LOG_LOADER_DEPS
            tester.AddLogger(this, "loader deps", "log: Loader complete and get dependency: {0}");
            tester.AddLogger(this, "empty deps", "warning: dependences is empty");
            tester.AddLogger(this, "deps", "log: Dependence: {0} => {1}");
            tester.AddLogger(this, "walk deps", "log: Walk dependences will out of stacks");
#endif
#if LOG_LOADER_LOAD
            tester.AddLogger(this, "load res", "log: Load res in dependence {0}");
#endif
#endif
#if LOG_FSM
            tester.AddLogger(this, "fsm changed", "{0} FSM state changed : {1} -> {2}");
            tester.AddLogger(this, "fsm state repeate", "error: state repeate, FSM name is {0}");
#endif
            tester.AddLogger(this, "wr file", "Writing File: {0}");
            tester.AddLogger(this, "log", "log: {0} ");
            tester.AddLogger(this, "warning", "warning: {0} ");
            tester.AddLogger(this, "error", "error: {0} ");
            tester.AddLogger(this, "todo", "todo: {0} ");

            tester.AddAsserter("framework start", false, "Welcom..", "Ticks Ready", "Managers Ready", "Framework Started");
        }

        public string Name { get; set; }
    }
}