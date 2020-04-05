using ShipDock.Applications;
using ShipDock.Notices;
using System;
using UnityEngine;

namespace KLGame
{
    public interface IKLRole : ICommonRole, INotificationSender
    {
        void UnderAttack();
        void StartTimingTask(int name, float time, Action completion = default);
        TimingTaskEntitas TimesEntitas { get; }
        KLProcessComponent Processing { get; }
        bool HitSomeOne { get; set; }
        Vector3 WeapontPos { get; set; }
    }

}