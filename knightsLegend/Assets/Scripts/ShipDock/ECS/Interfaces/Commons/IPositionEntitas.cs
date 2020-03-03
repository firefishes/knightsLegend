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
        void SetPahterTarget(Vector3 value);
        Vector3 PatherTargetPosition { get; }
        bool FindngPath { get; set; }
    }

    public interface ICollidableRole
    {
        List<int> CollidingRoles { get; }
    }

    public interface IStatesRole
    {
        int[] States { get; }
    }
}
