using ShipDock.Interfaces;
using ShipDock.Tools;
using System;
using UnityEngine;

namespace ShipDock.Applications
{
    [Serializable]
    [CreateAssetMenu(menuName = "ShipDock/Create/RPG/RoleSkillScopeSObj")]
    public class RoleSkillScopeSObj : ScriptableObject, IDispose
    {
        public SkillScopeMapper scopeMapper = new SkillScopeMapper();

        internal void Init()
        {
            scopeMapper.InitMapper();
        }

        public void Dispose()
        {
            Utils.Reclaim(scopeMapper);
        }

        public ScopeChecker GetSkillScope(int infoID)
        {
            return scopeMapper.GetValue(infoID).GetScopeChecker();
        }
    }

    [Serializable]
    public struct SkillScopeInfo
    {
#if UNITY_EDITOR
        public string skillName;
#endif
        public int ID;
        public float validAngle;
        public float minDistance;

        public ScopeChecker GetScopeChecker()
        {
            return ScopeChecker.GetScopeChecker(minDistance, validAngle);
        }
    }

    [Serializable]
    public class SkillScopes
    {
        [SerializeField]
        private int m_SkillID;
        [SerializeField]
        private int[] m_SkillScopeIDs;
        [SerializeField]
        private SkillScopeMapper m_ScopeInfos;

        //public SkillScopeInfo GetSkillScope()
        //{
        //    return 
        //}

        public int[] ScopeIDs
        {
            get
            {
                return m_SkillScopeIDs;
            }
        }

        public int SkillID
        {
            get
            {
                return m_SkillID;
            }
        }
    }

    public class SkillScopeMapper : SceneInfosMapper<int, SkillScopeInfo>
    {
        public override int GetInfoKey(ref SkillScopeInfo item)
        {
            return item.ID;
        }
    }
}
