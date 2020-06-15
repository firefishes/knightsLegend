using System.Collections.Generic;

namespace KLGame
{

    public class PlanGraphic
    {
        private Queue<int> mParents;
        private List<PlanNode> mNodes;
        private List<IAIExecutable> mExecutables;

        public PlanGraphic()
        {
            mParents = new Queue<int>();
            mNodes = new List<PlanNode>();
            mExecutables = new List<IAIExecutable>();
        }

        public int AddNode(IAIExecutable item, bool isChild = false, int parentIndex = int.MaxValue)
        {
            PlanNode node = new PlanNode
            {
                index = mNodes.Count
            };
            if (isChild)
            {
                int parent = isChild ? mNodes.Count - 1 : int.MaxValue;
                node.parent = parentIndex == int.MaxValue ? parent : parentIndex;
                node.next = int.MaxValue;
                mNodes[parent].SetNext(node.index);
            }
            else
            {
                node.parent = int.MaxValue;
                mParents.Enqueue(node.index);
            }
            mNodes.Add(node);
            mExecutables.Add(item);

            return node.index;
        }

        private IAIExecutable GetNode(int nodeIndex)
        {
            return mExecutables[nodeIndex];
        }

        public Queue<IAIExecutable> GetSolution(int nodeIndex, out int statu)
        {
            statu = 0;
            if (!mParents.Contains(nodeIndex))
            {
                statu = 1;
                return default;
            }

            PlanNode node = mNodes[nodeIndex];

            Queue<IAIExecutable> result = new Queue<IAIExecutable>();

            int index = node.index;
            IAIExecutable item = GetNode(index);
            result.Enqueue(item);

            while (node.parent != int.MaxValue)
            {
                index = node.parent;
                node = mNodes[index];

                index = node.index;
                item = GetNode(index);

                result.Enqueue(item);
            }

            return result;
        }
    }
}