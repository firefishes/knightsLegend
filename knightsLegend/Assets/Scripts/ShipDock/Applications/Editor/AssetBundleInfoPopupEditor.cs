using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using ShipDock.Applications;
using ShipDock.Tools;
using UnityEditor;
using UnityEngine;

namespace ShipDock.Editors
{
    public class ABAssetCreater
    {
        private List<string> mNameNode;

        public ABAssetCreater(string nameSource)
        {
            mNameNode = new List<string>();

            ABNameSourse = nameSource;
        }

        public string GetABName()
        {
            string node;
            string[] splited = ABNameSourse?.Split(StringUtils.PATH_SYMBOL_CHAR);
            int max = splited.Length;
            for (int i = 0; i < max; i++)
            {
                node = splited[i];
                if (node.IndexOf(StringUtils.DOT, StringComparison.Ordinal) >= 0)
                {
                    return mNameNode.Joins(StringUtils.PATH_SYMBOL);
                }

                if (i <= 1)
                {
                    mNameNode.Add(node);
                }
                else
                {
                    return mNameNode.Joins(StringUtils.PATH_SYMBOL);
                }
            }
            return string.Empty;
        }

        public AssetImporter Importer { get; set; }
        public string ABNameSourse { get; private set; }
    }

    public class AssetBundleInfoPopupEditor : ShipDockEditor
    {
        public static AssetBundleInfoPopupEditor Popup()
        {
            InitEditorWindow<AssetBundleInfoPopupEditor>("资源包信息");//, new Rect(0, 0, 400, 400));
            return focusedWindow as AssetBundleInfoPopupEditor;
        }

        protected override void InitConfigFlagAndValues()
        {
            base.InitConfigFlagAndValues();

            //SetValueItem("abName", string.Empty);
            SetValueItem("ab_item_name", string.Empty);
            //SetValueItem("abPath", string.Empty);
        }

        protected override void ReadyClientValues()
        {
        }

        protected override void UpdateClientValues()
        {
        }

        protected override void CheckGUI()
        {
            base.CheckGUI();

            EditorGUILayout.Space();
            EditorGUILayout.BeginVertical();
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("资源包名称：");
            //ValueItemTextField("abName");
            //ValueItemTextField("abPath");
            EditorGUILayout.Space();
            EditorGUILayout.EndHorizontal();

            ResList = ShipDockEditorData.Instance.selections;

            CreateAssetItemWithButton();

            string abName = string.Empty;
            if (GUILayout.Button("Build"))
            {
                ShipDockEditorData editorData = ShipDockEditorData.Instance;

                editorData.ABCreaterMapper?.Clear();
                Utils.Reclaim(ref editorData.ABCreaterMapper, false);

                editorData.ABCreaterMapper = new KeyValueList<string, List<ABAssetCreater>>();
                CreateAssetImporters(ref abName, ref editorData.ABCreaterMapper);

                string output = editorData.outputRoot;//.Append(abPath);
                if (!Directory.Exists(output))
                {
                    Directory.CreateDirectory(output);
                }

                BuildAssetByCreater();
                //BuildPipeline.BuildAssetBundles(output, BuildAssetBundleOptions.None, editorData.buildPlatform);

                if (EditorUtility.DisplayDialog("提示", string.Format("资源打包完成!!!"), "OK"))
                {
                    AssetDatabase.Refresh();
                }
            }

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();
        }

        private void CreateAssetImporters(ref string abName, ref KeyValueList<string, List<ABAssetCreater>> mapper)
        {
            string path;
            string assetItemName;
            string relativeName;
            string starter = StringUtils.PATH_SYMBOL.Append(AppPaths.resDataRoot);

            FileInfo fileInfo;
            List<ABAssetCreater> list;
            int starterLen = starter.Length;
            int max = ResList.Length;
            for (int i = 0; i < max; i++)
            {
                assetItemName = GetValueItem("res_" + i).Value;
                relativeName = assetItemName.Replace("Assets/".Append(AppPaths.resDataRoot), string.Empty);
                path = AppPaths.DataPathResDataRoot.Append(relativeName);

                fileInfo = new FileInfo(path);
                if (fileInfo.Extension == ".cs")
                {
                    continue;
                }

                int index = path.IndexOf(starter, StringComparison.Ordinal);
                ABAssetCreater creater = new ABAssetCreater(path.Substring(index + starterLen));
                abName = creater.GetABName();

                if (mapper.ContainsKey(abName))
                {
                    list = mapper[abName];
                }
                else
                {
                    list = new List<ABAssetCreater>();
                    mapper[abName] = list;
                }
                creater.Importer = AssetImporter.GetAtPath(assetItemName);
                list.Add(creater);
            }
        }

        private void BuildAssetByCreater()
        {
            ShipDockEditorData editorData = ShipDockEditorData.Instance;

            string abName;
            string output;
            List<ABAssetCreater> list;

            int max = editorData.ABCreaterMapper.Size;
            List<string> abNames = editorData.ABCreaterMapper.Keys;
            List<List<ABAssetCreater>> creaters = editorData.ABCreaterMapper.Values;
            for (int i = 0; i < max; i++)
            {
                abName = abNames[i];
                list = creaters[i];
                int m = list.Count;
                for (int n = 0; n < m; n++)
                {
                    list[n].Importer.assetBundleName = abName;
                    Debug.Log(abName);
                }
                //output = editorData.outputRoot.Append(abName);
                //Debug.Log(output);
                //if (!Directory.Exists(output))
                //{
                //    Directory.CreateDirectory(output);
                //}

                //for (int n = 0; n < m; n++)
                //{
                //    list[n].Importer.assetBundleName = default;
                //}
            }
            BuildPipeline.BuildAssetBundles(editorData.outputRoot, BuildAssetBundleOptions.None, editorData.buildPlatform);

        }

        private void CreateAssetItemWithButton()
        {
            int max = ResList.Length;
            UnityEngine.Object item;
            string fieldValue;
            for (int i = 0; i < max; i++)
            {
                item = ResList[i];
                SetValueItem("res_" + i.ToString(), AssetDatabase.GetAssetPath(item));
                if (GUILayout.Button(ResList[i].name))
                {
                    fieldValue = GetValueItem("res_" + i).Value;
                    GetValueItem("ab_item_name")?.Change(fieldValue);
                }
            }
            ValueItemLabel("ab_item_name");
        }

        public UnityEngine.Object[] ResList { get; set; } = new UnityEngine.Object[0];
    }

}
