using ShipDock.Applications;
using ShipDock.ECS;
using ShipDock.Notices;
using ShipDock.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public abstract class KLRole : RoleEntitas, IKLRole
    {

        public override void Dispose()
        {
            base.Dispose();

            this.Remove(OnRoleNotificationHandler);

            TimesEntitas?.ToPool();
            TimesEntitas = default;
            Processing = default;
            CollidingChanger = default;
        }

        public override void InitComponents()
        {
            base.InitComponents();

            this.Add(OnRoleNotificationHandler);

            ShipDockComponentManager components = ShipDockApp.Instance.Components;
            Processing = components.GetComponentByAID(KLConsts.C_PROCESS) as KLProcessComponent;

            TimesEntitas = TimingTaskEntitas.Create();
            TimesEntitas.CreateMapper();
            TimesEntitas.AddTiming(KLConsts.T_AI_ATK_TIME, 0);
            TimesEntitas.AddTiming(KLConsts.T_AI_ATK_HIT_TIME, 0);
        }

        protected virtual void OnRoleNotificationHandler(INoticeBase<int> param)
        {
            switch(param.Name)
            {
                case KLConsts.N_INIT_ENTITAS_CALLBACKS:

                    break;
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

        protected override int[] ComponentIDs { get; } = new int[]
        {
            KLConsts.C_PROCESS,
            KLConsts.C_ROLE_INPUT,
            KLConsts.C_ROLE_MOVE,
            KLConsts.C_POSITION,
            KLConsts.C_ROLE_COLLIDER,
            KLConsts.C_ROLE_MUST,
            KLConsts.C_ROLE_CAMP
        };

        public abstract int RoleFSMName { get; set; }
        public KLProcessComponent Processing { get; private set; }
        public TimingTaskEntitas TimesEntitas { get; private set; }
        public bool HitSomeOne { get; set; }
        public Vector3 WeapontPos { get; set; }
        public CommonRoleFSM RoleFSM { get; protected set; }
        public RoleFSMObj FSMStates { get; set; }
    }
}
