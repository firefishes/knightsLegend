using ShipDock.Applications;

namespace KLGame
{
    public class KLInputComponent : HostGameInputComponent<KLServer>
    {
        private bool[] mAtkButtonKeeps;
        private string mAtkButtonName;
        private string[] mAtkButtonNames;

        protected override void CheckUserInput()
        {
            base.CheckUserInput();
            
            if(mAtkButtonNames == default)
            {
                mAtkButtonNames = KLHostGameInputer.atkButtonNames;
                mAtkButtonKeeps = KLHostGameInputer.atkButtonKeeps;
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
                }
                else
                {
                    if (mRoleInput.GetUserInputValue(mAtkButtonName))
                    {
                        mRoleInput.SetUserInputValue(mAtkButtonName, false);
                    }
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