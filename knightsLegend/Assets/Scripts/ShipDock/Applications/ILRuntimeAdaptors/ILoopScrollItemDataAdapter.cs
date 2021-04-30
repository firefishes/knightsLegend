using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace ShipDock.Applications
{   
    public class ILoopScrollItemDataAdapter : CrossBindingAdaptor
    {
        class FillInfoToItem_0Info : CrossBindingMethodInfo
        {
            static Type[] pTypes = new Type[] {typeof(ShipDock.Applications.LoopScrollItem).MakeByRefType()};

            public FillInfoToItem_0Info()
                : base("FillInfoToItem")
            {

            }

            protected override Type ReturnType { get { return null; } }

            protected override Type[] Parameters { get { return pTypes; } }
            public void Invoke(ILTypeInstance instance, ref ShipDock.Applications.LoopScrollItem item)
            {
                EnsureMethod(instance);
                if (method != null)
                {
                    invoking = true;
                    try
                    {
                        using (var ctx = domain.BeginInvoke(method))
                        {
                            ctx.PushObject(item);
                            ctx.PushObject(instance);
                            ctx.PushReference(0);
                            ctx.Invoke();
                            item = ctx.ReadObject<ShipDock.Applications.LoopScrollItem>(0);
                        }
                    }
                    finally
                    {
                        invoking = false;
                    }
                }
            }

            public override void Invoke(ILTypeInstance instance)
            {
                throw new NotSupportedException();
            }
        }
        static FillInfoToItem_0Info mFillInfoToItem_0 = new FillInfoToItem_0Info();
        public override Type BaseCLRType
        {
            get
            {
                return typeof(ShipDock.Applications.ILoopScrollItemData);
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

        public class Adapter : ShipDock.Applications.ILoopScrollItemData, CrossBindingAdaptorType
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

            public void FillInfoToItem(ref ShipDock.Applications.LoopScrollItem item)
            {
                mFillInfoToItem_0.Invoke(this.instance, ref item);
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

