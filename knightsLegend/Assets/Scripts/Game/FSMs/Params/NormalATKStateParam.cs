using ShipDock.Applications;
using ShipDock.Pooling;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{

    public class KLRoleFSMStateParam : IKLRoleFSMParam
    {
        public void Revert()
        {
            Inpunts.Clear();
            RoleSceneComp = default;
            KLRole = default;
            StartPos = Vector3.zero;
            StartRotation = Quaternion.identity;
        }

        public void Reinit(IKLRoleSceneComponent comp, params int[] inputs)
        {
            int max = inputs.Length;
            for (int i = 0; i < max; i++)
            {
                Inpunts.Enqueue(inputs[i]);
            }
            RoleSceneComp = comp;

            FillValues();
        }

        public void FillValues()
        {
            RoleSceneComp?.FillRoleFSMStateParam(this);
        }

        public virtual void Clean()
        {
            Pooling<KLRoleFSMStateParam>.To(this);
        }

        public Queue<int> Inpunts { get; set; } = new Queue<int>();
        public IKLRoleSceneComponent RoleSceneComp { get; set; }
        public IKLRole KLRole { get; set; }
        public int CurrentSkillID { get; set; }
        public Vector3 StartPos { get; set; }
        public Quaternion StartRotation { get; set; }
        public SkillMotionsMapper SkillMapper { get; set; }

    }

    public class NormalATKStateParam : KLRoleFSMStateParam
    {
    }
}