using ShipDock.Applications;
using ShipDock.FSM;
using ShipDock.Pooling;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public interface IKLRoleFSMParam : IStateParam, IPoolable
    {
        void FillValues();
        Queue<int> Inpunts { get; set; }
        IKLRoleSceneComponent RoleSceneComp { get; set; }
        IKLRole KLRole { get; set; }
        int CurrentSkillID { get; set; }
        Vector3 StartPos { get; set; }
        Quaternion StartRotation { get; set; }
        SkillMotionsMapper SkillMapper { get; set; }
    }
}