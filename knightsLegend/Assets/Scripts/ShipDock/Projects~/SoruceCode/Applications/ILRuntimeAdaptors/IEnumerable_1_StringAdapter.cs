using System;
using System.Collections;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace ShipDock.Applications
{   
    public class IEnumerable_1_StringAdapter : CrossBindingAdaptor
    {
        static CrossBindingFunctionInfo<System.Collections.Generic.IEnumerator<System.String>> mGetEnumerator_0 = new CrossBindingFunctionInfo<System.Collections.Generic.IEnumerator<System.String>>("GetEnumerator");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(System.Collections.Generic.IEnumerable<System.String>);
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

        public class Adapter : System.Collections.Generic.IEnumerable<System.String>, CrossBindingAdaptorType
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

            public System.Collections.Generic.IEnumerator<System.String> GetEnumerator()
            {
                return mGetEnumerator_0.Invoke(this.instance);
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

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}

