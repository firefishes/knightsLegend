using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{
    public abstract class HotFixBase
    {
        public abstract void ShellInited(MonoBehaviour target);

        private Dictionary<string, Action> mHotFixMethods;

        protected void AddHotFixMethod(string name, Action method)
        {
            mHotFixMethods[name] = method;
        }

        protected virtual void InitHotFixedMethods()
        {

        }

        public Action GetUpdateMethods(string name)
        {
            if (mHotFixMethods == default)
            {
                mHotFixMethods = new Dictionary<string, Action>();
                InitHotFixedMethods();
            }

            Action result = default;
            switch (name)
            {
                case "FixedUpdate":
                    result = FixedUpdate;
                    break;
                case "Update":
                    result = Update;
                    break;
                case "LateUpdate":
                    result = LateUpdate;
                    break;
                case "OnDestroy":
                    result = OnDestroy;
                    break;
                default:
                    result = mHotFixMethods[name];
                    break;
            }
            return result;
        }

        protected virtual void OnDestroy()
        {
            mHotFixMethods?.Clear();
            mHotFixMethods = default;
        }

        public abstract void Update();
        public abstract void FixedUpdate();
        public abstract void LateUpdate();
    }
}