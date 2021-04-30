﻿using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.ECS
{
    public interface IPositionEntitas
    {
        float Speed { get; set; }
        float SpeedCurrent { get; set; }
        Vector3 PostionTarget { get; set; }
        Vector3 Direction { get; set; }
        Vector3 Position { get; set; }
        bool PositionEnabled { get; set; }
    }

    public interface IPathFindable : IPositionEntitas
    {
        float GetStopDistance();
        bool AfterGetStopDistance(float dist, Vector3 entitasPos);
        void SetPahterTarget(Vector3 value);
        Vector3 PatherTargetPosition { get; }
        bool FindingPath { get; set; }
        bool AfterGetStopDistChecked { get; set; }
    }

    public interface ICollidableRole
    {
        void CollidingChanged(int colliderID, bool isTrigger, bool isCollided);
        Action<int, int, bool, bool> CollidingChanger { get; set; }
        List<int> CollidingRoles { get; }
    }

    public interface IStatesRole
    {
        int[] States { get; }
    }
}
