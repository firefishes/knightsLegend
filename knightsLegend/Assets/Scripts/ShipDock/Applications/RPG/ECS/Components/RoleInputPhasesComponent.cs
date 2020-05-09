#define G_LOG
#define LOG_TRY_CATCH_ROLE

using ShipDock.ECS;
using ShipDock.Notices;
using ShipDock.Tools;

namespace ShipDock.Applications
{

    public class RoleInputPhasesComponent : ShipDockComponent, INotificationSender
    {
        protected ICommonRole mRole;
        protected IRoleInput mRoleInput;
        protected IRoleData mRoleData;

        private KeyValueList<int, int> mAllowCalleds;

        public override void Init()
        {
            base.Init();

            mAllowCalleds = new KeyValueList<int, int>();
        }

        public void AddAllowCalled(int phaseName, int allowCalled = 1)
        {
            mAllowCalleds[phaseName] = allowCalled;
        }

        public override void Execute(int time, ref IShipDockEntitas target)
        {
            base.Execute(time, ref target);

            mRole = target as ICommonRole;
#if LOG_TRY_CATCH_ROLE && UNITY_EDITOR
            try
            {
#endif
            mRoleInput = mRole.RoleInput;
#if LOG_TRY_CATCH_ROLE && UNITY_EDITOR
            }
            catch (System.Exception error)
            {
                Testers.Tester.Instance.Log(TesterRPG.Instance, TesterRPG.LOG, "error:".Append(error.Message));
            }
#endif

            if (mRoleInput == default)
            {
                return;
            }

            int phaseName = mRoleInput.RoleInputPhase;
            switch (phaseName)
            {
                default:
                    int allowCalled = mAllowCalleds.ContainsKey(phaseName) ? mAllowCalleds[phaseName] : 0;
                    mRoleInput.AdvancedInputPhase(phaseName, allowCalled);
                    break;
            }
        }

        protected virtual void InitRolePhases(IRoleInput roleInput)
        {
        }
    }
}