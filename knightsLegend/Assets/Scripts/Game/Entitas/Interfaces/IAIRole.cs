using System.Collections.Generic;

namespace KLGame
{
    public interface IAIRole : IKLRole
    {
        bool ShouldAIThinking();
        ISensor AISensor { get; set; }
        IAnticipathioner Anticipathioner { get; set; }
        List<int> AIThinkingStates { get; }
    }
}
