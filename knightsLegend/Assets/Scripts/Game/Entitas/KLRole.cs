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

            Utils.Reclaim(TimesEntitas);

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

            TimesEntitas = new TimingTaskEntitas();
        }

        protected virtual void OnRoleNotificationHandler(INoticeBase<int> obj)
        {
        }

        protected override void SetRoleInputInfo()
        {
            base.SetRoleInputInfo();

            RoleInput.FullRoleInputPhases = new List<int>()
            {
                UserInputPhases.ROLE_INPUT_PHASE_UNDERATTACKED
            };
        }

        public void StartTimingTask(int name, float time, Action completion = default)
        {
            TimingTasker roleTime = TimesEntitas.GetRoleTiming(name);
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

        public abstract int RoleFSMName { get; }
        public KLProcessComponent Processing { get; private set; }
        public TimingTaskEntitas TimesEntitas { get; private set; }
        public bool HitSomeOne { get; set; }
        public Vector3 WeapontPos { get; set; }
    }
}
