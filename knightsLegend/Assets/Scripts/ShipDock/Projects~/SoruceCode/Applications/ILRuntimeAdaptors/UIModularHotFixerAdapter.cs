using System;
using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;

namespace ShipDock.Applications
{
    public class UIModularHotFixerAdapter : CrossBindingAdaptor
    {
        static CrossBindingFunctionInfo<System.Int32[]> mget_DataProxyLinks_0 = new CrossBindingFunctionInfo<System.Int32[]>("get_DataProxyLinks");
        static CrossBindingMethodInfo<System.Int32[]> mset_DataProxyLinks_1 = new CrossBindingMethodInfo<System.Int32[]>("set_DataProxyLinks");
        static CrossBindingMethodInfo mDispose_2 = new CrossBindingMethodInfo("Dispose");
        static CrossBindingMethodInfo<ShipDock.Datas.IDataProxy, System.Int32> mOnDataProxyNotify_3 = new CrossBindingMethodInfo<ShipDock.Datas.IDataProxy, System.Int32>("OnDataProxyNotify");
        static CrossBindingMethodInfo<ShipDock.Notices.INoticeBase<System.Int32>> mUIModularHandler_4 = new CrossBindingMethodInfo<ShipDock.Notices.INoticeBase<System.Int32>>("UIModularHandler");
        static CrossBindingMethodInfo mInit_5 = new CrossBindingMethodInfo("Init");
        static CrossBindingMethodInfo mEnter_6 = new CrossBindingMethodInfo("Enter");
        static CrossBindingMethodInfo<System.Boolean> mExit_7 = new CrossBindingMethodInfo<System.Boolean>("Exit");
        static CrossBindingMethodInfo mRenew_8 = new CrossBindingMethodInfo("Renew");
        static CrossBindingFunctionInfo<System.Boolean> mget_IsStackable_9 = new CrossBindingFunctionInfo<System.Boolean>("get_IsStackable");
        static CrossBindingFunctionInfo<System.String> mget_ABName_10 = new CrossBindingFunctionInfo<System.String>("get_ABName");
        static CrossBindingFunctionInfo<System.Int32> mget_UILayer_11 = new CrossBindingFunctionInfo<System.Int32>("get_UILayer");
        static CrossBindingMethodInfo<System.Int32> mset_UILayer_12 = new CrossBindingMethodInfo<System.Int32>("set_UILayer");
        static CrossBindingMethodInfo mShowUI_13 = new CrossBindingMethodInfo("ShowUI");
        static CrossBindingMethodInfo mHideUI_14 = new CrossBindingMethodInfo("HideUI");
        static CrossBindingFunctionInfo<System.Boolean> mget_IsExited_15 = new CrossBindingFunctionInfo<System.Boolean>("get_IsExited");
        static CrossBindingFunctionInfo<System.Boolean> mget_IsStackAdvanced_16 = new CrossBindingFunctionInfo<System.Boolean>("get_IsStackAdvanced");
        static CrossBindingFunctionInfo<System.String> mget_UIAssetName_17 = new CrossBindingFunctionInfo<System.String>("get_UIAssetName");
        static CrossBindingMethodInfo<System.String> mset_UIAssetName_18 = new CrossBindingMethodInfo<System.String>("set_UIAssetName");
        static CrossBindingFunctionInfo<System.String> mget_Name_19 = new CrossBindingFunctionInfo<System.String>("get_Name");
        static CrossBindingMethodInfo<System.String> mset_Name_20 = new CrossBindingMethodInfo<System.String>("set_Name");
        static CrossBindingMethodInfo mInterrupt_21 = new CrossBindingMethodInfo("Interrupt");
        static CrossBindingMethodInfo mResetAdvance_22 = new CrossBindingMethodInfo("ResetAdvance");
        static CrossBindingMethodInfo mStackAdvance_23 = new CrossBindingMethodInfo("StackAdvance");
        public override Type BaseCLRType
        {
            get
            {
                return typeof(ShipDock.Applications.UIModularHotFixer);
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

        public class Adapter : ShipDock.Applications.UIModularHotFixer, CrossBindingAdaptorType
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
                if (mDispose_2.CheckShouldInvokeBase(this.instance))
                    base.Dispose();
                else
                    mDispose_2.Invoke(this.instance);
            }

            public override void OnDataProxyNotify(ShipDock.Datas.IDataProxy data, System.Int32 keyName)
            {
                if (mOnDataProxyNotify_3.CheckShouldInvokeBase(this.instance))
                    base.OnDataProxyNotify(data, keyName);
                else
                    mOnDataProxyNotify_3.Invoke(this.instance, data, keyName);
            }

            protected override void UIModularHandler(ShipDock.Notices.INoticeBase<System.Int32> param)
            {
                if (mUIModularHandler_4.CheckShouldInvokeBase(this.instance))
                    base.UIModularHandler(param);
                else
                    mUIModularHandler_4.Invoke(this.instance, param);
            }

            protected override void ShowUI()
            {
                if (mShowUI_13.CheckShouldInvokeBase(this.instance))
                    base.ShowUI();
                else
                    mShowUI_13.Invoke(this.instance);
            }

            protected override void HideUI()
            {
                if (mHideUI_14.CheckShouldInvokeBase(this.instance))
                    base.HideUI();
                else
                    mHideUI_14.Invoke(this.instance);
            }

            public override void Interrupt()
            {
                if (mInterrupt_21.CheckShouldInvokeBase(this.instance))
                    base.Interrupt();
                else
                    mInterrupt_21.Invoke(this.instance);
            }

            public override void ResetAdvance()
            {
                if (mResetAdvance_22.CheckShouldInvokeBase(this.instance))
                    base.ResetAdvance();
                else
                    mResetAdvance_22.Invoke(this.instance);
            }

            public override void StackAdvance()
            {
                if (mStackAdvance_23.CheckShouldInvokeBase(this.instance))
                    base.StackAdvance();
                else
                    mStackAdvance_23.Invoke(this.instance);
            }

            public override System.Int32[] DataProxyLinks
            {
                get
                {
                    if (mget_DataProxyLinks_0.CheckShouldInvokeBase(this.instance))
                        return base.DataProxyLinks;
                    else
                        return mget_DataProxyLinks_0.Invoke(this.instance);

                }
                set
                {
                    if (mset_DataProxyLinks_1.CheckShouldInvokeBase(this.instance))
                        base.DataProxyLinks = value;
                    else
                        mset_DataProxyLinks_1.Invoke(this.instance, value);

                }
            }

            public override System.Boolean IsStackable
            {
                get
                {
                    if (mget_IsStackable_9.CheckShouldInvokeBase(this.instance))
                        return base.IsStackable;
                    else
                        return mget_IsStackable_9.Invoke(this.instance);

                }
            }

            public override System.String ABName
            {
                get
                {
                    if (mget_ABName_10.CheckShouldInvokeBase(this.instance))
                        return base.ABName;
                    else
                        return mget_ABName_10.Invoke(this.instance);

                }
            }

            public override System.Int32 UILayer
            {
                get
                {
                    if (mget_UILayer_11.CheckShouldInvokeBase(this.instance))
                        return base.UILayer;
                    else
                        return mget_UILayer_11.Invoke(this.instance);

                }
                protected set
                {
                    if (mset_UILayer_12.CheckShouldInvokeBase(this.instance))
                        base.UILayer = value;
                    else
                        mset_UILayer_12.Invoke(this.instance, value);

                }
            }

            public override System.Boolean IsExited
            {
                get
                {
                    if (mget_IsExited_15.CheckShouldInvokeBase(this.instance))
                        return base.IsExited;
                    else
                        return mget_IsExited_15.Invoke(this.instance);

                }
            }

            public override System.Boolean IsStackAdvanced
            {
                get
                {
                    if (mget_IsStackAdvanced_16.CheckShouldInvokeBase(this.instance))
                        return base.IsStackAdvanced;
                    else
                        return mget_IsStackAdvanced_16.Invoke(this.instance);

                }
            }

            public override System.String UIAssetName
            {
                get
                {
                    if (mget_UIAssetName_17.CheckShouldInvokeBase(this.instance))
                        return base.UIAssetName;
                    else
                        return mget_UIAssetName_17.Invoke(this.instance);

                }
                protected set
                {
                    if (mset_UIAssetName_18.CheckShouldInvokeBase(this.instance))
                        base.UIAssetName = value;
                    else
                        mset_UIAssetName_18.Invoke(this.instance, value);

                }
            }

            public override System.String Name
            {
                get
                {
                    if (mget_Name_19.CheckShouldInvokeBase(this.instance))
                        return base.Name;
                    else
                        return mget_Name_19.Invoke(this.instance);

                }
                protected set
                {
                    if (mset_Name_20.CheckShouldInvokeBase(this.instance))
                        base.Name = value;
                    else
                        mset_Name_20.Invoke(this.instance, value);

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

