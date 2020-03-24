using ShipDock.Applications;
using System;

namespace KLGame
{
    public class KLRole : RoleEntitas, IKLRole
    {

        public override void InitComponents()
        {
            base.InitComponents();

            Processing = ShipDockApp.Instance.Components.GetComponentByAID(KLConsts.C_PROCESS) as KLProcessComponent;

            TimesEntitas = new TimingTaskEntitas();
        }

        public void StartTimingTask(int name, float time, Action completion = default)
        {
            TimingTasker roleTime = TimesEntitas.GetRoleTime(name);
            if (roleTime != default)
            {
                if (completion != default)
                {
                    roleTime.completion += completion;
                }
                roleTime.timeGapper.totalTime = time;
                roleTime.timeGapper.Start();
            }
        }

        public void UnderAttack()
        {
            RoleInput.SetInputPhase(UserInputPhases.ROLE_INPUT_PHASE_UNDERATTACKED);
        }

        public KLProcessComponent Processing { get; private set; }

        protected override int[] ComponentIDs { get; } = new int[]
        {
            KLConsts.C_PROCESS,
            KLConsts.C_ROLE_INPUT,
            KLConsts.C_ROLE_MOVE,
            KLConsts.C_POSITION,
            KLConsts.C_ROLE_COLLIDER,
            KLConsts.C_ROLE_MUST,
            KLConsts.C_ROLE_CAMP
        };

        public TimingTaskEntitas TimesEntitas { get; private set; }
    }
}
