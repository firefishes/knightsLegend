using ShipDock.Applications;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace KLGame
{
    public class KLInputComponent : HostGameInputComponent<KLServer>
    {
        private string mAtkButtonName;
        private string[] mAtkButtonNames;

        protected override void CheckUserInput()
        {
            base.CheckUserInput();
            
            if(mAtkButtonNames == default)
            {
                mAtkButtonNames = KLHostGameInputer.atkButtonNames;
            }

            int max = mAtkButtonNames.Length;
            for (int i = 0; i < max; i++)
            {
                mAtkButtonName = mAtkButtonNames[i];
                if (UserInputButtons.GetButton(mAtkButtonName))
                {
                    if(!mRoleInput.GetUserInputValue(mAtkButtonName))
                    {
                        mRoleInput.SetUserInputValue(mAtkButtonName, true);
                    }
                    UserInputButtons.SetActiveButton(mAtkButtonName, false);
                }
            }

        }

        protected override string[] RelateServerNames { get; } = new string[]
        {
            KLConsts.S_KL
        };

        protected override string MainServerName { get; } = KLConsts.S_KL;
    }

}