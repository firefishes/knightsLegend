using System.Collections.Generic;

namespace KLGame
{
    public interface IAIRole : IKLRole, IGoalExecuter
    {
        bool ShouldAIThinking();
        IAIBrain AIBrain { get; }
        ISensor AISensor { get; }
        IAIBehavioralInfo Anticipathioner { get; set; }
        IAIBehavioralInfo PolicyAnalyzer { get; set; }
        int ConductTimingTask { get; set; }
        List<int> AIThinkingStates { get; }
    }
}
