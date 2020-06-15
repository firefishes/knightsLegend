using ShipDock.Applications;
using ShipDock.FSM;
using ShipDock.Pooling;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public interface IKLRoleFSMParam : IKLRoleFSMAIParam
    {
        void Reinit(IKLRoleSceneComponent comp, params int[] inputs);
        Queue<int> Inpunts { get; set; }
        int CurrentSkillID { get; set; }
        Vector3 StartPos { get; set; }
        Quaternion StartRotation { get; set; }
    }

    public interface IKLRoleFSMAIParam : IStateParam, IPoolable
    {
        void FillValues();
        SkillMotionsMapper SkillMapper { get; set; }
        IKLRoleSceneComponent RoleSceneComp { get; set; }
        IKLRole KLRole { get; set; }
    }

    public class KLRoleFSMAIStateParam : IKLRoleFSMAIParam
    {

        public void Reinit(IKLRoleSceneComponent comp)
        {
            RoleSceneComp = comp;
            FillValues();
        }

        public void FillValues()
        {
            RoleSceneComp?.FillRoleFSMAIStateParam(this);
        }

        public void Revert()
        {
            RoleSceneComp = default;
            KLRole = default;
        }

        public void ToPool()
        {
            Pooling<KLRoleFSMAIStateParam>.To(this);
        }

        public IKLRoleSceneComponent RoleSceneComp { get; set; }
        public IKLRole KLRole { get; set; }
        public SkillMotionsMapper SkillMapper { get; set; }
    }
}