namespace KLGame
{
    public class Anticipathioner : IAnticipathioner
    {
        public bool IsExecuted { get; set; }
        public int StateFrom { get; set; }
        public AIStateWill AIStateWillChange { get; set; }
    }

}