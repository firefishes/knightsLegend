using UnityEngine;

namespace ShipDock.Applications
{
    public class HostGameInputer : InputerComponent
    {
        [SerializeField]
        private int m_UserInputComponentName;
        [SerializeField]
        protected HostGameInputerButtons m_InputerButtons;

        private int mAxisCount;
        private float mAxisValue;
        private string mDirectionKey;
        private string[] mDirectionButtons;
        private string[] mDirectionAxis;
        private bool mIsInputCompEmpty;
        private IHostGameInputComponent mHostGameInputComp;

        protected override void Awake()
        {
            base.Awake();

            m_InputerButtons.Init();
        }

        public override void CommitAfterSetToServer()
        {
            base.CommitAfterSetToServer();

            SetDirectionsKeys();

            mHostGameInputComp = mRelater.ComponentRef<IHostGameInputComponent>(m_UserInputComponentName);
            if (mHostGameInputComp != default)
            {
                mHostGameInputComp.SetUserInputerButtons(m_InputerButtons);
            }
            else
            {
                mIsInputCompEmpty = true;
            }
        }

        private void SetDirectionsKeys()
        {
            mDirectionAxis = InputerButtonsKeys.DIRECTION_AXIS;
            mDirectionButtons = m_InputerButtons.axis;
            mAxisCount = mDirectionButtons.Length;
        }

        private void Update()
        {
            if(mIsInputCompEmpty)
            {
                mRelater.CommitRelate();
                mHostGameInputComp = mRelater.ComponentRef<IHostGameInputComponent>(m_UserInputComponentName);
                if (mHostGameInputComp != default)
                {
                    mIsInputCompEmpty = false;
                    mHostGameInputComp.SetUserInputerButtons(m_InputerButtons);
                }
            }
        }

        private void FixedUpdate()
        {
            CheckDirectionsButtons();
            CheckCustomButtons();
        }
        
        private void CheckDirectionsButtons()
        {
            if (mDirectionButtons == default)
            {
                return;
            }
            
            for (int i = 0; i < mAxisCount; i++)
            {
                mDirectionKey = mDirectionButtons[i];
                mAxisValue = Input.GetAxis(mDirectionKey);
                m_InputerButtons.SetAxis(mDirectionAxis[i], mAxisValue);
                m_InputerButtons.SetActiveButton(mDirectionAxis[i], (mAxisValue != 0f));
            }
        }

        protected virtual void CheckCustomButtons()
        {

        }
    }
}
