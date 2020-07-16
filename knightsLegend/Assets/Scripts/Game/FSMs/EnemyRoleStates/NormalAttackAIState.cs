#define G_LOG

using ShipDock.Applications;
using ShipDock.FSM;
using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Testers;
using System;
using System.Collections.Generic;

namespace KLGame
{
    public class NormalAIState : FState, IAIState
    {
        private TimingTasker mThinkingTime;
        private MethodUpdater mStateUpdater;
        private IKLRoleFSMAIParam mStateParam;

        public NormalAIState(int name) : base(name)
        {
            mStateUpdater = new MethodUpdater
            {
                Update = OnStateUpdate
            };
            PositionComponent = (KLPositionComponent)ShipDockApp.Instance.Components.RefComponentByName(KLConsts.C_POSITION);
        }

        public override void InitState(IStateParam param = null)
        {
            base.InitState(param);

            mStateParam = param as IKLRoleFSMAIParam;
            AIRole = mStateParam.KLRole as IAIRole;
            RoleSceneComp = mStateParam.RoleSceneComp;

            if (mThinkingTime == default)
            {
                TimingTaskEntitas timingTaskEntitas = AIRole.TimesEntitas;
                mThinkingTime = timingTaskEntitas.GetTimingTasker(KLConsts.T_AI_THINKING, KLConsts.T_AI_THINKING_TIME_TASK_AI_WAITED);
                //mNormalATKTime = timingTaskEntitas.GetTimingTasker(KLConsts.T_AI_ATK_TIME, KLConsts.T_AI_THINKING_TIME_TASK_ATK);
                //mThinkingTime.completion += ExecuteNormalAtk;
            }
            mThinkingTime.ResetRunCounts();

            UpdaterNotice.AddUpdater(mStateUpdater);
        }

        public override void DeinitState()
        {
            base.DeinitState();

            Notice notice = Pooling<Notice>.From();
            RoleSceneComp.Dispatch(KLConsts.N_AI_RESET, notice);
            notice.ToPool();

            UpdaterNotice.RemoveUpdater(mStateUpdater);

            mThinkingTime.Stop();
            mStateParam?.ToPool();
            mStateParam = default;
            AIRole = default;
            RoleSceneComp = default;

        }

        private void ExecuteNormalAtk()
        {
            Tester.Instance.Log(TesterRPG.Instance, TesterRPG.LOG, "log: Enemy executeNormalAtk");
            mThinkingTime.Reset();

            //AIRole.SetShouldAtkAIWork(true);

            //float time = mNormalATKTime.RunCounts == 0 ? 0.1f : new System.Random().Next(1, 5);
            //mNormalATKTime.Start(time);
        }

        private void Atked()
        {
            Tester.Instance.Log(TesterRPG.Instance, TesterRPG.LOG, "log: Enemy Atked");

            if(AIRole != default)// && AIRole.ShouldAtkAIWork)
            {
                (AIRole.RoleInput as IRPGRoleInput).SetInputPhase(KLConsts.ENEMY_INPUT_PHASE_AFTER_NROMAL_ATK);
                Tester.Instance.Log(TesterRPG.Instance, TesterRPG.LOG, "log: Enemy phase ".Append((AIRole.RoleInput as IRPGRoleInput).RoleInputPhase.ToString()));
            }
        }

        private void OnStateUpdate(int time)
        {
            if (AIRole != default)
            {
                if (AIRole.FindingPath)
                {
                    Notice notice = Pooling<Notice>.From();
                    RoleSceneComp.Dispatch(KLConsts.N_AI_RESET, notice);
                    notice.ToPool();

                    //ChangeToPreviousState();

                    //TODO 这里要处理重置AI思考后不要再次进入这个状态，否则会出现 1->4，4->1 的情况
                }
            }
        }

        public void AIConduct()
        {
            ICommonRole role = AIRole as ICommonRole;
            bool isStoped = PositionComponent.IsEntitasStoped(ref role);
            if (isStoped)
            {
                float distance = role.GetStopDistance();
                List<SkillConduct> conducts = mStateParam.SkillMapper.GetDistanceNearestSkill(distance, MotionSceneInfo.CATEHORY_ATK);

                Random random = new Random();
                int index = random.Next(0, conducts.Count);
                SkillConduct conduct = conducts[index];
                index = random.Next(0, conduct.timingTasks.Length);
                AIRole.ConductTimingTask = conduct.timingTasks[index];
            }
            else
            {
                mThinkingTime.ResetRunCounts();
            }
        }

        public KLPositionComponent PositionComponent { get; private set; }
        public IAIRole AIRole { get; set; }
        public IKLRoleSceneComponent RoleSceneComp { get; private set; }
    }

}