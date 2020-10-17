using System;
using System.Collections.Generic;

namespace ShipDock.Tools
{
    public class TreeData<T>
    {
        public TreeData()
        {
            Nodes = new List<TreeData<T>>();
        }

        public TreeData(T data)
        {
            Data = data;
            Nodes = new List<TreeData<T>>();
        }

        public void SetData(T data)
        {
            Data = data;
        }

        /// <summary>
        /// 添加结点
        /// </summary>
        /// <param name="node">结点</param>
        public void AddNode(TreeData<T> node)
        {
            if (!Nodes.Contains(node))
            {
                node.Parent = this;
                Nodes.Add(node);
            }
        }

        /// <summary>
        /// 添加结点
        /// </summary>
        /// <param name="nodes">结点集合</param>
        public void AddNode(List<TreeData<T>> nodes)
        {
            int max = nodes.Count;
            TreeData<T> node;
            for (int i = 0; i < max; i++)
            {
                node = nodes[i];
                if (!nodes.Contains(node))
                {
                    node.Parent = this;
                    nodes.Add(node);
                }
            }
        }

        /// <summary>
        /// 移除结点
        /// </summary>
        /// <param name="node"></param>
        public void Remove(TreeData<T> node)
        {
            if (Nodes.Contains(node))
            {
                Nodes.Remove(node);
            }
        }

        /// <summary>
        /// 清空结点集合
        /// </summary>
        public void RemoveAll()
        {
            Nodes.Clear();
        }

        public static void Recursive(TreeData<T> target, Action<T> callback)
        {
            if (target == default)
            {
                return;
            }

            int max = target.Nodes.Count;
            if (max > 0)
            {
                TreeData<T> item;
                for (int i = 0; i < max; i++)
                {
                    item = target.Nodes[i];
                    callback.Invoke(item.Data);
                    Recursive(item, callback);
                }
            }
        }

        /// <summary>
        /// 父结点
        /// </summary>
        public TreeData<T> Parent { get; private set; }

        /// <summary>
        /// 结点数据
        /// </summary>
        public T Data { get; private set; }

        /// <summary>
        /// 子结点
        /// </summary>
        private List<TreeData<T>> Nodes { get; }
    }
}