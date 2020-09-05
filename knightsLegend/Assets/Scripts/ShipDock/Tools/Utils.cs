using ShipDock.Interfaces;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

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

        public static float RangeRandom(float min, float max)
        {
            System.Random random = new System.Random();
            var d = random.NextDouble() * (max - min) + min;
            return (float)d;
        }

        public static int RangeRandom(float min, float max, float t)
        {
            return (int)(t * RangeRandom(min, max));
        }

        public static float UnityRangeRandom(float min, float max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        public static int UnityRangeRandom(int min, int max)
        {
            return UnityEngine.Random.Range(min, max);
        }

        public static Vector3 GetGroundRandomPosInCircle(Vector3 circleOrigin, ref RayAndHitInfo raycastInfo, float radius, bool isPlaneZ = true, bool isUnityRandom = false)
        {
            Vector3 result = GetRandomPosInCircle(radius, true, isUnityRandom);
            result += circleOrigin;
            bool isHit = Raycast(result, Vector3.down, out raycastInfo.ray, out raycastInfo.hitInfo, raycastInfo.distance, raycastInfo.layerMask);
            if (isHit)
            {
                result = raycastInfo.hitInfo.point;
            }
            return result;
        }

        public static Vector3 GetRandomPosInCircle(float radius, bool isPlaneZ = true, bool isUnityRandom = false)
        {
            float random = isUnityRandom ? UnityRangeRandom(-radius, radius) : RangeRandom(-radius, radius);
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

        public static bool Raycast(Vector3 start, Vector3 direction, out Ray ray, out RaycastHit hitInfo, float distance, int layerMask)
        {
            ray = new Ray(start, direction);
            bool result = Physics.Raycast(ray, out hitInfo, distance, layerMask);
#if UNITY_EDITOR
            Debug.DrawRay(start, direction, Color.red);
#endif
            return result;
        }

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
    }

    public struct RayAndHitInfo
    {
        public Ray ray;
        public RaycastHit hitInfo;
        public float distance;
        public int layerMask;
    }
}
