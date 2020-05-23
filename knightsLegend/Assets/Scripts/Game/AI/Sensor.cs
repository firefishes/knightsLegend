namespace KLGame
{
    public class Sensor : ISensor
    {
        public float GetAtkThinkingTime()
        {
            return 0.1f;
        }

        public float Excited { get; set; }
        public float Anger { get; set; }
        public float Calm { get; set; }
        public float Dangerous { get; set; }

    }
}