using ShipDock.Applications;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class KLHostGameInputer : HostGameInputer
    {
        public readonly static string[] atkButtonNames = new string[] { "Fire1", "Fire2", "Fire3" };

        private string mAtkButtonName;

        protected override void CheckCustomButtons()
        {
            int max = atkButtonNames.Length;
            for (int i = 0; i < max; i++)
            {
                mAtkButtonName = atkButtonNames[i];
                if (Input.GetButtonDown(mAtkButtonName))
                {
                    m_InputerButtons.SetActiveButton(mAtkButtonName, true);
                }
            }
        }

        protected override int[] RelatedComponentNames { get; } = new int[]
        {
            KLConsts.C_ROLE_INPUT
        };
    }
}
