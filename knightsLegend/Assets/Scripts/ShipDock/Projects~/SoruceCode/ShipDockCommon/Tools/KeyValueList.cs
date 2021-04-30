
using ShipDock.Interfaces;
using ShipDock.Pooling;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Tools
{
    /// <summary>
    /// 
    /// 自制哈希类，支持以对象为键名
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class KeyValueList<K, V> : IDispose
    {

        #region 构造函数及初始化
        public KeyValueList()
        {
            Init();
        }

        public KeyValueList(int capacity)
        {
            Init(capacity);
        }

        ~KeyValueList()
        {
            Clear();
        }

        public void Reinit(int capacity = 0)
        {
            Init(capacity);
        }

        /// <summary>初始化</summary>
        protected void Init(int capacity = 0)
        {
            if (Keys != null)
            {
                Keys.Clear();
            }
            
            if (Values != null)
            {
                Values.Clear();
            }
            
            if(capacity != 0)
            {
                Keys = new List<K>(capacity);
                Values = new List<V>(capacity);
            }
            else
            {
                Keys = new List<K>();
                Values = new List<V>();
            }
            Capacity = Keys.Capacity;
        }

        public void Clone(ref K[] k, ref V[] v)
        {
            Clear();
            Keys = new List<K>(k);
            Values = new List<V>(v);
        }

        public void Clone(ref KeyValueList<K, V> target, bool isClear = false)
        {
            if(target == null)
            {
                target = new KeyValueList<K, V>();
            }
            int max = Keys.Count;
            for (int i = 0; i < max; i++)
            {
                target[Keys[i]] = Values[i];
            }
            if(isClear)
            {
                Clear();
            }
        }
        #endregion

        #region 销毁
        public virtual void Dispose()
        {
            Clear();
        }

        public void Dispose(bool isDisposeItems)
        {
            if (isDisposeItems)
            {
                IDispose item;
                int max = Values.Count;
                for (int i = 0; i < max; i++)
                {
                    if (Values[i] is IDispose)
                    {
                        item = Values[i] as IDispose;
                        item.Dispose();
                    }
                    else if(Values[i] is GameObject)
                    {
                        Object.DestroyImmediate(Values[i] as GameObject);
                    }
                    else if(Values[i] is IPoolable)
                    {
                        (Values[i] as IPoolable).Revert();
                    }
                }
            }
            Dispose();
        }

        protected void Purge()
        {
            Values = null;
            Keys = null;
        }

        public virtual void Clear(bool isTrimExcess = false)
        {
            Values.Clear();
            Keys.Clear();
            if(isTrimExcess)
            {
                TrimExcess();
            }
        }
        #endregion

        #region 键值对管理
        /// <summary>添加所有</summary>
        public void PutAll(ref KeyValueList<K, V> map)
        {
            if (map == null)
            {
                return;
            }

            Clear();

            int max = map.Size;
            if (max > 0)
            {
                K key;
                List<K> list = map.Keys;
                for (int i = 0; i < max; i++)
                {
                    key = list[i];
                    Put(key, map[key]);
                }
            }
        }

        /// <summary>检查是否包含某个属性</summary>
        public bool IsContainsKey(K key)
        {
            return KeyIndex(key) != -1;
        }

        /// <summary>检查是否包含某个值</summary>
        public bool IsContainsValue(V value)
        {
            return ValueIndex(value) != -1;
        }

        private int KeyIndex(K target)
        {
            return (Keys != null) ? Keys.IndexOf(target) : -1;
        }

        private int ValueIndex(V target)
        {
            return Values != null ? Values.IndexOf(target) : -1;
        }

        /// <summary>获取和设置值</summary>
        public V this[K key]
        {
            get
            {
                return GetValue(key);
            }
            set
            {
                if (Values == null)
                {
                    Init();
                }

                Put(key, value);
            }
        }

        public V Put(K key, V value)
        {
            V result = default(V);
            int index = KeyIndex(key);
            if (index != -1)
            {
                result = Values[index];
                Values[index] = value;
            }
            else
            {
                Values.Add(value);
                Keys.Add(key);
            }

            return result;
        }

        /// <summary>移除数据</summary>
        public V Remove(K key)
        {
            int index = KeyIndex(key);
            bool hasKey = index != -1;
            if ((key == null) || !hasKey)
            {
                return default(V);
            }

            V result = hasKey ? Values[index] : default(V);
            if (hasKey)
            {
                Keys.RemoveAt(index);
                Values.RemoveAt(index);
            }
            return result;
        }

        private bool CheckValid()
        {
            return Keys != null && Values != null;
        }

        /// <summary>通过数据获取键名</summary>
        public K GetKey(V value)
        {
            if (!CheckValid())
            {
                return default(K);
            }

            K result = default(K);
            int max = Keys.Count;
            for (int i = 0; i < max; i++)
            {
                result = Keys[i];
                if (Values[i].Equals(value))
                {
                    return result;
                }
            }
            return result;
        }

        /// <summary>通过键名获取数据</summary>
        public V GetValue(K key, bool isDelete = false)
        {
            if (!CheckValid())
            {
                return default;
            }

            int index = KeyIndex(key);
            bool hasKey = (index != -1);
            if (!hasKey)
            {
                return default;
            }

            V result = Values[index];
            if (isDelete)
            {
                Keys.RemoveAt(index);
                Values.RemoveAt(index);
            }
            return result;
        }

        /// <summary>通过索引获取数据</summary>
        public V GetValueByIndex(int index, bool isDelete = false)
        {
            if (!CheckValid())
            {
                return default;
            }

            V result = default;
            if (index < Keys.Count)
            {
                K key = Keys[index];
                result = GetValue(key, isDelete);
            }
            return result;
        }

        public V TryGet(K key)
        {
            if (IsContainsKey(key))
            {
                return GetValue(key);
            }
            else
            {
                return default;
            }
        }

        public bool ContainsKey(K key)
        {
            return Keys != null ? Keys.Contains(key) : false;
        }

        public bool ContainsValue(V value)
        {
            return Values != null ? Values.Contains(value) : false;
        }

        public void TrimExcess()
        {
            Keys.TrimExcess();
            Values.TrimExcess();
        }
        #endregion

        #region 属性
        public int Size
        {
            get
            {
                return (Keys != null) ? Keys.Count : 0;
            }
        }

        public bool IsEmpty
        {
            get
            {
                return Size < 1;
            }
        }

        public int Capacity { get; private set; }
        public List<K> Keys { get; private set; }
        public List<V> Values { get; private set; }
        #endregion

    }

    public class StringIntValueList : KeyValueList<string, int>
    {
        public int Change(string key, int value, int defaultValue = 0, bool checkRemovable = true, int removeLowFromValue = 0)
        {
            if(!IsContainsKey(key))
            {
                this[key] = defaultValue;
            }
            int result = this[key] += value;
            if(checkRemovable && (result <= removeLowFromValue))
            {
                Remove(key);
            }
            return result;
        }
    }
}