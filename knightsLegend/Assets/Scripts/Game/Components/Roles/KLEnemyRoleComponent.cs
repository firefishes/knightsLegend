using ShipDock.Applications;
using ShipDock.ECS;
using ShipDock.Pooling;
using System;

namespace KLGame
{
    public class KLEnemyRoleComponent : KLRoleComponent
    {
        private IAIRole mRoleATkAI;

        protected override void SetRoleEntitas()
        {
            mRole = new EnmeyRole();
            mRoleATkAI = mRole as IAIRole;
        }

        protected override void InitRoleInputCallbacks()
        {
            base.InitRoleInputCallbacks();

            SetRoleInputCallback(EnemyInputPhases.ENEMY_INPUT_PHASE_SET_NROMAL_ATK_TRIGGER_TIME, OnSetNormalATKTriggerTime);
            SetRoleInputCallback(EnemyInputPhases.ENEMY_INPUT_PHASE_NROMAL_ATK, OnEnemyNormalATK);
        }

        private void OnEnemyNormalATK()
        {
            if (!mRoleATkAI.InATKCycle)
            {
                m_RoleAnimator.SetFloat(m_BlendTreeInfo.MoveMotionName, 0f);
                //mNormalAtkMotionCreater.AddComboMotion(ref m_RoleAnimator);
                m_Skills?.skillMotions.StartSkill(1, ref m_RoleAnimator);
                mRoleATkAI.InATKCycle = true;

                EnemyAttked();
            }
        }

        private void EnemyAttked()
        {
            mRoleATkAI.TimesEntitas.GetRoleTime(RoleTimingTaskNames.NORMAL_ATK_HIT_TIME).completion -= EnemyAttked;
            ProcessHit hit = Pooling<ProcessHit>.From();
            hit.Target = mRole.EnemyMainLockDown as IShipDockEntitas;
            hit.Initiator = mRoleATkAI as IShipDockEntitas;
            KLRole.Processing.AddProcess(hit);
        }

        private void OnSetNormalATKTriggerTime()
        {
            TimingTasker target = mRoleATkAI.TimesEntitas.GetRoleTime(RoleTimingTaskNames.NORMAL_ATK_TIME);
            if (target.RunCounts > 0)
            {
                mRoleATkAI.StartTimingTask(RoleTimingTaskNames.NORMAL_ATK_TIME, UnityEngine.Random.Range(3f, 5f));
                mRoleInput.NextPhase();
            }
            else
            {
                target.DirectCompletion();
                OnEnemyNormalATK();
                //mRoleInput.SetInputPhase(EnemyInputPhases.ENEMY_INPUT_PHASE_NROMAL_ATK);
            }
        }

        public override void OnAtk1Completed()
        {
            base.OnAtk1Completed();

            mRoleATkAI.InATKCycle = false;
            mRoleInput.SetInputPhase(EnemyInputPhases.ENEMY_INPUT_PHASE_NROMAL_ATKED);
        }

        protected override bool CheckMoveBlock()
        {
            bool flag = base.CheckMoveBlock();
            if (flag)
            {
                mRoleATkAI.ResetAIRoleATK();
            }
            return flag;
        }
    }
}
