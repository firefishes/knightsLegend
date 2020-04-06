namespace KLGame
{
    public interface IAIRole : IKLRole
    {
        void ResetAIRoleATK();
        void SetShouldAtkAIWork(bool value);
        bool ShouldAtkAIWork { get; }
        bool IsInitNormalATKPhases { get; set; }
    }
}
