using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace ShipDock.Applications
{   
    public class ValueItemAdapter : CrossBindingAdaptor
    {
        static CrossBindingFunctionInfo<System.Int32> mget_Int_0 = new CrossBindingFunctionInfo<System.Int32>("get_Int");
        static CrossBindingMethodInfo<System.Int32> mset_Int_1 = new CrossBindingMethodInfo<System.Int32>("set_Int");
        static CrossBindingFunctionInfo<System.Single> mget_Float_2 = new CrossBindingFunctionInfo<System.Single>("get_Float");
        static CrossBindingMethodInfo<System.Single> mset_Float_3 = new CrossBindingMethodInfo<System.Single>("set_Float");
        static CrossBindingFunctionInfo<System.Double> mget_Double_4 = new CrossBindingFunctionInfo<System.Double>("get_Double");
        static CrossBindingMethodInfo<System.Double> mset_Double_5 = new CrossBindingMethodInfo<System.Double>("set_Double");
        static CrossBindingFunctionInfo<System.Boolean> mget_Bool_6 = new CrossBindingFunctionInfo<System.Boolean>("get_Bool");
        static CrossBindingMethodInfo<System.Boolean> mset_Bool_7 = new CrossBindingMethodInfo<System.Boolean>("set_Bool");
        static CrossBindingFunctionInfo<System.Int32> mget_Type_8 = new CrossBindingFunctionInfo<System.Int32>("get_Type");
        static CrossBindingMethodInfo<System.Int32> mset_Type_9 = new CrossBindingMethodInfo<System.Int32>("set_Type");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(ShipDock.Tools.ValueItem);
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

        public class Adapter : ShipDock.Tools.ValueItem, CrossBindingAdaptorType
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

            public override System.Int32 Int
            {
            get
            {
                if (mget_Int_0.CheckShouldInvokeBase(this.instance))
                    return base.Int;
                else
                    return mget_Int_0.Invoke(this.instance);

            }
            set
            {
                if (mset_Int_1.CheckShouldInvokeBase(this.instance))
                    base.Int = value;
                else
                    mset_Int_1.Invoke(this.instance, value);

            }
            }

            public override System.Single Float
            {
            get
            {
                if (mget_Float_2.CheckShouldInvokeBase(this.instance))
                    return base.Float;
                else
                    return mget_Float_2.Invoke(this.instance);

            }
            set
            {
                if (mset_Float_3.CheckShouldInvokeBase(this.instance))
                    base.Float = value;
                else
                    mset_Float_3.Invoke(this.instance, value);

            }
            }

            public override System.Double Double
            {
            get
            {
                if (mget_Double_4.CheckShouldInvokeBase(this.instance))
                    return base.Double;
                else
                    return mget_Double_4.Invoke(this.instance);

            }
            set
            {
                if (mset_Double_5.CheckShouldInvokeBase(this.instance))
                    base.Double = value;
                else
                    mset_Double_5.Invoke(this.instance, value);

            }
            }

            public override System.Boolean Bool
            {
            get
            {
                if (mget_Bool_6.CheckShouldInvokeBase(this.instance))
                    return base.Bool;
                else
                    return mget_Bool_6.Invoke(this.instance);

            }
            set
            {
                if (mset_Bool_7.CheckShouldInvokeBase(this.instance))
                    base.Bool = value;
                else
                    mset_Bool_7.Invoke(this.instance, value);

            }
            }

            public override System.Int32 Type
            {
            get
            {
                if (mget_Type_8.CheckShouldInvokeBase(this.instance))
                    return base.Type;
                else
                    return mget_Type_8.Invoke(this.instance);

            }
            set
            {
                if (mset_Type_9.CheckShouldInvokeBase(this.instance))
                    base.Type = value;
                else
                    mset_Type_9.Invoke(this.instance, value);

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

