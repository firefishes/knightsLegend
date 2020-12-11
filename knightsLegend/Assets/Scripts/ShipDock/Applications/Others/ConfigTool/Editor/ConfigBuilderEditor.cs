using UnityEditor;
using static ShipDock.Applications.ExcelTool;

namespace ShipDock.Applications
{
    public static class ConfigBuilderEditor
    {
        /// <summary>
        /// 批量删除AB包文件
        /// </summary>
        [MenuItem("ShipDock/Generate configs")]
        public static void GenerateCLRBindingCode()
        {
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

            string[] strs = Selection.assetGUIDs;
            foreach (var item in strs)
            {
                string path = AssetDatabase.GUIDToAssetPath(item);
                CreateItemArrayWithExcel(path);
            }
            AssetDatabase.Refresh();
        }
    }
}
