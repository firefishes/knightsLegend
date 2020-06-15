namespace KLGame
{
    public class AIBehavioralInfo : IAIBehavioralInfo
    {
        public bool IsExecuted { get; set; }
        public int StateFrom { get; set; } = int.MaxValue;
        public AIStateWillChange AIStateWillChange { get; set; }
    }

}