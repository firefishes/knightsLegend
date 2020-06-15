namespace KLGame
{
    public interface IWorldStateIssuer
    {
        IWorldState[] ProvideWorldStates { get; }
    }
}
