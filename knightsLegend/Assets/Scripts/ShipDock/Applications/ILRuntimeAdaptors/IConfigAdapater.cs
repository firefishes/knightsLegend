using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace ShipDock.Applications
{   
    public class IConfigAdapter : CrossBindingAdaptor
    {
        static CrossBindingFunctionInfo<System.Int32> mGetID_0 = new CrossBindingFunctionInfo<System.Int32>("GetID");
        static CrossBindingMethodInfo<ShipDock.Tools.ByteBuffer> mParse_1 = new CrossBindingMethodInfo<ShipDock.Tools.ByteBuffer>("Parse");
        static CrossBindingFunctionInfo<System.String> mget_CRCValue_2 = new CrossBindingFunctionInfo<System.String>("get_CRCValue");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(ShipDock.Config.IConfig);
            }
        }

        public override Type AdaptorType
        {
            get
            {
                return typeof(Adapter);
            }
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            return new Adapter(appdomain, instance);
        }

        public class Adapter : ShipDock.Config.IConfig, CrossBindingAdaptorType
        {
            ILTypeInstance instance;
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            public Adapter()
            {

            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.instance = instance;
            }

            public ILTypeInstance ILInstance { get { return instance; } }

            public System.Int32 GetID()
            {
                return mGetID_0.Invoke(this.instance);
            }

            public void Parse(ShipDock.Tools.ByteBuffer buffer)
            {
                mParse_1.Invoke(this.instance, buffer);
            }

            public System.String CRCValue
            {
            get
            {
                return mget_CRCValue_2.Invoke(this.instance);

            }
            }

            public override string ToString()
            {
                IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
                m = instance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    return instance.ToString();
                }
                else
                    return instance.Type.FullName;
            }
        }
    }
}

