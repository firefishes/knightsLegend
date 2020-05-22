using ShipDock.Pooling;
using ShipDock.Tools;

namespace KLGame
{
    public class AIStateWill : IPoolable
    {
        public void Revert()
        {
            SkillID = int.MaxValue;
            StateWill = int.MaxValue;

            int[] inputs = Inputs;
            Utils.Reclaim(ref inputs);

            RoleFSMParam?.ToPool();
            RoleFSMParam = default;
            Inputs = default;
        }

        public void ToPool()
        {
            Pooling<AIStateWill>.To(this);
        }

        public int SkillID { get; set; }
        public int StateWill { get; set; }
        public int[] Inputs { get; set; }
        public IKLRoleFSMParam RoleFSMParam { get; set; }
    }
}