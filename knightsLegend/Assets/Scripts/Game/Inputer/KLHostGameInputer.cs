using ShipDock.Applications;
using UnityEngine;

namespace KLGame
{
    public class KLHostGameInputer : HostGameInputer
    {
        public readonly static string[] atkButtonNames = new string[] { "Fire1", "Fire2", "Fire3" };
        public readonly static bool[] atkButtonKeeps = new bool[] { false, true, false };

        private bool mButtonKeppable;
        private string mAtkButtonName;

        protected override void CheckCustomButtons()
        {
            int max = atkButtonNames.Length;
            for (int i = 0; i < max; i++)
            {
                mAtkButtonName = atkButtonNames[i];
                mButtonKeppable = atkButtonKeeps[i];
                if (mButtonKeppable)
                {
                    m_InputerButtons.SetActiveButton(mAtkButtonName, Input.GetButton(mAtkButtonName));
                }
                else
                {
                    m_InputerButtons.SetActiveButton(mAtkButtonName, Input.GetButtonDown(mAtkButtonName));
                }
            }
        }

        protected override int[] RelatedComponentNames { get; } = new int[]
        {
            KLConsts.C_ROLE_INPUT
        };
    }
}
