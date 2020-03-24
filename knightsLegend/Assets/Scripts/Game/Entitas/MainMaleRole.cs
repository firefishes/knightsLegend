using ShipDock.Applications;
using ShipDock.ECS;
using ShipDock.Tools;
using System;
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

    public interface IAIRole : IKLRole
    {
        void SetShouldAtkAIWork(bool value);
        bool ShouldAtkAIWork { get; }
        bool InATKCycle { get; set; }
        bool IsInitNormalATKPhases { get; set; }
    }

    public class RoleTime
    {
        public float dTime;
        public TimeGapper timeGapper;
        public Action completion;

        public RoleTime(int name, ref KeyValueList<int, RoleTime> mapper)
        {
            timeGapper = new TimeGapper();
            mapper[name] = this;
            completion += UpdateRunCounts;
            Name = name;
        }

        public void ResetRunCounts()
        {
            RunCounts = 0;
        }

        public void DirectCompletion()
        {
            completion?.Invoke();
        }

        private void UpdateRunCounts()
        {
            RunCounts++;
        }

        public bool IsStart
        {
            get
            {
                return timeGapper.isStart;
            }
        }

        public int RunCounts { get; private set; }
        public int Name { get; private set; }
    }

    public static class RoleTimeNames
    {
        public const int NORMAL_ATK_TIME = 0;
        public const int NORMAL_ATK_HIT_TIME = 1;
    }
    
    public class RoleTimesEntitas : EntitasComponentable
    {
        private KeyValueList<int, RoleTime> mRoleTimeMapper;

        public RoleTimesEntitas() : base()
        {
            InitComponents();

            mRoleTimeMapper = new KeyValueList<int, RoleTime>();
            new RoleTime(RoleTimeNames.NORMAL_ATK_TIME, ref mRoleTimeMapper);
            new RoleTime(RoleTimeNames.NORMAL_ATK_HIT_TIME, ref mRoleTimeMapper);
        }

        public RoleTime GetRoleTime(int name)
        {
            return mRoleTimeMapper[name];
        }

        public void UpdateAllTimes(float dTime, ref List<RoleTime> allRoleTimes, ref RoleTime item)
        {
            bool flag;
            allRoleTimes = mRoleTimeMapper.Values;
            int max = allRoleTimes.Count;
            for (int i = 0; i < max; i++)
            {
                item = allRoleTimes[i];
                if(item.IsStart)
                {
                    flag = item.timeGapper.TimeAdvanced(dTime);
                    if(flag)
                    {
                        item.completion?.Invoke();
                    }
                }
            }
        }

        protected override int[] ComponentIDs { get; } = new int[]
        {
            KLConsts.C_ROLE_TIMES
        };
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

        protected override void SetRoleInputInfo()
        {
            base.SetRoleInputInfo();

            RoleInput.RoleInputType = KLConsts.ROLE_INPUT_TYPE_ENEMY;
        }

        public override void AfterGetStopDistance(float dist, Vector3 entitasPos)
        {
            base.AfterGetStopDistance(dist, entitasPos);

            if(RoleInput.RoleInputPhase == UserInputPhases.ROLE_INPUT_PHASE_NONE)
            {
                return;
            }

            if (!ShouldAtkAIWork)
            {
                SetShouldAtkAIWork(true);
                RoleInput.SetInputPhase(EnemyInputPhases.ENEMY_INPUT_PHASE_SET_NROMAL_ATK_TRIGGER_TIME);
            }
        }

        public void SetATKID(int value)
        {
            ATKID = value;
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

        public override float GetStopDistance()
        {
            return 2.5f;
        }

        public int ATKID { get; private set; }
        public bool ShouldAtkAIWork { get; private set; }
        public bool InATKCycle { get; set; }
        public bool IsInitNormalATKPhases { get; set; }
    }
}
