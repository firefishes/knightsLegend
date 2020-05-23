namespace KLGame
{
    public class Anticipathioner : IAnticipathioner
    {
        public bool IsExecuted { get; set; }
        public int StateFrom { get; set; } = int.MaxValue;
        public AIStateWillChange AIStateWillChange { get; set; }
    }

}