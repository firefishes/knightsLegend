using ShipDock.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

namespace ShipDock.Tools
{
    public static class Utils
    {
        /// <summary>
        /// 随机种子递增值
        /// </summary>
        private static int randomSeedByOrder = 0;

        /// <summary>
        /// 销毁对象
        /// </summary>
        public static void Reclaim(IDispose target)
        {
            if (target != null)
            {
                target.Dispose();
            }
        }

        /// <summary>
        /// 清理泛型数组
        /// </summary>
        public static void ClearList<T>(T[] target)
        {
            Array.Clear(target, 0, target.Length);
        }
        
        /// <summary>
        /// 清理泛型数组并根据参数销毁其中元素
        /// </summary>
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

        /// <summary>
        /// 清理列表并根据参数销毁其中元素
        /// </summary>
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

        /// <summary>
        /// 清理队列并根据参数销毁其中元素
        /// </summary>
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

        /// <summary>
        /// 清理堆栈并根据参数销毁其中元素
        /// </summary>
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

        /// <summary>
        /// 清理键值列表并根据参数销毁其中元素
        /// </summary>
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

        /// <summary>
        /// 清理字典并根据参数销毁其中元素
        /// </summary>
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

        /// <summary>
        /// 克隆列表
        /// </summary>
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
        /// 逻辑与方式做子集检测
        /// </summary>
        public static bool IsContains(int target, int containsPart)
        {
            return (target & containsPart) == containsPart;
        }

        /// <summary>
        /// 递增方式更新随机数种子
        /// </summary>
        public static int NextRandomSeed()
        {
            randomSeedByOrder++;
            return randomSeedByOrder;
        }

        /// <summary>
        /// 获取浮点型随机数
        /// </summary>
        public static float RangeRandom(float min, float max, int seed = -1)
        {
            System.Random random = seed == -1 ? new System.Random() : new System.Random(seed);
            var d = random.NextDouble() * (max - min) + min;
            return (float)d;
        }

        /// <summary>
        /// 获取整型随机数
        /// </summary>
        public static int RangeRandom(int min, int max, int seed = -1)
        {
            System.Random random = seed == -1 ? new System.Random() : new System.Random(seed);
            random.Next(min, max);
            return random.Next(min, max);
        }

        /// <summary>
        /// 获取 Unity 提供的浮点随机数
        /// </summary>
        public static float UnityRangeRandom(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        /// <summary>
        /// 获取 Unity 提供的整型随机数
        /// </summary>
        public static int UnityRangeRandom(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        /// <summary>
        /// 获取 circleOrigin 为圆心，radius 为半径的圆形区域内，由射线命中特定层的随机点
        /// </summary>
        public static Vector3 RandomPositionInCircleFromRay(Vector3 circleOrigin, ref RayAndHitInfo raycastInfo, float radius, bool isPlaneZ = true, bool isUnityRandom = false, int seed = -1)
        {
            Vector3 result = RandomPositionInCircle(radius, isPlaneZ, isUnityRandom, seed);
            result += circleOrigin;
            bool isHit = Raycast(result, Vector3.down, out raycastInfo.ray, out raycastInfo.hitInfo, raycastInfo.distance, raycastInfo.layerMask);
            if (isHit)
            {
                result = raycastInfo.hitInfo.point;
            }
            return result;
        }

        /// <summary>
        /// 获取原点为圆心，radius 为半径的圆形区域内的随机点
        /// </summary>
        public static Vector3 RandomPositionInCircle(float radius, bool isPlaneZ = true, bool isUnityRandom = false, int seed = -1)
        {
            float random = isUnityRandom ? UnityRangeRandom(-radius, radius) : RangeRandom(-radius, radius, seed);
            Vector3 result = Vector3.one * random;

            float x = radius * Mathf.Cos(result.x);
            x = isUnityRandom ? UnityRangeRandom(0, x) : RangeRandom(0, x);

            float y = radius * Mathf.Sin(result.y);
            y = isUnityRandom ? UnityRangeRandom(0, y) : RangeRandom(0, y);

            result.Set(x, y, 0);

            if (isPlaneZ)
            {
                result.Set(result.x, 0, result.y);
            }
            return result;
        }

        /// <summary>
        /// 获取原点为圆心，radius 为半径、，rangeLimit为收束倍率的半圆区域内的点
        /// </summary>
        public static Vector3 RandomPositionOnCircle(float radius, bool isPlaneZ = true, bool isUnityRandom = false, int seed = -1)
        {
            float randAngle = isUnityRandom ? UnityRangeRandom(0f, -10f) : RangeRandom(0f, 10f, seed);
            
            float x = radius * Mathf.Cos(randAngle);
            float y = radius * Mathf.Sin(randAngle);
            y = Mathf.Abs(y);
            Vector3 result = new Vector3(x, y, 0);
            if (isPlaneZ)
            {
                result.Set(result.x, 0, result.y);
            }
            return result;
        }

        /// <summary>
        /// 基础的射线检测
        /// </summary>
        public static bool Raycast(Vector3 start, Vector3 direction, out Ray ray, out RaycastHit hitInfo, float distance, int layerMask)
        {
            ray = new Ray(start, direction);
            bool result = Physics.Raycast(ray, out hitInfo, distance, layerMask);
#if UNITY_EDITOR
            Debug.DrawRay(start, direction, Color.red);
#endif
            return result;
        }

        /// <summary>
        /// 使用 RayAndHitInfo 为参数的射线检测
        /// </summary>
        public static bool Raycast(ref RayAndHitInfo rayAndHitInfo)
        {
            return Raycast(
                rayAndHitInfo.start, 
                rayAndHitInfo.direction, 
                out rayAndHitInfo.ray, 
                out rayAndHitInfo.hitInfo, 
                rayAndHitInfo.distance, 
                rayAndHitInfo.layerMask);
        }

        /// <summary>
        /// 使用反射的方式调用函数（支持泛型参数）
        /// </summary>
        public static void InvokeGenericMethod(ref object binder, ref MethodInfo methodInfo, ref Type[] generaicTypes, params object[] paramsValue)
        {
            if (methodInfo != null)
            {
                if (methodInfo.ContainsGenericParameters)
                {
                    MethodInfo method = methodInfo.MakeGenericMethod(generaicTypes);

                    if (method != null)
                    {
                        method.Invoke(binder, paramsValue);
                    }
                }
                else
                {
                    methodInfo.Invoke(binder, paramsValue);
                }
            }
        }

        /// <summary>
        /// 在所有子节点中搜索变换器组件
        /// </summary>
        public static void SearchTransformInChildren(string name, Transform tf, ref Transform result)
        {
            if (tf.name.Equals(name))
            {
                result = tf;
                return;
            }
            Transform child;
            int max = tf.childCount;
            for (int i = 0; i < max; i++)
            {
                child = tf.GetChild(i);
                SearchTransformInChildren(name, child, ref result);
            }
        }

        /// <summary>
        /// 在所有子节点中搜索指定类型的所有组件
        /// </summary>
        public static void GetComponentsInAllChildren<T>(ref List<T> items, Transform tf) where T : Component
        {
            Transform child;
            int max = tf.childCount;
            for (int i = 0; i < max; i++)
            {
                child = tf.GetChild(i);
                T[] weapons = child.GetComponents<T>();
                if (weapons.Length > 0)
                {
                    items.AddRange(weapons);
                }
                GetComponentsInAllChildren(ref items, child);
            }
        }

        /// <summary>
        /// 若数组的元素个数少于指定数量，以最后一个元素为值进行最大填充
        /// </summary>
        public static void ReplenishListByLastValue<T>(ref T[] willReplenish, int max)
        {
            int less = willReplenish.Length;
            int count = max - less;
            if (count > 0)
            {
                T last = willReplenish[less - 1];
                T[] replaced = new T[max];
                for (int i = 0; i < max; i++)
                {
                    replaced[i] = (i < less) ? willReplenish[i] : last;
                }
                willReplenish = replaced;
            }
        }

        /// <summary>
        /// 字符串变Vector3
        /// </summary>
        public static Vector3 Vector3Parse(string vet)
        {
            vet = vet.Replace("(", "").Replace(")", "");
            string[] res = vet.Split(',');
            if (res.Length > 2)
                return new Vector3(float.Parse(res[0]), float.Parse(res[1]), float.Parse(res[2]));
            else
                return Vector3.zero;
        }

        /// <summary>
        /// 世界坐标转UI坐标
        /// </summary>
        public static bool WorldToUIPosition(string pos,GameObject parent,ref Camera UICamera,out Vector3 localPos)
        {
            var position = Utils.Vector3Parse(pos);
            Vector3 viewPos = Camera.main.WorldToViewportPoint(position);
            if (viewPos.z < 0)
            {
                localPos = Vector3.zero;
                return false;
            }
            viewPos.x -= 0.5f;
            viewPos.y -= 0.5f;

            var screenPos = new Vector3(UICamera.pixelWidth * viewPos.x, UICamera.pixelHeight * viewPos.y, 0);
            localPos = screenPos;
            return true;
        }
    }

    public class RayAndHitInfo
    {
        public Vector3 start;
        public Vector3 direction;
        public Ray ray;
        public RaycastHit hitInfo;
        public float distance;
        public float radius;
        public int layerMask;
    }
}
