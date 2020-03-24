using ShipDock.Applications;
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
                mNormalAtkMotionCreater.AddComboMotion(ref m_RoleAnimator);
                mRoleATkAI.InATKCycle = true;

                mRoleATkAI.StartTimeEntitasGapper(RoleTimeNames.NORMAL_ATK_HIT_TIME, 0.2f, EnemyAttked);
            }
        }

        private void EnemyAttked()
        {
            mRoleATkAI.TimesEntitas.GetRoleTime(RoleTimeNames.NORMAL_ATK_HIT_TIME).completion -= EnemyAttked;
            if (mRoleATkAI.EnemyMainLockDown != default)
            {
                (mRoleATkAI.EnemyMainLockDown as IKLRole).UnderAttack();
            }
        }

        private void OnSetNormalATKTriggerTime()
        {
            RoleTime target = mRoleATkAI.TimesEntitas.GetRoleTime(RoleTimeNames.NORMAL_ATK_TIME);
            if (target.RunCounts > 0)
            {
                mRoleATkAI.StartTimeEntitasGapper(RoleTimeNames.NORMAL_ATK_TIME, UnityEngine.Random.Range(0.5f, 2f));
                mRoleInput.NextPhase();
            }
            else
            {
                target.DirectCompletion();
                OnEnemyNormalATK();
                //mRoleInput.SetInputPhase(EnemyInputPhases.ENEMY_INPUT_PHASE_NROMAL_ATK);
            }
        }

        protected override void OnAtk1Completed()
        {
            base.OnAtk1Completed();

            mRoleATkAI.InATKCycle = false;
            mRoleInput.SetInputPhase(EnemyInputPhases.ENEMY_INPUT_PHASE_NROMAL_ATKED);
        }
    }
}
