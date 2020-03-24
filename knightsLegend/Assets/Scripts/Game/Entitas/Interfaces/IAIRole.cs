namespace KLGame
{
    public interface IAIRole : IKLRole
    {
        void SetShouldAtkAIWork(bool value);
        bool ShouldAtkAIWork { get; }
        bool InATKCycle { get; set; }
        bool IsInitNormalATKPhases { get; set; }
    }
}
