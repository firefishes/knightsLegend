#if UNITY_EDITOR
using ILRuntime.Runtime.Enviorment;
using System.IO;
using UnityEditor;

[System.Reflection.Obfuscation(Exclude = true)]
public class ILRuntimeCLRBinding
{
    private const string ILRUNTIME_GENERATE_CLR_BINDING_CODE = "ILRuntime/Generate CLR Binding Code by Analysis";

   [MenuItem(ILRUNTIME_GENERATE_CLR_BINDING_CODE)]
    static void GenerateCLRBindingByAnalysis()
    {
        //用新的分析热更dll调用引用来生成绑定代码
        AppDomain domain = new AppDomain();
        
        using (FileStream fs = new FileStream("Assets/StreamingAssets/HotFix_Project.dll", FileMode.Open, FileAccess.Read))
        {
            domain.LoadAssembly(fs);

            //Crossbind Adapter is needed to generate the correct binding code
            InitILRuntime(domain);
            ILRuntime.Runtime.CLRBinding.BindingCodeGenerator.GenerateBindingCode(domain, "Assets/Samples/ILRuntime/Generated");
        }

        AssetDatabase.Refresh();
    }

    static void InitILRuntime(AppDomain domain)
    {
        //这里需要注册所有热更DLL中用到的跨域继承Adapter，否则无法正确抓取引用
        //domain.RegisterCrossBindingAdaptor(new MonoBehaviourAdapter());
        //domain.RegisterCrossBindingAdaptor(new CoroutineAdapter());
        //domain.RegisterCrossBindingAdaptor(new TestClassBaseAdapter());
        //domain.RegisterValueTypeBinder(typeof(Vector3), new Vector3Binder());
    }
}
#endif
