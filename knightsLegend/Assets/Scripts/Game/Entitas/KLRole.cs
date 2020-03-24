using ShipDock.Applications;
using System;

namespace KLGame
{
    public interface IKLRole : ICommonRole
    {
        void UnderAttack();
        void StartTimeEntitasGapper(int name, float time, Action completion = default);
        RoleTimesEntitas TimesEntitas { get; }
    }

    public class KLRole : RoleEntitas, IKLRole
    {

        public override void InitComponents()
        {
            base.InitComponents();

            TimesEntitas = new RoleTimesEntitas();
        }

        public void StartTimeEntitasGapper(int name, float time, Action completion = default)
        {
            RoleTime roleTime = TimesEntitas.GetRoleTime(name);
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

        protected override int[] ComponentIDs { get; } = new int[]
        {
            KLConsts.C_ROLE_INPUT,
            KLConsts.C_ROLE_MOVE,
            KLConsts.C_POSITION,
            KLConsts.C_ROLE_COLLIDER,
            KLConsts.C_ROLE_MUST,
            KLConsts.C_ROLE_CAMP
        };

        public RoleTimesEntitas TimesEntitas { get; private set; }
    }
}
