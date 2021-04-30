using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static ShipDock.Editors.ExcelTool;

namespace ShipDock.Editors
{
    public class ConfigBuilderPopupEditor : ShipDockEditor
    {
        /// <summary>
        /// 批量删除AB包文件
        /// </summary>
        public static void GenerateConfigCodeAndFile()
        {
            Popup();
        }

        [MenuItem("ShipDock/Generate Configs")]
        public static ConfigBuilderPopupEditor Popup()
        {
            InitEditorWindow<ConfigBuilderPopupEditor>("构建配置文件");
            return focusedWindow as ConfigBuilderPopupEditor;
        }

        protected override void ReadyClientValues()
        {
        }

        protected override void UpdateClientValues() { }

        protected override void CheckGUI()
        {
            base.CheckGUI();

            Setting = new ExeclSetting
            {
                classDefine = new ExeclDefination
                {
                    row = 0,
                    column = 0,
                },
                classType = new ExeclDefination
                {
                    row = 0,
                    column = 1,
                },
                IDFieldName = new ExeclDefination
                {
                    row = 0,
                    column = 2,
                },
                dataStart = new ExeclDefination
                {
                    row = 5,
                    column = 1,
                }
            };

            if (GUILayout.Button("Build Config To Res Version"))
            {
                string path, relativeName;
                string[] strs = Selection.assetGUIDs;
                List<string> relativeNames = new List<string>();
                foreach (var item in strs)
                {
                    path = AssetDatabase.GUIDToAssetPath(item);
                    CreateItemArrayWithExcel(path, out relativeName);
                    relativeNames.Add(relativeName.ToLower());
                }
                AssetDatabase.Refresh();
            }
        }
    }
}
