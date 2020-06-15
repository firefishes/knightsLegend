namespace KLGame
{
    public struct PlanNode
    {
        public int parent;
        public int next;
        public int index;

        public void SetNext(int nextIndex)
        {
            next = nextIndex;
        }
    }
}