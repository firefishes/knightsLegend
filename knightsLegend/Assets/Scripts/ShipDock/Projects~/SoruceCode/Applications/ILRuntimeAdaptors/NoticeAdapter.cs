using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace ShipDock.Applications
{   
    public class NoticeAdapter : CrossBindingAdaptor
    {
        static CrossBindingMethodInfo mToPool_0 = new CrossBindingMethodInfo("ToPool");
        static CrossBindingMethodInfo mDispose_1 = new CrossBindingMethodInfo("Dispose");
        static CrossBindingMethodInfo mPurge_2 = new CrossBindingMethodInfo("Purge");
        static CrossBindingMethodInfo mRevert_3 = new CrossBindingMethodInfo("Revert");
        static CrossBindingMethodInfo<System.Int32> mSetNoticeName_4 = new CrossBindingMethodInfo<System.Int32>("SetNoticeName");
        static CrossBindingFunctionInfo<System.Boolean> mget_IsRecivedNotice_5 = new CrossBindingFunctionInfo<System.Boolean>("get_IsRecivedNotice");
        static CrossBindingMethodInfo<System.Boolean> mset_IsRecivedNotice_6 = new CrossBindingMethodInfo<System.Boolean>("set_IsRecivedNotice");
        static CrossBindingFunctionInfo<System.Int32> mget_Name_7 = new CrossBindingFunctionInfo<System.Int32>("get_Name");
        static CrossBindingFunctionInfo<ShipDock.Notices.INotificationSender> mget_NotifcationSender_8 = new CrossBindingFunctionInfo<ShipDock.Notices.INotificationSender>("get_NotifcationSender");
        static CrossBindingMethodInfo<ShipDock.Notices.INotificationSender> mset_NotifcationSender_9 = new CrossBindingMethodInfo<ShipDock.Notices.INotificationSender>("set_NotifcationSender");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(ShipDock.Notices.Notice);
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

        public class Adapter : ShipDock.Notices.Notice, CrossBindingAdaptorType
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

            public override void ToPool()
            {
                if (mToPool_0.CheckShouldInvokeBase(this.instance))
                    base.ToPool();
                else
                    mToPool_0.Invoke(this.instance);
            }

            public override void Dispose()
            {
                if (mDispose_1.CheckShouldInvokeBase(this.instance))
                    base.Dispose();
                else
                    mDispose_1.Invoke(this.instance);
            }

            protected override void Purge()
            {
                if (mPurge_2.CheckShouldInvokeBase(this.instance))
                    base.Purge();
                else
                    mPurge_2.Invoke(this.instance);
            }

            public override void Revert()
            {
                if (mRevert_3.CheckShouldInvokeBase(this.instance))
                    base.Revert();
                else
                    mRevert_3.Invoke(this.instance);
            }

            public override void SetNoticeName(System.Int32 value)
            {
                if (mSetNoticeName_4.CheckShouldInvokeBase(this.instance))
                    base.SetNoticeName(value);
                else
                    mSetNoticeName_4.Invoke(this.instance, value);
            }

            public override System.Boolean IsRecivedNotice
            {
            get
            {
                if (mget_IsRecivedNotice_5.CheckShouldInvokeBase(this.instance))
                    return base.IsRecivedNotice;
                else
                    return mget_IsRecivedNotice_5.Invoke(this.instance);

            }
            set
            {
                if (mset_IsRecivedNotice_6.CheckShouldInvokeBase(this.instance))
                    base.IsRecivedNotice = value;
                else
                    mset_IsRecivedNotice_6.Invoke(this.instance, value);

            }
            }

            public override System.Int32 Name
            {
            get
            {
                if (mget_Name_7.CheckShouldInvokeBase(this.instance))
                    return base.Name;
                else
                    return mget_Name_7.Invoke(this.instance);

            }
            }

            public override ShipDock.Notices.INotificationSender NotifcationSender
            {
            get
            {
                if (mget_NotifcationSender_8.CheckShouldInvokeBase(this.instance))
                    return base.NotifcationSender;
                else
                    return mget_NotifcationSender_8.Invoke(this.instance);

            }
            set
            {
                if (mset_NotifcationSender_9.CheckShouldInvokeBase(this.instance))
                    base.NotifcationSender = value;
                else
                    mset_NotifcationSender_9.Invoke(this.instance, value);

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

