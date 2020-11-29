using ILRuntime.Mono.Cecil.Pdb;
using ILRuntime.Runtime.Enviorment;
using System.IO;

namespace ShipDock.Applications
{
    public class ILRuntimeIniter
    {
        public static bool ApplySingleDomain { get; set; }

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

        public void Build(byte[] dll, byte[] pdb)
        {
            if (ApplySingleDomain)
            {
                return;
            }

            mDllMemeoryStream = new MemoryStream(dll);
            mPdbMemeoryStream = new MemoryStream(pdb);

            ILRuntimeDomain.LoadAssembly(mDllMemeoryStream, mPdbMemeoryStream, new PdbReaderProvider());

        }
    }
}