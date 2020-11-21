#define G_LOG
#define LOADER
#define LOADER_FAILED
#define _LOADER_SUCCESS
#define _LOADER_DEPS
#define _LOADER_LOAD
#define POOLING
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
#if POOLING
#if LOG_WHEN_ERROR_POOL_TYPE
            tester.AddLogger(this, "pool type error", "error: A wrong pooling revert, instance type is {0}");
#endif
#endif
#if LOG_DIR_FROM_DEVICE_LOCAL_CHECKED
            tester.AddLogger(this, LOG2, "log: Asset is from priv: {0}, path key is: {1}");
            tester.AddLogger(this, LOG3, "log: Asset path is : {0}");
#endif
#if LOADER
#if LOADER_FAILED
            tester.AddLogger(this, "loader failed", "error: Loader load failed, Url is {0}");
#endif
#if LOADER_SUCCESS
            tester.AddLogger(this, "loader success", "log: Loader successed: {0}");
#endif
#if LOADER_DEPS
            tester.AddLogger(this, "loader deps", "log: Loader complete and get dependency: {0}");
            tester.AddLogger(this, "empty deps", "warning: : dependences is empty");
            tester.AddLogger(this, "deps", "log: Dependence: {0} => {1}");
            tester.AddLogger(this, "walk deps", "log: Walk dependences will out of stacks");
#endif
#if LOADER_LOAD
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

            tester.AddAsserter("framework start", false, "Welcom..", "Managers Ready", "Ticks Ready", "Framework Started");
        }

        public string Name { get; set; }
    }
}