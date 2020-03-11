namespace ShipDock.Applications
{
    public class HostGameInputComponent<S> : UserInputComponent<S>, IHostGameInputComponent where S : MainServer
    {
        protected override float GetHorizontal()
        {
            return UserInputButtons.GetAxis(InputerButtonsKeys.DIRECTION_AXIS_H_KEY);
        }

        protected override float GetVertical()
        {
            return UserInputButtons.GetAxis(InputerButtonsKeys.DIRECTION_AXIS_V_KEY);
        }

        public void SetUserInputerButtons(IUserInputerButtons target)
        {
            UserInputButtons = target;
        }

        private bool IsJoypad()
        {
            return false;
        }

        public IUserInputerButtons UserInputButtons { get; private set; }

    }
}