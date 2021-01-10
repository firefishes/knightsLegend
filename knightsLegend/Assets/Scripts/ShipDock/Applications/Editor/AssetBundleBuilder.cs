#define SHOW_MENU_IN_EDITOR

using ShipDock.Applications;
using System;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace ShipDock.Editors
{
    public class AssetBundleBuilder
    {
        public Action OnABBuilt;

        /// <summary>
        /// 批量删除AB包文件
        /// </summary>
        [MenuItem("ShipDock/Delete Asset Bundles")]
        public static void DelAssetBundle()
        {

            DeleteABRes(AppPaths.StreamingResDataRoot);

            string resRoot = AppPaths.StreamingResDataRoot;
            string platformRoot = GetSuffix(BuildTarget.Android);
            DeleteABRes(resRoot + platformRoot);
            platformRoot = GetSuffix(BuildTarget.iOS);
            DeleteABRes(resRoot + platformRoot);
            platformRoot = GetSuffix(BuildTarget.StandaloneOSX);
            DeleteABRes(resRoot + platformRoot);
            platformRoot = GetSuffix(BuildTarget.StandaloneWindows);
            DeleteABRes(resRoot + platformRoot);
            platformRoot = GetSuffix(BuildTarget.StandaloneWindows64);
            DeleteABRes(resRoot + platformRoot);
        }

        /// <summary>
        /// 批量清楚AB名称
        /// </summary>
        [MenuItem("ShipDock/Clear Asset Bundles")]
        public static void ClearAssetBundleName()
        {
            // UnityEngine.Object[] arr = Selection.GetFiltered(typeof(UnityEngine.Object), SelectionMode.TopLevel);
            int length = AssetDatabase.GetAllAssetBundleNames().Length;
            string[] oldAssetBundleNames = new string[length];
            for (int i = 0; i < length; i++)
            {
                oldAssetBundleNames[i] = AssetDatabase.GetAllAssetBundleNames()[i];
            }
            for (int j = 0; j < oldAssetBundleNames.Length; j++)
            {
                AssetDatabase.RemoveAssetBundleName(oldAssetBundleNames[j], true);
            }
            length = AssetDatabase.GetAllAssetBundleNames().Length;
        }

        #region 资源打包相关
#if SHOW_MENU_IN_EDITOR
        [MenuItem("ShipDock/Build Asset Bundle/IOS")]
#endif
        public static void BuildIOSAB()
        {
            Debug.Log("Start build IOS asset bundles");
            AssetBundleBuilder builder = new AssetBundleBuilder();
            builder.BuildAssetBundle(BuildTarget.iOS);
        }

#if SHOW_MENU_IN_EDITOR
        [MenuItem("ShipDock/Build Asset Bundle/ANDROID")]
#endif
        public static void BuildAndroidAB()
        {
            Debug.Log("Start build Android asset bundles");
            AssetBundleBuilder builder = new AssetBundleBuilder();
            builder.BuildAssetBundle(BuildTarget.Android);
        }

#if SHOW_MENU_IN_EDITOR
        [MenuItem("ShipDock/Build Asset Bundle/OSX")]
#endif
        public static void BuildOSXAB()
        {
            Debug.Log("Start build OSX asset bundles");
            AssetBundleBuilder builder = new AssetBundleBuilder();
            builder.BuildAssetBundle(BuildTarget.StandaloneOSX);
        }

#if SHOW_MENU_IN_EDITOR
        [MenuItem("ShipDock/Build Asset Bundle/WIN")]
#endif
        public static void BuildWinAB()
        {
            Debug.Log("Start build Win asset bundles");
            AssetBundleBuilder builder = new AssetBundleBuilder();
            builder.BuildAssetBundle(BuildTarget.StandaloneWindows);
        }

#if SHOW_MENU_IN_EDITOR
        [MenuItem("ShipDock/Build Asset Bundle/WIN64")]
#endif
        public static void BuildWin64AB()
        {
            Debug.Log("Start build win64 asset bundles");
            AssetBundleBuilder builder = new AssetBundleBuilder();
            builder.BuildAssetBundle(BuildTarget.StandaloneWindows64);
        }
        #endregion

        public static string GetSuffix(BuildTarget buildPlatform)
        {
            string result;
            switch (buildPlatform)
            {
                case BuildTarget.Android:
                    result = "_ANDROID/";
                    break;
                case BuildTarget.iOS:
                    result = "_IOS/";
                    break;
                case BuildTarget.StandaloneWindows:
                    result = "_WIN/";
                    break;
                case BuildTarget.StandaloneWindows64:
                    result = "_WIN64/";
                    break;
                case BuildTarget.StandaloneOSX:
                    result = "_OSX/";
                    break;
                default:
                    result = "_UNKNOWN";
                    break;
            }
            return result;
        }

        /// <summary>
        /// 资源打包
        /// </summary>
        public void BuildAssetBundle(BuildTarget buildPlatform)
        {
            AssetDatabase.RemoveUnusedAssetBundleNames();
            string rootPath = AppPaths.DataPathResDataRoot;
            if (!Directory.Exists(rootPath))
            {
                EditorUtility.DisplayDialog("警告！", "所需资源目录不存在：" + rootPath, "确定");
                return;
            }
            string[] assetLabelRoots = Directory.GetDirectories(rootPath);
            if (assetLabelRoots.Length == 0)
            {
                EditorUtility.DisplayDialog("提示！", "没有需要打包的资源！！！", "确定");
                return;
            }

            ShipDockEditorData editorData = ShipDockEditorData.Instance;
            editorData.outputRoot = AppPaths.ABBuildOutputRoot;
            editorData.platformPath = GetSuffix(buildPlatform);
            editorData.buildPlatform = buildPlatform;

            UnityEngine.Object[] selections = Selection.GetFiltered<UnityEngine.Object>(SelectionMode.DeepAssets);
            ShipDockEditorData.Instance.selections = selections;
            AssetBundleInfoPopupEditor.Popup();
        }

        private static void DeleteABRes(string strNeedDeleteDIR)
        {
            if (!string.IsNullOrEmpty(strNeedDeleteDIR))
            {
                if (!Directory.Exists(strNeedDeleteDIR))
                {
                    return;
                }
                if (File.Exists(strNeedDeleteDIR + ".meta"))
                {
                    File.Delete(strNeedDeleteDIR + ".meta");
                }
                Directory.Delete(strNeedDeleteDIR, true);//注意： 这里参数"true"表示可以删除非空目录
                AssetDatabase.Refresh();//刷新
            }
        }
    }
}
