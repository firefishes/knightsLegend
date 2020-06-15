using ShipDock.Interfaces;
using System;
using System.Collections.Generic;

namespace ShipDock.Tools
{
    public static class Utils
    {
        public static void Reclaim(IDispose target)
        {
            if (target != null)
            {
                target.Dispose();
            }
        }
        
        public static void Reclaim<T>(ref T[] target, bool isSetNull = true, bool isDisposeItems = false)
        {
            if ((target != default) && (target.Length > 0))
            {
                if (isDisposeItems)
                {
                    IDispose dp;
                    int max = target.Length;
                    for (int i = 0; i < max; i++)
                    {
                        dp = target[i] as IDispose;
                        Reclaim(dp);
                    }
                }
                Array.Clear(target, 0, target.Length);
            }
            if (isSetNull)
            {
                target = default;
            }
        }

        public static void Reclaim<T>(ref List<T> target, bool isSetNull = true, bool isDisposeItems = false)
        {
            if (target == null)
            {
                return;
            }
            if (isDisposeItems)
            {
                IDispose dp;
                int max = target.Count;
                for (int i = 0; i < max; i++)
                {
                    dp = target[i] as IDispose;
                    Reclaim(dp);
                }
            }
            target.Clear();

            if (isSetNull)
            {
                target = null;
            }
        }
        
        public static void Reclaim<T>(ref Queue<T> target, bool isSetNull = true, bool isDisposeItems = false)
        {
            if (target == null)
            {
                return;
            }

            if (isDisposeItems)
            {
                T item;
                IDispose dp;
                int max = target.Count;
                while (target.Count > 0)
                {
                    item = target.Dequeue();
                    dp = item as IDispose;
                    Reclaim(dp);
                }
            }
            target.Clear();

            if (isSetNull)
            {
                target = null;
            }
        }

        public static void Reclaim<T>(ref Stack<T> target, bool isSetNull = true, bool isDisposeItems = false)
        {
            if (target == null)
            {
                return;
            }

            if (isDisposeItems)
            {
                T item;
                IDispose dp;
                int max = target.Count;
                while (target.Count > 0)
                {
                    item = target.Peek();
                    dp = item as IDispose;
                    Reclaim(dp);
                }
            }
            target.Clear();

            if (isSetNull)
            {
                target = null;
            }
        }

        public static void Reclaim<K, V>(ref KeyValueList<K, V> target, bool isSetNull = true, bool isDisposeItems = false)
        {
            if (target == null)
            {
                return;
            }
            
            target.Dispose(isDisposeItems);

            if (isSetNull)
            {
                target = null;
            }
        }

        public static void Reclaim<K, V>(ref Dictionary<K, V> target, bool isSetNull = true, bool isDisposeItems = false)
        {
            if (target == null)
            {
                return;
            }

            if (isDisposeItems)
            {
                K key;
                V value;
                IDispose item;
                var keys = target.Keys;
                int max = keys.Count;
                var enumer = keys.GetEnumerator();
                for (int i = 0; i < max; i++)
                {
                    key = enumer.Current;
                    value = target[key];
                    if(value is IDispose)
                    {
                        item = value as IDispose;
                        item.Dispose();
                    }
                    enumer.MoveNext();
                }
            }
            else
            {
                target.Clear();
            }

            if (isSetNull)
            {
                target = null;
            }
        }

        public static void CloneFrom<T>(ref List<T> target, ref List<T> from, bool isClearRaw = false)
        {
            if (from == null)
            {
                return;
            }
            int max = from.Count;
            if (target == null)
            {
                target = new List<T>(max);
            }
            else
            {
                target.Clear();
                if (target.Count < max)
                {
                    target = new List<T>(max);
                }
                else
                {
                    target.TrimExcess();
                }
            }
            for (int i = 0; i < max; i++)
            {
                target.Add(from[i]);
            }
            if (isClearRaw)
            {
                from.Clear();
            }
        }

        /// <summary>
        /// 检测目标值是否包含权限
        /// </summary>
        public static bool IsContains(int target, int containsPart)
        {
            return (target & containsPart) == containsPart;
        }
    }
}
