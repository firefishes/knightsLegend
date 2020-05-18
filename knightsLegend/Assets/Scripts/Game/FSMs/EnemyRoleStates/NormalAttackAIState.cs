#define G_LOG

using ShipDock.Applications;
using ShipDock.FSM;
using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.Testers;
using UnityEngine;

namespace KLGame
{
    public class NormalAttackAIState : FState
    {
        private TimingTasker mThinkingTime;
        private TimingTasker mNormalATKTime;
        private MethodUpdater mStateUpdater;
        private IKLRoleFSMAIParam mStateParam;

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

            mStateParam = param as IKLRoleFSMAIParam;
            AIRole = mStateParam.KLRole as IAIRole;
            RoleSceneComp = mStateParam.RoleSceneComp;

            if (mThinkingTime == default)
            {
                TimingTaskEntitas timingTaskEntitas = AIRole.TimesEntitas;
                mThinkingTime = timingTaskEntitas.GetTimingTasker(KLConsts.T_AI_THINKING, 0);
                mNormalATKTime = timingTaskEntitas.GetTimingTasker(KLConsts.T_AI_ATK_TIME, 0);

                mThinkingTime.TotalCount = 1;
                mThinkingTime.completion += ExecuteNormalAtk;
                mNormalATKTime.TotalCount = 1;
                mNormalATKTime.completion += Atked;
            }

            mThinkingTime.Start(0.2f);

            UpdaterNotice.AddUpdater(mStateUpdater);
        }

        public override void DeinitState()
        {
            base.DeinitState();

            UpdaterNotice.RemoveUpdater(mStateUpdater);

            mThinkingTime.Stop();
            mNormalATKTime.Stop();

            AIRole.SetShouldAtkAIWork(false);

            mStateParam?.ToPool();
            mStateParam = default;
            AIRole = default;
            RoleSceneComp = default;

        }

        private void ExecuteNormalAtk()
        {
            Tester.Instance.Log(TesterRPG.Instance, TesterRPG.LOG, "log: Enemy executeNormalAtk");
            mThinkingTime.Reset();

            AIRole.SetShouldAtkAIWork(true);

            float time = mNormalATKTime.RunCounts == 0 ? 0.1f : new System.Random().Next(1, 5);
            mNormalATKTime.Start(time);
        }

        private void Atked()
        {
            Tester.Instance.Log(TesterRPG.Instance, TesterRPG.LOG, "log: Enemy Atked");
            mNormalATKTime.Reset();

            if((AIRole != default) && AIRole.ShouldAtkAIWork)
            {
                AIRole.RoleInput.SetInputPhase(KLConsts.ENEMY_INPUT_PHASE_AFTER_NROMAL_ATK);
                Tester.Instance.Log(TesterRPG.Instance, TesterRPG.LOG, "log: Enemy phase ".Append(AIRole.RoleInput.RoleInputPhase.ToString()));
            }
        }

        private void OnStateUpdate(int time)
        {
            if(AIRole != default)
            {
                if (AIRole.FindingPath)
                {
                    Notice notice = Pooling<Notice>.From();
                    RoleSceneComp.Broadcast(KLConsts.N_AI_RESET, notice);
                    notice.ToPool();

                    ChangeToPreviousState();
                }
            }
        }

        public IAIRole AIRole { get; set; }
        public IKLRoleSceneComponent RoleSceneComp { get; private set; }
    }

}