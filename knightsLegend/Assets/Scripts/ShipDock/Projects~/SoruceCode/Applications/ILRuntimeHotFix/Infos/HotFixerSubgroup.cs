
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif

using System;
using System.Collections.Generic;
using UnityEngine;

namespace ShipDock.Applications
{
    [Serializable]
    public class HotFixerSubgroup
    {
        [Header("桥接至热更端的绑定信息")]
        [SerializeField]
        [Tooltip("AsstBundle资源名")]
        protected string m_HotFixABName;
#if ODIN_INSPECTOR
        [SuffixLabel(".dll")]
#endif
        [SerializeField]
        [Tooltip("dll热更资源名")]
        protected string m_HotFixDLL;
#if ODIN_INSPECTOR
        [SuffixLabel(".pdb")]
#endif
        [SerializeField]
        [Tooltip("pdb符号表文件资源名")]
        protected string m_HotFixPDB;
#if ODIN_INSPECTOR
        [SerializeField]
        private bool m_EditHotFixBinding = false;
        [ReadOnly]
        [PropertyTooltip("@Overview")]
        [SerializeField]
        private string m_Overviews = "热更绑定值概览";

        private string mOverviewTooltips;

        public string Overview
        {
            get
            {
                mOverviewTooltips = string.Empty;
                if (m_EditHotFixBinding)
                {
                    return string.Empty;
                }
                string temp;
                int max = m_ComponentData != default ? m_ComponentData.Length : 0;
                string res = string.Empty;
                ValueSubgroup item;
                string s = " = ", nextLine = "\r\n";
                if (max > 0)
                {
                    mOverviewTooltips = "值绑定：".Append(nextLine);
                }
                for (int i = 0; i < max; i++)
                {
                    item = m_ComponentData[i];
                    switch (item.valueType)
                    {
                        case ValueItemType.LAYER_MASK:
                            temp = item.keyField.Append(s, item.GetLayerMask().ToString());
                            break;
                        case ValueItemType.VECTOR_2:
                            temp = item.keyField.Append(s, item.GetV2().ToString());
                            break;
                        case ValueItemType.VECTOR_3:
                            temp = item.keyField.Append(s, item.GetV3().ToString());
                            break;
                        case ValueItemType.BOOL:
                            temp = item.keyField.Append(s, item.GetBool().Bool ? "True" : "False");
                            break;
                        case ValueItemType.FLOAT:
                            temp = item.keyField.Append(s, item.Result().Float.ToString());
                            break;
                        case ValueItemType.INT:
                            temp = item.keyField.Append(s, item.Result().Int.ToString());
                            break;
                        case ValueItemType.COLOR:
                            temp = item.keyField.Append(s, item.GetColor().ToString());
                            break;
                        default:
                            temp = item.keyField.Append(s, item.Result().Value);
                            break;
                    }
                    mOverviewTooltips = mOverviewTooltips.Append("    ", temp, nextLine);
                }
                max = m_SceneNodes != default ? m_SceneNodes.Length : 0;
                if (max > 0)
                {
                    mOverviewTooltips = mOverviewTooltips.Append(nextLine, "对象绑定：", nextLine);
                }
                SceneNodeSubgroup sceneItem;
                temp = string.Empty;
                for (int i = 0; i < max; i++)
                {
                    sceneItem = m_SceneNodes[i];
                    switch(sceneItem.valueType)
                    {
                        case SceneNodeType.GAME_OBJECT:
                            temp = sceneItem.keyField.Append(" {", sceneItem.value.name, "}");
                            break;
                        case SceneNodeType.ANIMATOR:
                            temp = sceneItem.keyField.Append("Animator {", sceneItem.animator.name, "}");
                            break;
                        case SceneNodeType.CAMERA:
                            temp = sceneItem.keyField.Append(" Camera {", sceneItem.lens.name, "}");
                            break;
                        case SceneNodeType.SPRITE:
                            temp = sceneItem.keyField.Append(" Sprite {", sceneItem.sprite.name, "}");
                            break;
                        case SceneNodeType.TEXTURE:
                            temp = sceneItem.keyField.Append(" Texture {", sceneItem.texture.name, "}");
                            break;
                        case SceneNodeType.UI_BUTTON:
                            temp = sceneItem.keyField.Append(" Button {", sceneItem.button.name, "}");
                            break;
                        case SceneNodeType.UI_IMAGE:
                            temp = sceneItem.keyField.Append(" Image {", sceneItem.image.name, "}");
                            break;
                        case SceneNodeType.UI_TEXT:
                            temp = sceneItem.keyField.Append(" Text {", sceneItem.Label.name, "}");
                            break;
                        case SceneNodeType.ANI_CURVE:
                            int n = sceneItem.animationCurve.keys.Length;
                            float t, v;
                            temp = string.Empty;
                            string kf = "    key frame: time=", c = ", value=";
                            Keyframe k;
                            for (int j = 0; j < n; j++)
                            {
                                k = sceneItem.animationCurve.keys[j];
                                t = k.time;
                                v = k.value;
                                temp = temp.Append(kf, t.ToString(), c, v.ToString(), nextLine);
                            }
                            temp = string.Format(sceneItem.keyField.Append(":", nextLine, "{0}"), temp);
                            break;
                        default:
                            temp = sceneItem.keyField.Append(" Null");
                            break;
                    }
                    mOverviewTooltips = mOverviewTooltips.Append("    ", temp, nextLine);
                }
                return mOverviewTooltips;
            }
        }

        [ShowIf("m_EditHotFixBinding", true)]
#endif
        [SerializeField]
        [Tooltip("热更端可能用到的值类型桥接设置")]
        private ValueSubgroup[] m_ComponentData;
#if ODIN_INSPECTOR
        [ShowIf("m_EditHotFixBinding", true)]
#endif
        [SerializeField]
        [Tooltip("热更端可能用到的引用类型桥接设置")]
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