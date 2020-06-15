namespace KLGame
{
    public abstract class Sensor : ISensor
    {
        public abstract float GetAtkThinkingTime();
        public abstract float GetDefThinkingTime();
        public abstract float GetDefCancelThikingTime();
        public abstract float GetDecisionTime();

        public float Excited { get; set; }
        public float Anger { get; set; }
        public float Calm { get; set; }
        public float Dangerous { get; set; }

    }

    public interface IAIState
    {
        void AIConduct();
    }

    public class NormalSensor : Sensor
    {
        public override float GetAtkThinkingTime()
        {
            return 5f;
        }

        public override float GetDefThinkingTime()
        {
            return 0.1f;
        }

        public override float GetDefCancelThikingTime()
        {
            return 0.3f;
        }

        public override float GetDecisionTime()
        {
            return 0.2f;
        }
    }
}