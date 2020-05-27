using ShipDock.Interfaces;
using ShipDock.Tools;
using System;
using UnityEngine;

namespace ShipDock.Applications
{
    [Serializable]
    [CreateAssetMenu(menuName = "ShipDock/Create/RPG/RoleSkillList")]
    public class RoleSkillListSObj : ScriptableObject, IDispose
    {
#if UNITY_EDITOR
        public TextAsset raw;
#endif

        [SerializeField]
        public SkillsMapper skills = new SkillsMapper();
        [SerializeField]
        public SkillMotionsMapper skillMotions = new SkillMotionsMapper();

        internal void Init()
        {
            skills.InitMapper();
            skillMotions.SkillsMapper = skills;
            skillMotions.InitMapper();
        }

        public void Dispose()
        {
            Utils.Reclaim(skills);
            Utils.Reclaim(skillMotions);

            skills = default;
            skillMotions = default;
        }

        public RoleSkillListSObj LoadObj()
        {
            return Instantiate(this);
        }
    }

}