namespace KLGame
{
    public interface IWorldState
    {
        void SetStateID(int id);
        int StateID { get; }
        bool Available { get; set; }
        bool WillRemoveFromWorld { get; set; }
        int OrientedType { get; }
    }
}