using ShipDock.Applications;
using ShipDock.FSM;
using ShipDock.Notices;
using ShipDock.Pooling;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class AIBrain : IAIBrain
    {

        public void Clean()
        {
            Sencor = default;
            Anticipathioner = default;
            PolicyAnalyzer = default;
            RoleFSM = default;
        }

        public void InitAIBrain(ISensor sencor, IAIBehavioralInfo anticipathioner, IAIBehavioralInfo policyAnalyzer)
        {
            Sencor = sencor;
            Anticipathioner = anticipathioner;
            PolicyAnalyzer = policyAnalyzer;
        }

        public void SetRole(IKLRole role)
        {
            RoleFSM = role.RoleFSM;

            TimingTaskEntitas taskEntitas = role.TimesEntitas;
            taskEntitas.AddTiming(KLConsts.T_AI_THINKING, KLConsts.T_AI_THINKING_TIME_TASK_ATK);
            taskEntitas.AddTiming(KLConsts.T_AI_THINKING, KLConsts.T_AI_THINKING_TIME_TASK_DEF);
            taskEntitas.AddTiming(KLConsts.T_AI_THINKING, KLConsts.T_AI_THINKING_TIME_TASK_AI_WAITED);

            TimingTasker timingTasker = taskEntitas.GetTimingTasker(KLConsts.T_AI_THINKING, KLConsts.T_AI_THINKING_TIME_TASK_ATK);
            timingTasker.TotalCount = 1;
            timingTasker.completion += OnNormalAtk;
            timingTasker.completion += timingTasker.Reset;

            timingTasker = taskEntitas.GetTimingTasker(KLConsts.T_AI_THINKING, KLConsts.T_AI_THINKING_TIME_TASK_DEF);
            timingTasker.completion += OnNormalDef;

            timingTasker = taskEntitas.GetTimingTasker(KLConsts.T_AI_THINKING, KLConsts.T_AI_THINKING_TIME_TASK_AI_WAITED);
            timingTasker.TotalCount = 1;
            timingTasker.completion += OnAIWaited;
            timingTasker.completion += timingTasker.Reset;
        }

        public void ExecuteDecision(IAIRole roleATkAI, IKLRoleSceneComponent roleSceneComponent)
        {
            IAIBehavioralInfo behavioralInfo = roleATkAI.Anticipathioner;
            AIStateWillChange stateWill = behavioralInfo.AIStateWillChange;
            if (stateWill != default)
            {
                roleSceneComponent.SetCurrentSkillID(stateWill.SkillID);
                stateWill.RoleFSMParam?.Reinit(roleSceneComponent, stateWill.Inputs);
                RoleFSM.ChangeState(stateWill.StateWill, stateWill.RoleFSMParam);
                stateWill.ToPool();
                behavioralInfo.AIStateWillChange = default;
            }
            else
            {
                behavioralInfo = roleATkAI.PolicyAnalyzer;
                stateWill = behavioralInfo.AIStateWillChange;

            }
            behavioralInfo.IsExecuted = false;
        }

        private void OnAIWaited()
        {
            if (RoleFSM.Current is IAIState state)
            {
                state.AIConduct();
            }
        }

        private void OnNormalAtk()
        {
            if (Anticipathioner != default)
            {
                if (Anticipathioner.StateFrom == int.MaxValue && Anticipathioner.AIStateWillChange == default)
                {
                    PolicyAnalyzer.AIStateWillChange = new AIStateWillChange
                    {
                        SkillID = 1,
                        Inputs = new int[] { 1 },
                        StateWill = NormalRoleStateName.NORMAL_ATK,
                        RoleFSMParam = Pooling<NormalATKStateParam>.From()
                    };
                    //ConductTimingTask = int.MaxValue;
                }
            }
        }

        private void OnNormalDef()
        {
            if (Anticipathioner != default)
            {
                Anticipathioner.StateFrom = int.MaxValue;
                Anticipathioner.AIStateWillChange = new AIStateWillChange
                {
                    SkillID = 3,
                    StateWill = NormalRoleStateName.NORMAL_DEF,
                    RoleFSMParam = Pooling<KLRoleFSMStateParam>.From()
                };
            }
        }

        private ISensor Sencor { get; set; }
        private IStateMachine RoleFSM { get; set; }

        public IAIBehavioralInfo Anticipathioner { get; private set; }
        public IAIBehavioralInfo PolicyAnalyzer { get; private set; }

    }

}