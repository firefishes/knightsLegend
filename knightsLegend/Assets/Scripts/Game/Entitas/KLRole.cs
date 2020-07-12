using ShipDock.Applications;
using ShipDock.ECS;
using ShipDock.Notices;
using ShipDock.Tools;
using System;
using UnityEngine;

namespace KLGame
{
    public abstract class KLRole : RoleEntitas, IKLRole
    {

        public KLRole() : base()
        {
            IsStartTrancking = false;
        }

        public override void Dispose()
        {
            base.Dispose();

            this.Remove(OnRoleNotificationHandler);

            Utils.Reclaim(BattleDataUnit);

            RemoveFromWorld();

            TimesEntitas?.ToPool();
            TimesEntitas = default;
            Processing = default;
            WorldStates = default;
            CollidingChanger = default;
            BattleDataUnit = default;
            HasAddToWorld = false;
        }

        public override void InitComponents()
        {
            base.InitComponents();

            this.Add(OnRoleNotificationHandler);

            ShipDockComponentManager components = ShipDockApp.Instance.Components;
            Processing = components.RefComponentByName(KLConsts.C_PROCESS) as KLProcessComponent;
            WorldStates = components.RefComponentByName(KLConsts.C_WORLD_STATES) as KLWorldStatesComponent;

            TimesEntitas = TimingTaskEntitas.Create();
            TimesEntitas.CreateMapper();
            //TimesEntitas.AddTiming(KLConsts.T_AI_ATK_TIME, 0);
            //TimesEntitas.AddTiming(KLConsts.T_AI_ATK_HIT_TIME, 0);

            AddToWorld();
        }

        protected virtual void OnRoleNotificationHandler(INoticeBase<int> param)
        {
            //switch(param.Name)
            //{
            //    case KLConsts.N_INIT_ENTITAS_CALLBACKS:

            //        break;
            //}
            switch (param.Name)
            {
                case KLConsts.N_ROLE_ADD_TO_WORLD:
                    AddToWorld();
                    break;
                case KLConsts.N_ROLE_TIMING:
                    TimingControll(param as TimingTaskNotice);
                    break;
            }
        }

        private void AddToWorld()
        {
            if (HasAddToWorld)
            {
                return;
            }
            HasAddToWorld = true;

            OnAddToWorld();
        }

        protected virtual void OnAddToWorld()
        {
            WorldStates?.AddWorldGoals(this);
            WorldStates?.AddWorldStates(this);
        }

        private void RemoveFromWorld()
        {
            if (HasAddToWorld)
            {
                OnRemoveFormWorld();
            }
        }

        protected virtual void OnRemoveFormWorld()
        {
            WorldStates?.RemoveWorldGoals(this);
            WorldStates?.RemoveWorldStates(this);
        }

        private void TimingControll(TimingTaskNotice notice)
        {
            int name = notice.Name;
            TimingTasker timingTasker = TimesEntitas.GetTimingTasker(notice.TimingName, notice.MapperIndex);

            if (timingTasker != default)
            {
                if (notice.IsStart)
                {
                    timingTasker.Start(notice.Time);
                }
                else if(notice.IsReset)
                {
                    timingTasker.Reset();
                }
                else
                {
                    timingTasker.SetStateToFinish();
                }
            }
        }

        protected override void SetRoleInputInfo()
        {
            base.SetRoleInputInfo();

            //RoleInput.FullRoleInputPhases = new List<int>()
            //{
            //    UserInputPhases.ROLE_INPUT_PHASE_UNDERATTACKED
            //};
        }

        public void StartTimingTask(int name, int mapperIndex, float time, Action completion = default)
        {
            TimingTasker roleTime = TimesEntitas.GetTimingTasker(name, mapperIndex);
            if (roleTime != default)
            {
                if (completion != default)
                {
                    roleTime.completion += completion;
                }
                roleTime.timeGapper.totalTime = time;
                roleTime.timeGapper.Start();
            }
        }

        public void UnderAttack()
        {
            RoleInput.SetInputPhase(UserInputPhases.ROLE_INPUT_PHASE_UNDERATTACKED);
        }

        public override void CollidingChanged(int colliderID, bool isTrigger, bool isCollided)
        {
            CollidingChanger?.Invoke(ID, colliderID, isTrigger, isCollided);
        }

        public void SetBattleUnit(BattleUnit battleUnit)
        {
            BattleDataUnit = battleUnit;
        }

        public void SetStopDistance(float distance)
        {
            StopDistance = distance;
        }
        
        protected override int[] ComponentNames { get; } = new int[]
        {
            KLConsts.C_PROCESS,
            KLConsts.C_ROLE_INPUT,
            //KLConsts.C_ROLE_MOVE,
            KLConsts.C_POSITION,
            KLConsts.C_ROLE_COLLIDER,
            KLConsts.C_ROLE_MUST,
            KLConsts.C_ROLE_CAMP,
            KLConsts.C_ROLE_BATTLE_DATA,
            KLConsts.C_WORLD_STATES,
        };
        
        protected float StopDistance { get; set; } = 1.5f;

        public abstract int RoleFSMName { get; set; }
        public KLProcessComponent Processing { get; private set; }
        public KLWorldStatesComponent WorldStates { get; private set; }
        public TimingTaskEntitas TimesEntitas { get; private set; }
        public bool HitSomeOne { get; set; }
        public Vector3 WeapontPos { get; set; }
        public CommonRoleFSM RoleFSM { get; protected set; }
        public RoleFSMObj FSMStates { get; set; }
        public BattleUnit BattleDataUnit { get; private set; }
        public Quaternion CurQuaternaion { get; set; } = Quaternion.identity;
        public bool IsDead { get; set; } = false;
        public int DefenceType { get; set; } = 0;
        public bool HasAddToWorld { get; private set; }
        public virtual IGoal[] ProvideGoals { get; protected set; } = default;
        public virtual IWorldState[] ProvideWorldStates { get; protected set; } = default;
    }
}
