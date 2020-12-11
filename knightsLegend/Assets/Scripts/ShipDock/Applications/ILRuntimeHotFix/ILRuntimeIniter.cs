using ILRuntime.Mono.Cecil.Pdb;
using ILRuntime.Runtime.Enviorment;
using System.IO;

namespace ShipDock.Applications
{
    /// <summary>
    /// ILRuntime热更（dll代码）启动器
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class ILRuntimeIniter
    {
        /// <summary>是否应用单独一文件热更模式</summary>
        public static bool ApplySingleDomain { get; set; } = true;
        public static bool HasLoadAnyAssembly { get; set; }

        private MemoryStream mDllMemeoryStream;
        private MemoryStream mPdbMemeoryStream;

        public AppDomain ILRuntimeDomain { get; private set; }

        public ILRuntimeIniter(AppDomain appDomain)
        {
            ILRuntimeDomain = appDomain;
        }

        public void Clear()
        {
            ILRuntimeDomain = default;
            mDllMemeoryStream = default;
            mPdbMemeoryStream = default;
        }
        
        /// <summary>
        /// 创建热更代码数据
        /// </summary>
        /// <param name="dll"></param>
        /// <param name="pdb"></param>
        public void Build(byte[] dll, byte[] pdb)
        {
            if (ApplySingleDomain && HasLoadAnyAssembly)//单一热更包模式下，如已加载过热更资源则不做后续操作
            {
                return;
            }

            mDllMemeoryStream = new MemoryStream(dll);
            mPdbMemeoryStream = new MemoryStream(pdb);

            ILRuntimeDomain.LoadAssembly(mDllMemeoryStream, mPdbMemeoryStream, new PdbReaderProvider());

            HasLoadAnyAssembly = true;
        }
    }
}