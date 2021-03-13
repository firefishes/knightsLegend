using System;
using UnityEngine;

namespace ShipDock.Applications
{
    [Serializable]
    public class HotFixSubgroup
    {
        [SerializeField]
        public string initerNameInResource = "Initer";
        [SerializeField]
        public string initerClassName = "ShipDock.GameRunner";
        [SerializeField]
        public string initerGameCompSetter = "SetGameComponent";
    }
}