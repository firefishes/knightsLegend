namespace KLGame
{
    public interface IWorldEffect
    {
        void CommitEffect(ref IWorldState worldState);
        bool Available { get; set; }
    }
}