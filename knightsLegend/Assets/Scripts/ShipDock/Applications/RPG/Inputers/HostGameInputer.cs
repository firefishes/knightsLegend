using ShipDock.Tools;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{
    public class HostGameInputer : InputerComponent
    {
        [SerializeField]
        private int m_UserInputComponentName;
        [SerializeField]
        private HostGameInputerButtons m_InputerButtons;

        private int mAxisCount;
        private float mAxisValue;
        private string mDirectionKey;
        private string[] mDirectionButtons;
        private string[] mDirectionAxis;
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
            mHostGameInputComp.SetUserInputerButtons(m_InputerButtons);
        }

        private void SetDirectionsKeys()
        {
            mDirectionAxis = InputerButtonsKeys.DIRECTION_AXIS;
            mDirectionButtons = m_InputerButtons.axis;
            mAxisCount = mDirectionButtons.Length;
        }

        private void Update()
        {
            CheckDirectionsButtons();
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
    }
}
