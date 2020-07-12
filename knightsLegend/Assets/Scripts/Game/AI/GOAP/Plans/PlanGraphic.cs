using System.Collections.Generic;

namespace KLGame
{

    public class PlanGraphic
    {
        private int mParentIndex;
        private List<int> mParents;
        private List<PlanNode> mNodes;
        private List<IAIExecutable> mExecutables;

        public PlanGraphic()
        {
            mParentIndex = -1;
            mParents = new List<int>();
            mNodes = new List<PlanNode>();
            mExecutables = new List<IAIExecutable>();
        }

        public void AddNode(IAIExecutable item, bool isChild = false)
        {
            PlanNode node = new PlanNode
            {
                dataIndex = mExecutables.Count,
                nodeIndex = mNodes.Count
            };

            mNodes.Add(node);
            mExecutables.Add(item);

            if (isChild)
            {
                node.parent = mParentIndex - 1;
                mParentIndex = node.nodeIndex;
            }
            else
            {
                node.parent = mParentIndex;
                mParentIndex = node.nodeIndex;
                mParents.Add(mParentIndex);
            }
        }

        private IAIExecutable GetNode(int nodeIndex)
        {
            return mExecutables[nodeIndex];
        }

        public Queue<IAIExecutable> GetSolution(out int statu)
        {
            statu = 0;
            
            PlanNode node;
            IAIExecutable executable;
            Queue<IAIExecutable> result = new Queue<IAIExecutable>();

            int parent;
            int max = mNodes.Count;
            for (int i = 0; i < max; i++)
            {
                node = mNodes[i];
                parent = node.parent;
                executable = mExecutables[node.dataIndex];

                if(node.parent == -1)
                {
                    result.Enqueue(executable);
                }
                else
                {
                    if (node.parent != parent)
                    {

                    }
                    else
                    {
                        result.Enqueue(executable);
                    }
                }
            }

            return result;
        }
    }

    public class PlanSolution
    {
        public PlanSolution()
        {
            Solution = new Queue<IAIExecutable>();
        }

        public void Enqueue(IAIExecutable item)
        {
            Solution.Enqueue(item);
        }

        public PlanSolution PlanNext(IAIExecutable item)
        {
            Branch = new PlanSolution();
            Branch.Enqueue(item);
            Branch.SetParent(this);
            return Branch;
        }


        public void SetParent(PlanSolution parent)
        {
            Parent = parent;
        }

        public Queue<IAIExecutable> Solution { get; private set; }
        public PlanSolution Branch { get; private set; }
        public PlanSolution Parent { get; private set; }
    }
}