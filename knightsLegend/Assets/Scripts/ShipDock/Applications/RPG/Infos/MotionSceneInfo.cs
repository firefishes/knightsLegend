using ShipDock.Interfaces;
using ShipDock.Tools;
using System;

namespace ShipDock.Applications
{
    [Serializable]
    public class MotionSceneInfo : IDispose
    {
        public const int CATEHORY_NONE = 0;
        public const int CATEHORY_ATK = 1;
        public const int CATEHORY_DEF = 2;
        public const int CATEHORY_ASSIST = 3;
        public const int CATEHORY_UNDDER_ATK = 4;

#if UNITY_EDITOR
        public string skillName;
#endif

        public bool isCombo;
        public int ID;
        public int category;
        public float validDistance;
        public float checkComboTime;
        public int[] timingTasks;
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