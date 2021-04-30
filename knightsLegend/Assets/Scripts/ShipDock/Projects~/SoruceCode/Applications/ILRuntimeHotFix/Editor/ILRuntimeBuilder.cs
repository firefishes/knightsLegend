#define OPEN_HOTFIX_EDITOR
#define _OPEN_GENERATE_ADAPTER_EDITOR
#define DEF_APPLICATION_SETTING_BASE_ADAPTER

#if OPEN_HOTFIX_EDITOR
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using ShipDock.Applications;
using System;
using System.Configuration;
using System.IO;
using UnityEditor;
using AppDomain = ILRuntime.Runtime.Enviorment.AppDomain;

namespace ShipDock.Editors
{
    [System.Reflection.Obfuscation(Exclude = true)]
    public class HotFixBaseILRuntimeBuilder
    {
        private static AppDomain AppDomain()
        {
            return ILRuntimeAppEditor.GetInstance().ILRuntimeHotFix.ILAppDomain;
        }

        private static void CommonForBindingCode(Action<AppDomain, string> callback)
        {
            ILRuntimeHotFix.InitInEditor();

            ILRuntimeAppEditor app = ILRuntimeAppEditor.GetInstance();

            AppDomain domain = AppDomain();
            string[] strs = Selection.assetGUIDs;
            foreach (var item in strs)
            {
                string path = AssetDatabase.GUIDToAssetPath(item);
                callback.Invoke(domain, path);
            }
            AssetDatabase.Refresh();
        }

        /// <summary>
        /// 批量删除AB包文件
        /// </summary>
        [MenuItem("ShipDock/Generate CLR Binding Code by Analysis")]
        public static void GenerateCLRBindingCode()
        {
            CommonForBindingCode(BindingAll);
        }

        private static void BindingAll(AppDomain domain, string path)
        {
            string generatedRoot = "Assets/Scripts/HotFixGenerated";
            string info = string.Format("即将分析并生成热更脚本中的CLR绑定代码，是否继续？\r\n\r\n已选中的热更文件：{0}\r\n\r\n 生成位置：{1}", path, generatedRoot);

            if (EditorUtility.DisplayDialog("提示", info, "必须的", "我再想想"))
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    domain.LoadAssembly(fs);

                    ILRuntimeAppEditor app = ILRuntimeAppEditor.GetInstance();
#if DEF_APPLICATION_SETTING_BASE_ADAPTER
                    domain.RegisterCrossBindingAdaptor(new ApplicationSettingsBaseAdapter());
#endif
                    app.ILRuntimeHotFix.Start();
                    ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(domain, generatedRoot);
                }
            }
        }

        [MenuItem("ShipDock/Generate Cross Binding Adapter")]
        public static void GenerateAdapter()
        {
            if (EditorUtility.DisplayDialog("提示", "即将生成跨域调用的绑定适配器，是否继续？", "必须的", "我再想想"))
            {
                CommonForBindingCode(OnGenerateAdapter);
            }
        }

        private static void OnGenerateAdapter(AppDomain domain, string path)
        {
            ILRuntimeAppEditor app = ILRuntimeAppEditor.GetInstance();
            IHotFixConfig config = app.GetHotFixConfig();

            string[] splits = path.Split('.');
            splits = splits[0].Split('/');
            string name = splits[splits.Length - 1];
            string code = string.Empty;
            var className = config.AutoAdapterGenerates;
            bool hasAdapterConfig = false;
            foreach(var item in className)
            {
                if (item.Key == name)
                {
                    hasAdapterConfig = true;
                    if (path.Contains(".cs"))
                    {
                        using (StreamWriter sw = new StreamWriter(path))
                        {
                            sw.WriteLine(CrossBindingCodeGenerator.GenerateCrossBindingAdapterCode(item.Value, config.SpaceName));
                        }
                    }
                    else
                    {
                        string info = string.Format("选定的文件 {0} 不是cs代码文件", path);
                        EditorUtility.DisplayDialog("生成错误", info, "朕知道了");
                        throw new Exception(info);
                    }
                }
            }
            if (!hasAdapterConfig)
            {
                string info = "选定的文件可能未包含在适配器的配置中，请通过修改 AppHotFixConfigBase 或其子类中的 GetAutoAdapterGenerates() 方法增加定义之后重试";
                EditorUtility.DisplayDialog("生成错误", info, "朕知道了");
                throw new Exception(info);
            }
        }
    }

#if DEF_APPLICATION_SETTING_BASE_ADAPTER
    public class ApplicationSettingsBaseAdapter : ILRuntimeAdapter<ApplicationSettingsBase, ApplicationSettingsBaseAdapter.Adapter>
    {
        public class Adapter : IAdapter
        {
            public ILRuntime.Runtime.Enviorment.AppDomain Appdomain { get; set; }

            public ILTypeInstance ILInstance { get; private set; }

            public void SetILInstance(ILTypeInstance value)
            {
                ILInstance = value;
            }
        }
    }
#endif
}
#endif