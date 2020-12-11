using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;
using System.Collections.Generic;

namespace ShipDock.Applications
{
    public interface IAdapter : CrossBindingAdaptorType
    {
        ILRuntime.Runtime.Enviorment.AppDomain Appdomain { set; get; }
        void SetILInstance(ILTypeInstance value);
}

    public class ILRuntimeAdapter<T, D> : CrossBindingAdaptor where D : IAdapter, new()
    {
        //static CrossBindingFunctionInfo<int> mget_Value_0 = new CrossBindingFunctionInfo<int>("get_Value");
        //static CrossBindingMethodInfo<int> mset_Value_1 = new CrossBindingMethodInfo<int>("set_Value");
        //static CrossBindingMethodInfo<string> mTestVirtual_2 = new CrossBindingMethodInfo<string>("TestVirtual");
        //static CrossBindingMethodInfo<int> mTestAbstract_3 = new CrossBindingMethodInfo<int>("TestAbstract");

        private static readonly Dictionary<string, CrossBindingMethodInfo> crossBindingMethods = new Dictionary<string, CrossBindingMethodInfo>();

        public override Type BaseCLRType
        {
            get
            {
                return typeof(T);
            }
        }

        public override Type AdaptorType
        {
            get
            {
                return typeof(D);
            }
        }

        public ILRuntimeAdapter()
        {
            InitAdapter();
        }

        /// <summary>
        /// 覆盖此方法，设置适配器中需要跨域绑定的函数信息 CrossBindingFunctionInfo
        /// 用于覆盖适配目标类的成员方法
        /// </summary>
        protected virtual void InitAdapter()
        {
        }

        public override object CreateCLRInstance(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        {
            D result = new D
            {
                Appdomain = appdomain
            };
            result.SetILInstance(instance);
            return result;
        }

        public void SetCrossBindingMethodInfo(string name, CrossBindingMethodInfo info)
        {
            crossBindingMethods[name] = info;
        }

        public void GetCrossBindingMethodInfo(string name, out CrossBindingMethodInfo info)
        {
            info = crossBindingMethods[name];
        }

        protected bool VirtualMethod(IAdapter adapter, string name)
        {
            GetCrossBindingMethodInfo(name, out CrossBindingMethodInfo info);
            return info == default ? false : info.CheckShouldInvokeBase(adapter.ILInstance);
        }

        protected void AbstractMethod(IAdapter adapter, string name, out CrossBindingMethodInfo info)
        {
            GetCrossBindingMethodInfo(name, out info);
        }

        public string AdapterToString(IAdapter adapter)
        {
            IMethod m = adapter.Appdomain.ObjectType.GetMethod("ToString", 0);
            ILTypeInstance typeInstance = adapter.ILInstance;
            m = typeInstance.Type.GetVirtualMethod(m);
            if (m == null || m is ILMethod)
            {
                return typeInstance.ToString();
            }
            else
            {
                return typeInstance.Type.FullName;
            }
        }

        //public class Adapter<T> : CrossBindingAdaptorType
        //{
        //    ILRuntime.Runtime.Enviorment.AppDomain appdomain;

        //    public T RawInstance { get; set; }

        //    public Adapter()
        //    {
        //    }

        //    public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
        //    {
        //        this.appdomain = appdomain;
        //        ILInstance = instance;
        //    }

        //    public ILTypeInstance ILInstance { get; }


        //public override void TestVirtual(string str)
        //{
        //    if (mTestVirtual_2.CheckShouldInvokeBase(this.ILInstance))
        //        base.TestVirtual(str);
        //    else
        //        mTestVirtual_2.Invoke(this.ILInstance, str);
        //}

        //public override void TestAbstract(int gg)
        //{
        //    mTestAbstract_3.Invoke(ILInstance, gg);
        //}

        //public override int Value
        //{
        //    get
        //    {
        //        if (mget_Value_0.CheckShouldInvokeBase(this.ILInstance))
        //            return base.Value;
        //        else
        //            return mget_Value_0.Invoke(this.ILInstance);

        //    }
        //    set
        //    {
        //        if (mset_Value_1.CheckShouldInvokeBase(this.ILInstance))
        //            base.Value = value;
        //        else
        //            mset_Value_1.Invoke(this.ILInstance, value);

        //    }
        //}

        //public override string ToString()
        //{
        //    IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
        //    m = ILInstance.Type.GetVirtualMethod(m);
        //    if (m == null || m is ILMethod)
        //    {
        //        return ILInstance.ToString();
        //    }
        //    else
        //        return ILInstance.Type.FullName;
        //}
        //}
    }
}