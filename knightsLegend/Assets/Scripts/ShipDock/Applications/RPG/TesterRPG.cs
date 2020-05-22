#define G_LOG
#define SHOW_ENMEY_POS_GUI
#define SHOW_ROLE_PHASE_GUI

using System;
using System.Collections.Generic;
using System.Diagnostics;
using ShipDock.Testers;
using ShipDock.Tools;
using UnityEngine;

namespace ShipDock.Applications
{
    public class TesterRPG : Singletons<TesterRPG>, ITester
    {
        public const int LOG = -1;

        private static RoleComponent sceneSelectedRole;

        private string mPhaseContentGUI;
        private GUIStyle mEnemyLabelStyle;
        private TimeGapper mTimerForPhaseGUI = new TimeGapper();
        private Queue<int> mPhasesQueueForGUI = new Queue<int>();
        
        public TesterRPG()
        {
            Tester tester = Tester.Instance;
            tester.AddTester(this);
            tester.AddLogger(this, LOG, "{0}");
        }

#if UNITY_EDITOR
        [Conditional("G_LOG")]
        public void ShowRoleInfoInGUI(RoleComponent selectedRole)
        {
            if (sceneSelectedRole != default && sceneSelectedRole != selectedRole)
            {
                sceneSelectedRole.CancelShowEnemyPos();
            }
            sceneSelectedRole = selectedRole;

            if (mEnemyLabelStyle == default)
            {
                mEnemyLabelStyle = new GUIStyle("enemyPosLabel")
                {
                    fontSize = 20
                };
            }

            ICommonRole roleEntitas = selectedRole.RoleEntitas;

            string content = string.Empty;
            ShowRoleLockDownEnemyInGUI(ref roleEntitas, ref content);
            ShowRolePhaseInGUI(ref roleEntitas, ref content);
        }
#endif

        [Conditional("G_LOG")]
        [Conditional("SHOW_ENMEY_POS_GUI")]
        private void ShowRoleLockDownEnemyInGUI(ref ICommonRole roleEntitas, ref string content)
        {
            content = (roleEntitas != default) ? roleEntitas.GetDistFromMainLockDown().ToString() : string.Empty;
            if (roleEntitas.EnemyTracking != default)
            {
                GUILayout.Label("Locked down enemy distance：".Append(content), mEnemyLabelStyle);
            }
        }

        [Conditional("G_LOG")]
        [Conditional("SHOW_ROLE_PHASE_GUI")]
        private void ShowRolePhaseInGUI(ref ICommonRole roleEntitas, ref string content)
        {
            if (!mTimerForPhaseGUI.TimeAdvanced(Time.deltaTime) && mPhasesQueueForGUI.Count < 10)
            {
                if (!mPhasesQueueForGUI.Contains(roleEntitas.RoleInput.RoleInputPhase))
                {
                    mPhasesQueueForGUI.Enqueue(roleEntitas.RoleInput.RoleInputPhase);
                }
            }
            else
            {
                mTimerForPhaseGUI.totalTime = 2f;
                mTimerForPhaseGUI.Start();
                mPhaseContentGUI = string.Empty;
                while (mPhasesQueueForGUI.Count > 0)
                {
                    mPhaseContentGUI = mPhaseContentGUI.Append(mPhasesQueueForGUI.Dequeue().ToString(), ", ");
                }
                mPhasesQueueForGUI.Clear();
            }
            GUILayout.Label("Current role phase: " + mPhaseContentGUI, mEnemyLabelStyle);
        }

        public string Name { get; set; }
    }

}