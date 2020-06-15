namespace KLGame
{
    public interface IAIBrain
    {
        void Clean();
        void InitAIBrain(ISensor sencor, IAIBehavioralInfo anticipathioner, IAIBehavioralInfo policyAnalyzer);
        void SetRole(IKLRole role);
        void ExecuteDecision(IAIRole mRoleATkAI, IKLRoleSceneComponent roleSceneComponent);
        IAIBehavioralInfo Anticipathioner { get; }
        IAIBehavioralInfo PolicyAnalyzer { get; }
    }
}