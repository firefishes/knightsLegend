using ShipDock.Notices;
using ShipDock.Pooling;

namespace KLGame
{
    public interface IAIRole : IKLRole
    {
        void ResetAIRoleATK();
        void SetShouldAtkAIWork(bool value);
        bool ShouldAtkAIWork { get; }
        bool IsInitNormalATKPhases { get; set; }
        IAnticipathioner Anticipathioner { get; set; }
    }

    public interface IAnticipathioner : INotificationSender
    {
        int StateFrom { get; set; }
        bool IsExecuted { get; set; }
        AIStateWill AIStateWillChange { get; set; }
    }

    public class Anticipathioner : IAnticipathioner
    {
        public int StateFrom { get; set; }
        public bool IsExecuted { get; set; }
        public AIStateWill AIStateWillChange { get; set; }
    }

    public class AIStateWill : IPoolable
    {
        public int SkillID { get; set; }
        public int StateWill { get; set; }
        public IKLRoleFSMParam RoleFSMParam { get; set; }

        public void Revert()
        {
            SkillID = int.MaxValue;
            StateWill = int.MaxValue;
            RoleFSMParam?.ToPool();
            RoleFSMParam = default;
        }

        public void ToPool()
        {
            Pooling<AIStateWill>.To(this);
        }
    }
}
