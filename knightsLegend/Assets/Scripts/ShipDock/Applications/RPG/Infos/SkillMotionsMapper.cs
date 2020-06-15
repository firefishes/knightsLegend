using ShipDock.Tools;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{

    [Serializable]
    public class SkillMotionsMapper : SceneInfosMapper<int, MotionSceneInfo>
    {
        private readonly static ValueItem[] emptySkillInfoValueItems = new ValueItem[0];

        private List<int> mComboMotionIndex = new List<int>();

        public override void InitMapper()
        {
            base.InitMapper();

            m_DisposeInfos = false;
        }

        protected override void AfterInitItem(ref MotionSceneInfo item)
        {
            base.AfterInitItem(ref item);

            int triggerIDIndex = item.indexsForID.Length >= 1 ? item.indexsForID[0] : -1;
            if (item.isCombo)
            {
                int transIDIndex = item.indexsForID.Length >= 2 ? item.indexsForID[1] : -1;
                ComboMotionCreater target = GetComboMotionCreater(SkillsMapper, triggerIDIndex, transIDIndex, item.checkComboTime);
                item.ComboMotion = target;

                int index = Values.IndexOf(item);
                if(!mComboMotionIndex.Contains(index))
                {
                    mComboMotionIndex.Add(index);
                }
            }
            else
            {
                int idForSkillMapper = item.indexsForID[0];
                SkillInfo infos = SkillsMapper[idForSkillMapper];
                item.MotionSkillInfo = infos;
                item.Motion = new AnimationInfoUpdater();
            }
        }

        private ComboMotionCreater GetComboMotionCreater(SkillsMapper mapper, int IDForTriggers = -1, int IDForTrans = -1, float checkComboTime = 0f)
        {
            ValueItem[] triggers = (IDForTriggers > -1) ? mapper[IDForTriggers].GetInfos() : emptySkillInfoValueItems;
            ValueItem[] trans = (IDForTrans > -1) ? mapper[IDForTrans].GetInfos() : emptySkillInfoValueItems;
            ComboMotionCreater result = new ComboMotionCreater(trans.Length, triggers, trans);
            return result;
        }

        public override int GetInfoKey(ref MotionSceneInfo item)
        {
            return item.ID;
        }

        public ComboMotionCreater GetComboMotion(int id, ref Animator animator)
        {
            ComboMotionCreater result = default;
            MotionSceneInfo sceneInfo = GetValue(id);
            if (sceneInfo != default)
            {
                if (sceneInfo.isCombo)
                {
                    result = sceneInfo.ComboMotion;
                    result.StartComboMotion(ref animator);
                }
            }
            return result;
        }

        public AnimationInfoUpdater GetMotion(int id, ref Animator animator)
        {
            AnimationInfoUpdater result = default;
            MotionSceneInfo sceneInfo = GetValue(id);
            if (sceneInfo != default)
            {
                if (!sceneInfo.isCombo)
                {
                    result = sceneInfo.Motion;
                    result.Start(animator, sceneInfo.MotionSkillInfo.GetInfos());
                }
            }
            return result;
        }

        public List<SkillConduct> GetDistanceNearestSkill(float distance, int category, bool shouldMults = true)
        {
            int max = Size;
            MotionSceneInfo item;
            List<SkillConduct> result = new List<SkillConduct>();
            for (int i = 0; i < max; i++)
            {
                item = GetValue(Keys[i]);
                if ((category == item.category) && (distance <= item.validDistance))
                {
                    SkillConduct conduct = new SkillConduct
                    {
                        id = item.ID,
                        timingTasks = item.timingTasks
                    };
                    result.Add(conduct);
                    if (!shouldMults)
                    {
                        break;
                    }
                }
            }
            return result;
        }

        public SkillsMapper SkillsMapper { get; set; }
    }

    public struct SkillConduct
    {
        public int id;
        public int[] timingTasks;
    }
}