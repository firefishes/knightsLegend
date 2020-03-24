using ShipDock.Applications;
using System;

namespace KLGame
{
    public interface IKLRole : ICommonRole
    {
        void UnderAttack();
        void StartTimingTask(int name, float time, Action completion = default);
        TimingTaskEntitas TimesEntitas { get; }
        KLProcessComponent Processing { get; }
    }

}