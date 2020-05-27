namespace KLGame
{
    public interface ISensor
    {
        float GetAtkThinkingTime();
        float GetDefThinkingTime();
        float Excited { get; set; }
        float Anger { get; set; }
        float Calm { get; set; }
        float Dangerous { get; set; }
    }
}