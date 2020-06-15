namespace KLGame
{
    public interface ISensor
    {
        float GetDecisionTime();
        float GetAtkThinkingTime();
        float GetDefThinkingTime();
        float GetDefCancelThikingTime();
        float Excited { get; set; }
        float Anger { get; set; }
        float Calm { get; set; }
        float Dangerous { get; set; }
    }
}