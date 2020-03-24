﻿using ShipDock.Applications;
using ShipDock.ECS;
using ShipDock.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class RoleAIAtkComponent : RoleInputPhasesComponent
    {
        private IAIRole mAIRole;
        private PositionComponent mPositionComp;

        public override void Init()
        {
            base.Init();

            AddAllowCalled(EnemyInputPhases.ENEMY_INPUT_PHASE_NROMAL_ATKED, 0);
        }

        protected override void InitRolePhases(IRoleInput roleInput)
        {
            mAIRole.IsInitNormalATKPhases = true;
            roleInput.AddEntitasCallback(EnemyInputPhases.ENEMY_INPUT_PHASE_UPDATE_NROMAL_ATK_TRIGGER_TIME, UpdateNormalATKTriggerTime);
            roleInput.AddEntitasCallback(EnemyInputPhases.ENEMY_INPUT_PHASE_NROMAL_ATKED, AfterNormalATK);
        }

        private void AfterNormalATK()
        {
            if (mPositionComp.IsEntitasStoped(ref mRole))
            {
                mAIRole.SetShouldAtkAIWork(true);
                mRoleInput.SetInputPhase(EnemyInputPhases.ENEMY_INPUT_PHASE_SET_NROMAL_ATK_TRIGGER_TIME);
            }
            else
            {
                ResetAIRoleATK();
            }
        }

        private void UpdateNormalATKTriggerTime()
        {
            if (mPositionComp.IsEntitasStoped(ref mRole))
            {
                if (mAIRole.ShouldAtkAIWork)
                {
                    if(!mAIRole.InATKCycle)
                    {
                        if (!mAIRole.TimesEntitas.GetRoleTime(RoleTimingTaskNames.NORMAL_ATK_TIME).IsStart)
                        {
                            mRoleInput.SetInputPhase(EnemyInputPhases.ENEMY_INPUT_PHASE_NROMAL_ATK);
                        }
                    }
                }
                else
                {
                    mRoleInput.SetInputPhase(EnemyInputPhases.ENEMY_INPUT_PHASE_SET_NROMAL_ATK_TRIGGER_TIME);
                }
            }
            else
            {
                ResetAIRoleATK();
            }
        }

        private void ResetAIRoleATK()
        {
            TimingTasker target = mAIRole.TimesEntitas.GetRoleTime(RoleTimingTaskNames.NORMAL_ATK_TIME);
            target.ResetRunCounts();
            
            mAIRole.InATKCycle = false;
            mAIRole.SetShouldAtkAIWork(false);
            mRoleInput.SetInputPhase(UserInputPhases.ROLE_INPUT_PHASE_MOVE_READY);
        }

        public override void Execute(int time, ref IShipDockEntitas target)
        {
            if (mPositionComp == default)
            {
                mPositionComp = ShipDockApp.Instance.Components.GetComponentByAID(KLConsts.C_POSITION) as PositionComponent;
            }
            
            mAIRole = target as IAIRole;
            
            base.Execute(time, ref target);

            if(!mAIRole.IsInitNormalATKPhases)
            {
                InitRolePhases(mRoleInput);
            }

        }
    }

}