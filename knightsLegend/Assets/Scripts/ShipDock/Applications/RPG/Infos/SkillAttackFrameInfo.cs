using System;
using UnityEngine;

namespace ShipDock.Applications
{
    [Serializable]
    public class SkillAttackFrameInfo
    {
        [SerializeField]
        public int frameMin;
        [SerializeField]
        public int frameMax;
    }

}