using ShipDock.Interfaces;
using ShipDock.Tools;
using System;

namespace ShipDock.Applications
{
    [Serializable]
    public class MotionSceneInfo : IDispose
    {
#if UNITY_EDITOR
        public string skillName;
#endif

        public bool isCombo;
        public int ID;
        public float checkComboTime;
        public int[] indexsForID;
        public SkillAttackFrameInfo[] attackFrameInfo;

        public void Dispose()
        {
            Utils.Reclaim(Motion);
            Utils.Reclaim(ComboMotion);

            ComboMotion = default;
            Motion = default;
            MotionSkillInfo = default;
        }

        public ComboMotionCreater ComboMotion { get; set; }
        public SkillInfo MotionSkillInfo { get; set; }
        public AnimationInfoUpdater Motion { get; set; }
    }
}