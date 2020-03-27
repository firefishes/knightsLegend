using ShipDock.Tools;
using System;

namespace ShipDock.Applications
{

    [Serializable]
    public class SkillInfo
    {
#if UNITY_EDITOR
        public string skillName;
#endif
        public int name;
        public ValueSubgroup[] skillParams;

        public ValueItem[] GetInfos()
        {
            int max = skillParams.Length;
            ValueItem[] result = new ValueItem[max];
            for (int i = 0; i < max; i++)
            {
                result[i] = skillParams[i].GetValue();
            }
            return result;
        }
    }
}
