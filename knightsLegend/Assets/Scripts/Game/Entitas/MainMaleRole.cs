using ShipDock.Applications;
using ShipDock.ECS;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class MainMaleRole : KLRole
    {
        public MainMaleRole()
        {
            IRoleData data = KLRoleData.GetRoleDataByRandom();
            data.Speed = 18f;
            data.ConfigID = 0;
            SetRoleData(data);
            
            IsUserControlling = true;
            PositionEnabled = false;

            Camp = 0;
        }
    }

    public interface IAIRole : ICommonRole
    {
        void SetNormalATKTriggerTime(float time);
        void SetATKID(int value);
        void SetShouldAtkAIWork(bool value);
        bool ShouldAtkAIWork { get; }
        int ATKID { get; }
        bool InATKCycle { get; set; }
        float NormalATKTriggerTime { get; }
    }

    public class EnmeyRole : KLRole, IAIRole
    {

        private int[] mComponentIDs;

        public EnmeyRole()
        {
            IRoleData data = KLRoleData.GetRoleDataByRandom();
            data.ConfigID = 1;
            SetRoleData(data);

            IsUserControlling = false;
            PositionEnabled = true;

            Camp = 1;
        }

        public override void AfterGetStopDistance(float dist, Vector3 entitasPos)
        {
            base.AfterGetStopDistance(dist, entitasPos);

            ShouldAtkAIWork = true;
        }

        public void SetATKID(int value)
        {
            ATKID = value;
        }

        public void SetNormalATKTriggerTime(float time)
        {
            NormalATKTriggerTime = time;
        }

        public void SetShouldAtkAIWork(bool value)
        {
            ShouldAtkAIWork = value;
        }

        protected override int[] ComponentIDs
        {
            get
            {
                if (mComponentIDs == default)
                {
                    base.ComponentIDs.ContactToArr(new int[] { KLConsts.C_ROLE_AI_ATK }, out mComponentIDs);
                }
                return mComponentIDs;
            }
        }

        public int ATKID { get; private set; }
        public bool ShouldAtkAIWork { get; private set; }
        public float NormalATKTriggerTime { get; private set; }
        public bool InATKCycle { get; set; }
    }
}
