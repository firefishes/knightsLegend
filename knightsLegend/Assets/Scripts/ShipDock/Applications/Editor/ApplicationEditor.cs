#define G_LOG

using ShipDock.Applications;
using ShipDock.Loader;
using ShipDock.Tools;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace ShipDock.Editors
{
    public class ApplicationEditor : ShipDockEditor
    {
        [MenuItem("ShipDock/Applicaton Editor")]
        public static void Open()
        {
            InitEditorWindow<ApplicationEditor>("游戏客户端设置");
        }

        [MenuItem("ShipDock/Create Application")]
        public static void CreateApplication()
        {
            string name = "UIRoot";
            if (GameObject.Find(name) == default)
            {
                List<GameObject> result = new List<GameObject>();
                ShipDockEditorUtils.FindAssetInEditorProject(ref result, "UIRoot t:GameObject", @"Assets\Scripts\ShipDock\Applications\Prefabs");

                GameObject UIRoot = Instantiate(result[0]);
                UIRoot.name = name;
                Canvas canvas = UIRoot.GetComponentInChildren<Canvas>();

                CanvasScaler canvasScaler = UIRoot.GetComponentInChildren<CanvasScaler>();
                GraphicRaycaster graphicRaycaster = UIRoot.GetComponentInChildren<GraphicRaycaster>();

                GameObject target = GameObject.Find("UIRoot/EventSystem");
                EventSystem eventSystem = target.GetComponent<EventSystem>();
                StandaloneInputModule inputModule = target.GetComponent<StandaloneInputModule>();

                if (canvasScaler == default)
                {
                    canvas.gameObject.AddComponent<CanvasScaler>();
                }
                if (graphicRaycaster == default)
                {
                    canvas.gameObject.AddComponent<GraphicRaycaster>();
                }
                if (eventSystem == default)
                {
                    target.AddComponent<EventSystem>();
                }
                if (inputModule == default)
                {
                    target.AddComponent<StandaloneInputModule>();
                }
            }

            string gameSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            Transform p = ShipDockEditorUtils.CreateGameObjectWithComponent<ShipDockGame>(gameSceneName);
            ShipDockEditorUtils.CreateGameObjectWithComponent<AssetsPoolingComponent>("AssetPool", p);
            ShipDockEditorUtils.CreateGameObjectWithComponent<CustomAssetCoordinator>("CustomAsset", p);
            ShipDockEditorUtils.CreateGameObjectWithComponent<TesterComponent>("TesterAsserter", p);
        }

        private BuildTarget mABBuildTarget;

        public override void Preshow()
        {
            base.Preshow();
        }

        protected override void InitConfigFlagAndValues()
        {
            base.InitConfigFlagAndValues();


        }

        protected override void CheckGUI()
        {
            base.CheckGUI();

            EditorGUILayout.Space();
            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("AB资源包");
            EditorGUILayout.Space();

            UpdateABBuildPanelUI();

            EditorGUILayout.EndHorizontal();
        }

        private void UpdateABBuildPanelUI()
        {
            Func<BuildTarget, bool, string> buildABFunc = GetABBuildOutputFunc();
            if (GUILayout.Button("IOS"))
            {
                mApplyStrings["build_ab_output"] = buildABFunc(BuildTarget.iOS, false);
                //AssetBundleBuilder.OnABBuilt = OnBuildABFinished;
                ConfirmPopup("IOS AB", "你确定？", AssetBundleBuilder.BuildIOSAB, "就是干");
            }
            if (GUILayout.Button("OSX"))
            {
                mApplyStrings["build_ab_output"] = buildABFunc(BuildTarget.StandaloneOSX, false);
                //AssetBundleBuilder.OnABBuilt = OnBuildABFinished;
                ConfirmPopup("OSX AB", "你确定？", AssetBundleBuilder.BuildOSXAB, "就是干");
            }
            if (GUILayout.Button("ANDROID"))
            {
                mApplyStrings["build_ab_output"] = buildABFunc(BuildTarget.Android, false);
                //AssetBundleBuilder.OnABBuilt = OnBuildABFinished;
                ConfirmPopup("Android AB", "你确定？", AssetBundleBuilder.BuildAndroidAB, "就是干");
            }
            if (GUILayout.Button("WIN"))
            {
                mApplyStrings["build_ab_output"] = buildABFunc(BuildTarget.StandaloneWindows, false);
                //AssetBundleBuilder.OnABBuilt = OnBuildABFinished;
                ConfirmPopup("Win AB", "你确定？", AssetBundleBuilder.BuildWinAB, "就是干");
            }
            if (GUILayout.Button("WIN64"))
            {
                mApplyStrings["build_ab_output"] = buildABFunc(BuildTarget.StandaloneWindows64, false);
                //AssetBundleBuilder.OnABBuilt = OnBuildABFinished;
                ConfirmPopup("Win64 AB", "你确定？", AssetBundleBuilder.BuildWin64AB, "就是干");
            }
            if (GUILayout.Button("DELETE"))
            {
                ConfirmPopup("Delete all AB", "你确定？", AssetBundleBuilder.DelAssetBundle, "绝不反悔", log: "资源已删除");
            }
        }

        private void OnBuildABFinished()
        {
        }

        protected Func<BuildTarget, bool, string> GetABBuildOutputFunc()
        {
            Func<BuildTarget, bool, string> buildABFunc = (BuildTarget buildTarget, bool isFinish) =>
            {
                //HasBuildAbOutput = true;
                mABBuildTarget = buildTarget;
                return (isFinish) ? mABBuildTarget.ToString().Append("资源打包完成...(*^o^*)") : "即将开始资源打包，平台：".Append(mABBuildTarget.ToString(), "\n");
            };
            return buildABFunc;
        }

        protected override void ReadyClientValues()
        {
        }

        protected override void UpdateClientValues()
        {
        }
    }

}
