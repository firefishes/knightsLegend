﻿using ShipDock.Applications;
using ShipDock.Notices;
using System;
using UnityEngine;

namespace KLGame
{
    public interface IKLRole : ICommonRole, INotificationSender
    {
        void SetBattleUnit(BattleUnit battleUnit);
        void UnderAttack();
        void StartTimingTask(int name, int mapperIndex, float time, Action completion = default);
        TimingTaskEntitas TimesEntitas { get; }
        KLProcessComponent Processing { get; }
        bool HitSomeOne { get; set; }
        Vector3 WeapontPos { get; set; }
        CommonRoleFSM RoleFSM { get; }
        BattleUnit BattleDataUnit { get; }
    }

}