using ShipDock.Tools;
using System;
using UnityEngine;

namespace ShipDock.Applications
{
    [Serializable]
    [CreateAssetMenu(menuName = "ShipDock/Create/HostGameInputerButtons")]
    public class HostGameInputerButtons : ScriptableObject, IUserInputerButtons
    {
        public string[] directionButtons = new string[]
        {
        };

        public string[] axis = new string[]
        {
            "Horizontal", "Vertical"
        };

        private readonly KeyValueList<string, string> mMapper = new KeyValueList<string, string>();
        private readonly KeyValueList<string, bool> mButtonActives = new KeyValueList<string, bool>();
        private readonly KeyValueList<string, float> mButtonValues = new KeyValueList<string, float>();

        public HostGameInputerButtons()
        {
        }

        public virtual void Init()
        {
            InitAxisValues();
        }

        private void InitAxisValues()
        {
            AddButtonMap(InputerButtonsKeys.DIRECTION_AXIS_H_KEY, axis[0]);
            AddButtonMap(InputerButtonsKeys.DIRECTION_AXIS_V_KEY, axis[1]);
            SetAxis(InputerButtonsKeys.DIRECTION_AXIS_H_KEY, 0f);
            SetAxis(InputerButtonsKeys.DIRECTION_AXIS_V_KEY, 0f);
        }
        
        public void AddButtonMap(string key, string buttonName)
        {
            mMapper[key] = buttonName;
        }

        public bool GetButton(string key)
        {
            return mButtonActives[key];
        }

        public void SetActiveButton(string key, bool value)
        {
            mButtonActives[key] = value;
        }

        public float GetAxis(string key)
        {
            return mButtonValues[key];
        }

        public void SetAxis(string key, float value)
        {
            mButtonValues[key] = value;
        }
    }

}