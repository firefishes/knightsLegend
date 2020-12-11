using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{
    [Serializable]
    public class HotFixerSubgroup
    {
        [SerializeField]
        protected string m_HotFixABName;
        [SerializeField]
        protected string m_HotFixDLL;
        [SerializeField]
        protected string m_HotFixPDB;
        [SerializeField]
        private ValueSubgroup[] m_ComponentData;
        [SerializeField]
        private SceneNodeSubgroup[] m_SceneNodes;
        
        public Dictionary<string, ValueSubgroup> DataMapper { get; private set; }
        public Dictionary<string, SceneNodeSubgroup> SceneNodeMapper { get; private set; }
        
        public ValueSubgroup[] ComponentData
        {
            get
            {
                return m_ComponentData;
            }
        }

        public SceneNodeSubgroup[] SceneNodes
        {
            get
            {
                return m_SceneNodes;
            }
        }


        public string HotFixDLL
        {
            get
            {
                return m_HotFixDLL;
            }
        }

        public string HotFixPDB
        {
            get
            {
                return m_HotFixPDB;
            }
        }

        public string HotFixABName
        {
            get
            {
                return m_HotFixABName;
            }
        }

#if UNITY_EDITOR
        public void Sync()
        {
            int max = m_ComponentData.Length;
            for (int i = 0; i < max; i++)
            {
                m_ComponentData[i]?.Sync();
            }
        }
#endif

        internal void Clear()
        {
            SceneNodeSubgroup item;
            int max = SceneNodes == default ? 0 : SceneNodes.Length;
            for (int i = 0; i < max; i++)
            {
                item = SceneNodes[i];
                item.value = default;
            }
            SceneNodes?.Clone();

            DataMapper?.Clear();
            SceneNodeMapper?.Clear();
        }

        internal void Init()
        {
            DataMapper = new Dictionary<string, ValueSubgroup>();
            SceneNodeMapper = new Dictionary<string, SceneNodeSubgroup>();


            string key;
            ValueSubgroup valueSubgroup;
            int max = m_ComponentData.Length;
            for (int i = 0; i < max; i++)
            {
                valueSubgroup = m_ComponentData[i];
                key = valueSubgroup.keyField;
                DataMapper[key] = valueSubgroup;
            }

            SceneNodeSubgroup sceneNode;
            max = SceneNodes.Length;
            for (int i = 0; i < max; i++)
            {
                sceneNode = SceneNodes[i];
                key = sceneNode.keyField;
                SceneNodeMapper[key] = sceneNode;
            }
        }
        
        public ValueSubgroup GetDataField(ref string keyField)
        {
            return ((DataMapper != default) && DataMapper.ContainsKey(keyField)) ? DataMapper[keyField] : default;
        }

        public SceneNodeSubgroup GetSceneNode(ref string keyField)
        {
            return ((SceneNodeMapper != default) && SceneNodeMapper.ContainsKey(keyField)) ? SceneNodeMapper[keyField] : default;
        }
    }
}