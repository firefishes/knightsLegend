namespace KLGame
{
    public interface ISensor
    {
        float GetAtkThinkingTime();
        float Excited { get; set; }
        float Anger { get; set; }
        float Calm { get; set; }
        float Dangerous { get; set; }
    }
}