using ShipDock.Applications;
using ShipDock.FSM;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class NormalAttackAIState : FState
    {
        private TimingTasker mNormalATKTime;
        private IKLRoleFSMAIParam mStateParam;
        private MethodUpdater mStateUpdater;

        public NormalAttackAIState(int name) : base(name)
        {
            mStateUpdater = new MethodUpdater
            {
                Update = OnStateUpdate
            };
        }

        public override void InitState(IStateParam param = null)
        {
            base.InitState(param);

            Debug.Log("ai atk state");

            mStateParam = param as IKLRoleFSMAIParam;

            AIRole = mStateParam.KLRole as IAIRole;
            AIRole.SetShouldAtkAIWork(true);

            mNormalATKTime = AIRole.TimesEntitas.GetRoleTiming(RoleTimingTaskNames.NORMAL_ATK_TIME);
            AIRole.RoleInput.NextPhase();
            float time = mNormalATKTime.RunCounts == 0 ? 0.1f : UnityEngine.Random.Range(1f, 5f);
            AIRole.StartTimingTask(RoleTimingTaskNames.NORMAL_ATK_TIME, time);

            UpdaterNotice.AddUpdater(mStateUpdater);
        }

        public override void DeinitState()
        {
            base.DeinitState();

            AIRole.SetShouldAtkAIWork(false);

            mStateParam?.Clean();
            mStateParam = default;
            AIRole = default;
            mNormalATKTime = default;

            UpdaterNotice.RemoveUpdater(mStateUpdater);
        }

        private void OnStateUpdate(int time)
        {
            if(AIRole != default)
            {
                if (AIRole.FindngPath)
                {
                    ChangeToPreviousState();
                }
            }
        }

        public IAIRole AIRole { get; set; }
    }

}