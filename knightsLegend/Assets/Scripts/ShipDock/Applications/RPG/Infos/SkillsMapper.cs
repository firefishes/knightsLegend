using System;

namespace ShipDock.Applications
{

    [Serializable]
    public class SkillsMapper : SceneInfosMapper<int, SkillInfo>
    {
        public override int GetInfoKey(ref SkillInfo item)
        {
            return item.name;
        }
    }
}
