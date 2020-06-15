using ShipDock.Applications;
using ShipDock.Notices;
using System;
using UnityEngine;

namespace KLGame
{
    public interface IKLWorldItem : IGoalIssuer, IWorldStateIssuer
    {
        bool HasAddToWorld { get; }
    }

    public interface IKLRole : ICommonRole, INotificationSender, IKLWorldItem
    {
        void SetStopDistance(float distance);
        void SetBattleUnit(BattleUnit battleUnit);
        void UnderAttack();
        void StartTimingTask(int name, int mapperIndex, float time, Action completion = default);
        
        TimingTaskEntitas TimesEntitas { get; }
        KLProcessComponent Processing { get; }
        KLWorldStatesComponent WorldStates { get; }
        Vector3 WeapontPos { get; set; }
        CommonRoleFSM RoleFSM { get; }
        BattleUnit BattleDataUnit { get; }
        Quaternion CurQuaternaion { get; set; }
        bool HitSomeOne { get; set; }
        bool IsDead { get; set; }
        int DefenceType { get; set; }
    }
    
    public interface IMainRole : IKLRole
    {
    }
    
}