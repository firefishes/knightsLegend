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

            m_DisposeSkillInfos = false;
        }

        protected override void AfterInitItem(ref MotionSceneInfo item)
        {
            base.AfterInitItem(ref item);

            if (item.isCombo)
            {
                ComboMotionCreater target = GetComboMotionCreater(SkillsMapper, item.indexsForID[0], item.indexsForID[1], default, item.checkComboTime);
                target.MotionCompletionEvent = item.motionCompletionEvent;
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

        private ComboMotionCreater GetComboMotionCreater(SkillsMapper mapper, int IDForTriggers = -1, int IDForTrans = -1, Action onCompleted = default, float checkComboTime = 0f)
        {
            ValueItem[] triggers = (IDForTriggers > -1) ? mapper[IDForTriggers].GetInfos() : emptySkillInfoValueItems;
            ValueItem[] trans = (IDForTrans > -1) ? mapper[IDForTrans].GetInfos() : emptySkillInfoValueItems;
            ComboMotionCreater result = new ComboMotionCreater(trans.Length, triggers, trans, onCompleted);
            result.SetCheckComboTime(checkComboTime);
            return result;
        }

        public override int GetInfoKey(ref MotionSceneInfo item)
        {
            return item.ID;
        }

        public void StartSkill(int id, ref Animator animator, Action<Animator> onMotionCompleted = default)
        {
            MotionSceneInfo sceneInfo = GetValue(id);
            if (sceneInfo != default)
            {
                if (sceneInfo.isCombo)
                {
                    sceneInfo.ComboMotion.AddComboMotion(ref animator);
                }
                else
                {
                    if (sceneInfo.Motion.HasCompleted)
                    {
                        sceneInfo.Motion.Start(animator, 0f, onMotionCompleted, sceneInfo.MotionSkillInfo.GetInfos());
                    }
                }
            }
        }

        internal void UpdateMotions(ref Animator m_RoleAnimator)
        {
            int max = mComboMotionIndex.Count;
            for (int i = 0; i < max; i++)
            {
                Values[mComboMotionIndex[i]].ComboMotion?.CheckAnimator(ref m_RoleAnimator);
                Values[mComboMotionIndex[i]].ComboMotion?.CountComboTime(ref m_RoleAnimator);
            }
        }

        public SkillsMapper SkillsMapper { get; set; }
    }
}