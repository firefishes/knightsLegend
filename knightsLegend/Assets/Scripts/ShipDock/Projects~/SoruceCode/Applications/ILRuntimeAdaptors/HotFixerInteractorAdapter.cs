using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace ShipDock.Applications
{   
    public class HotFixerInteractorAdapter : CrossBindingAdaptor
    {
        static CrossBindingMethodInfo mRelease_0 = new CrossBindingMethodInfo("Release");
        static CrossBindingMethodInfo<ShipDock.Applications.HotFixerUI, ShipDock.Applications.HotFixerUIAgent> mInitInteractor_1 = new CrossBindingMethodInfo<ShipDock.Applications.HotFixerUI, ShipDock.Applications.HotFixerUIAgent>("InitInteractor");
        static CrossBindingMethodInfo<System.Int32, ShipDock.Notices.INoticeBase<System.Int32>> mDispatch_2 = new CrossBindingMethodInfo<System.Int32, ShipDock.Notices.INoticeBase<System.Int32>>("Dispatch");
        //static CrossBindingMethodInfo<System.Boolean, System.Action<System.Boolean>> mHotFixUIExit_3 = new CrossBindingMethodInfo<System.Boolean, System.Action<System.Boolean>>("HotFixUIExit");
        static CrossBindingMethodInfo mUpdateInteractor_4 = new CrossBindingMethodInfo("UpdateInteractor");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(ShipDock.Applications.HotFixerInteractor);
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

        public class Adapter : ShipDock.Applications.HotFixerInteractor, CrossBindingAdaptorType
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

            public override void Release()
            {
                if (mRelease_0.CheckShouldInvokeBase(this.instance))
                    base.Release();
                else
                    mRelease_0.Invoke(this.instance);
            }

            public override void InitInteractor(ShipDock.Applications.HotFixerUI UIOwner, ShipDock.Applications.HotFixerUIAgent agent)
            {
                if (mInitInteractor_1.CheckShouldInvokeBase(this.instance))
                    base.InitInteractor(UIOwner, agent);
                else
                    mInitInteractor_1.Invoke(this.instance, UIOwner, agent);
            }

            public override void Dispatch(System.Int32 name, ShipDock.Notices.INoticeBase<System.Int32> param)
            {
                if (mDispatch_2.CheckShouldInvokeBase(this.instance))
                    base.Dispatch(name, param);
                else
                    mDispatch_2.Invoke(this.instance, name, param);
            }

            //public override void HotFixUIExit(System.Boolean isDestroy, System.Action<System.Boolean> onUIStackExit)
            //{
            //    if (mHotFixUIExit_3.CheckShouldInvokeBase(this.instance))
            //        base.HotFixUIExit(isDestroy, onUIStackExit);
            //    else
            //        mHotFixUIExit_3.Invoke(this.instance, isDestroy, onUIStackExit);
            //}

            public override void UpdateInteractor()
            {
                mUpdateInteractor_4.Invoke(this.instance);
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

