using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace ShipDock.Applications
{   
    public class DataProxyAdapter : CrossBindingAdaptor
    {
        static CrossBindingMethodInfo mDispose_0 = new CrossBindingMethodInfo("Dispose");
        static CrossBindingMethodInfo<ShipDock.Datas.IDataExtracter> mRegister_1 = new CrossBindingMethodInfo<ShipDock.Datas.IDataExtracter>("Register");
        static CrossBindingMethodInfo<ShipDock.Datas.IDataExtracter> mUnregister_2 = new CrossBindingMethodInfo<ShipDock.Datas.IDataExtracter>("Unregister");
        static CrossBindingFunctionInfo<System.Int32> mget_DataName_3 = new CrossBindingFunctionInfo<System.Int32>("get_DataName");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(ShipDock.Datas.DataProxy);
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

        public class Adapter : ShipDock.Datas.DataProxy, CrossBindingAdaptorType
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

            public override void Dispose()
            {
                if (mDispose_0.CheckShouldInvokeBase(this.instance))
                    base.Dispose();
                else
                    mDispose_0.Invoke(this.instance);
            }

            public override void Register(ShipDock.Datas.IDataExtracter dataHandler)
            {
                if (mRegister_1.CheckShouldInvokeBase(this.instance))
                    base.Register(dataHandler);
                else
                    mRegister_1.Invoke(this.instance, dataHandler);
            }

            public override void Unregister(ShipDock.Datas.IDataExtracter dataHandler)
            {
                if (mUnregister_2.CheckShouldInvokeBase(this.instance))
                    base.Unregister(dataHandler);
                else
                    mUnregister_2.Invoke(this.instance, dataHandler);
            }

            public override System.Int32 DataName
            {
            get
            {
                if (mget_DataName_3.CheckShouldInvokeBase(this.instance))
                    return base.DataName;
                else
                    return mget_DataName_3.Invoke(this.instance);

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

