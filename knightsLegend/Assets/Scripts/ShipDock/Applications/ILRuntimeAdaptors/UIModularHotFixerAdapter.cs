using ILRuntime.CLR.Method;
using ILRuntime.Runtime.Enviorment;
using ILRuntime.Runtime.Intepreter;
using System;

namespace ShipDock.Applications
{
    public class UIModularHotFixerAdapter : CrossBindingAdaptor
    {
        static CrossBindingFunctionInfo<int[]> mget_DataProxyLinks_0 = new CrossBindingFunctionInfo<int[]>("get_DataProxyLinks");
        static CrossBindingMethodInfo mDispose_1 = new CrossBindingMethodInfo("Dispose");
        static CrossBindingMethodInfo<Datas.IDataProxy, int> mOnDataProxyNotify_2 = new CrossBindingMethodInfo<Datas.IDataProxy, int>("OnDataProxyNotify");
        static CrossBindingMethodInfo<Notices.INoticeBase<int>> mUIModularHandler_3 = new CrossBindingMethodInfo<Notices.INoticeBase<int>>("UIModularHandler");
        static CrossBindingMethodInfo mInit_4 = new CrossBindingMethodInfo("Init");
        static CrossBindingFunctionInfo<HotFixerInteractor> mGetInteractor_5 = new CrossBindingFunctionInfo<HotFixerInteractor>("GetInteractor");
        static CrossBindingFunctionInfo<bool> mget_IsStackable_6 = new CrossBindingFunctionInfo<bool>("get_IsStackable");
        static CrossBindingFunctionInfo<string> mget_ABName_7 = new CrossBindingFunctionInfo<string>("get_ABName");
        static CrossBindingFunctionInfo<int> mget_UILayer_8 = new CrossBindingFunctionInfo<int>("get_UILayer");
        static CrossBindingMethodInfo<int> mset_UILayer_9 = new CrossBindingMethodInfo<int>("set_UILayer");
        static CrossBindingMethodInfo mEnter_10 = new CrossBindingMethodInfo("Enter");
        static CrossBindingMethodInfo mRenew_11 = new CrossBindingMethodInfo("Renew");
        static CrossBindingMethodInfo mShowUI_12 = new CrossBindingMethodInfo("ShowUI");
        static CrossBindingMethodInfo mHideUI_13 = new CrossBindingMethodInfo("HideUI");
        static CrossBindingMethodInfo<bool> mExit_14 = new CrossBindingMethodInfo<bool>("Exit");
        static CrossBindingMethodInfo mInterrupt_15 = new CrossBindingMethodInfo("Interrupt");
        static CrossBindingMethodInfo mResetAdvance_16 = new CrossBindingMethodInfo("ResetAdvance");
        static CrossBindingMethodInfo mStackAdvance_17 = new CrossBindingMethodInfo("StackAdvance");
        static CrossBindingFunctionInfo<bool> mget_IsExited_18 = new CrossBindingFunctionInfo<bool>("get_IsExited");
        static CrossBindingFunctionInfo<bool> mget_IsStackAdvanced_19 = new CrossBindingFunctionInfo<bool>("get_IsStackAdvanced");
        static CrossBindingFunctionInfo<string> mget_UIAssetName_20 = new CrossBindingFunctionInfo<string>("get_UIAssetName");
        static CrossBindingMethodInfo<string> mset_UIAssetName_21 = new CrossBindingMethodInfo<string>("set_UIAssetName");
        static CrossBindingFunctionInfo<string> mget_Name_22 = new CrossBindingFunctionInfo<string>("get_Name");
        static CrossBindingMethodInfo<string> mset_Name_23 = new CrossBindingMethodInfo<string>("set_Name");

        public override Type BaseCLRType
        {
            get
            {
                return typeof(UIModularHotFixer);
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

        public class Adapter : UIModularHotFixer, CrossBindingAdaptorType
        {
            ILRuntime.Runtime.Enviorment.AppDomain appdomain;

            public Adapter()
            {

            }

            public Adapter(ILRuntime.Runtime.Enviorment.AppDomain appdomain, ILTypeInstance instance)
            {
                this.appdomain = appdomain;
                this.ILInstance = instance;
            }

            public ILTypeInstance ILInstance { get; }

            public override void Dispose()
            {
                if (mDispose_1.CheckShouldInvokeBase(this.ILInstance))
                    base.Dispose();
                else
                    mDispose_1.Invoke(this.ILInstance);
            }

            public override void OnDataProxyNotify(ShipDock.Datas.IDataProxy data, System.Int32 keyName)
            {
                if (mOnDataProxyNotify_2.CheckShouldInvokeBase(this.ILInstance))
                    base.OnDataProxyNotify(data, keyName);
                else
                    mOnDataProxyNotify_2.Invoke(this.ILInstance, data, keyName);
            }

            protected override void UIModularHandler(ShipDock.Notices.INoticeBase<System.Int32> param)
            {
                if (mUIModularHandler_3.CheckShouldInvokeBase(this.ILInstance))
                    base.UIModularHandler(param);
                else
                    mUIModularHandler_3.Invoke(this.ILInstance, param);
            }

            public override void Init()
            {
                if (mInit_4.CheckShouldInvokeBase(this.ILInstance))
                    base.Init();
                else
                    mInit_4.Invoke(this.ILInstance);
            }

            protected override ShipDock.Applications.HotFixerInteractor GetInteractor()
            {
                if (mGetInteractor_5.CheckShouldInvokeBase(this.ILInstance))
                    return base.GetInteractor();
                else
                    return mGetInteractor_5.Invoke(this.ILInstance);
            }

            public override void Enter()
            {
                if (mEnter_10.CheckShouldInvokeBase(this.ILInstance))
                    base.Enter();
                else
                    mEnter_10.Invoke(this.ILInstance);
            }

            public override void Renew()
            {
                if (mRenew_11.CheckShouldInvokeBase(this.ILInstance))
                    base.Renew();
                else
                    mRenew_11.Invoke(this.ILInstance);
            }

            protected override void ShowUI()
            {
                if (mShowUI_12.CheckShouldInvokeBase(this.ILInstance))
                    base.ShowUI();
                else
                    mShowUI_12.Invoke(this.ILInstance);
            }

            protected override void HideUI()
            {
                if (mHideUI_13.CheckShouldInvokeBase(this.ILInstance))
                    base.HideUI();
                else
                    mHideUI_13.Invoke(this.ILInstance);
            }

            public override void Exit(System.Boolean isDestroy)
            {
                if (mExit_14.CheckShouldInvokeBase(this.ILInstance))
                    base.Exit(isDestroy);
                else
                    mExit_14.Invoke(this.ILInstance, isDestroy);
            }

            public override void Interrupt()
            {
                if (mInterrupt_15.CheckShouldInvokeBase(this.ILInstance))
                    base.Interrupt();
                else
                    mInterrupt_15.Invoke(this.ILInstance);
            }

            public override void ResetAdvance()
            {
                if (mResetAdvance_16.CheckShouldInvokeBase(this.ILInstance))
                    base.ResetAdvance();
                else
                    mResetAdvance_16.Invoke(this.ILInstance);
            }

            public override void StackAdvance()
            {
                if (mStackAdvance_17.CheckShouldInvokeBase(this.ILInstance))
                    base.StackAdvance();
                else
                    mStackAdvance_17.Invoke(this.ILInstance);
            }

            public override System.Int32[] DataProxyLinks
            {
                get
                {
                    if (mget_DataProxyLinks_0.CheckShouldInvokeBase(this.ILInstance))
                        return base.DataProxyLinks;
                    else
                        return mget_DataProxyLinks_0.Invoke(this.ILInstance);

                }
            }

            public override System.Boolean IsStackable
            {
                get
                {
                    if (mget_IsStackable_6.CheckShouldInvokeBase(this.ILInstance))
                        return base.IsStackable;
                    else
                        return mget_IsStackable_6.Invoke(this.ILInstance);

                }
            }

            public override System.String ABName
            {
                get
                {
                    if (mget_ABName_7.CheckShouldInvokeBase(this.ILInstance))
                        return base.ABName;
                    else
                        return mget_ABName_7.Invoke(this.ILInstance);

                }
            }

            public override System.Int32 UILayer
            {
                get
                {
                    if (mget_UILayer_8.CheckShouldInvokeBase(this.ILInstance))
                        return base.UILayer;
                    else
                        return mget_UILayer_8.Invoke(this.ILInstance);

                }
                protected set
                {
                    if (mset_UILayer_9.CheckShouldInvokeBase(this.ILInstance))
                        base.UILayer = value;
                    else
                        mset_UILayer_9.Invoke(this.ILInstance, value);

                }
            }

            public override System.Boolean IsExited
            {
                get
                {
                    if (mget_IsExited_18.CheckShouldInvokeBase(this.ILInstance))
                        return base.IsExited;
                    else
                        return mget_IsExited_18.Invoke(this.ILInstance);

                }
            }

            public override bool IsStackAdvanced
            {
                get
                {
                    if (mget_IsStackAdvanced_19.CheckShouldInvokeBase(this.ILInstance))
                        return base.IsStackAdvanced;
                    else
                        return mget_IsStackAdvanced_19.Invoke(this.ILInstance);

                }
            }

            public override System.String UIAssetName
            {
                get
                {
                    if (mget_UIAssetName_20.CheckShouldInvokeBase(this.ILInstance))
                        return base.UIAssetName;
                    else
                        return mget_UIAssetName_20.Invoke(this.ILInstance);

                }
                protected set
                {
                    if (mset_UIAssetName_21.CheckShouldInvokeBase(this.ILInstance))
                        base.UIAssetName = value;
                    else
                        mset_UIAssetName_21.Invoke(this.ILInstance, value);

                }
            }

            public override System.String Name
            {
                get
                {
                    if (mget_Name_22.CheckShouldInvokeBase(this.ILInstance))
                        return base.Name;
                    else
                        return mget_Name_22.Invoke(this.ILInstance);

                }
                protected set
                {
                    if (mset_Name_23.CheckShouldInvokeBase(this.ILInstance))
                        base.Name = value;
                    else
                        mset_Name_23.Invoke(this.ILInstance, value);

                }
            }

            public override string ToString()
            {
                IMethod m = appdomain.ObjectType.GetMethod("ToString", 0);
                m = ILInstance.Type.GetVirtualMethod(m);
                if (m == null || m is ILMethod)
                {
                    return ILInstance.ToString();
                }
                else
                    return ILInstance.Type.FullName;
            }
        }
    }
}

