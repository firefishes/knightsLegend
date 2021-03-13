using ILRuntime.Runtime.Enviorment;
using ShipDock.Applications;
using ShipDock.Config;
using ShipDock.Datas;
using ShipDock.Interfaces;
using ShipDock.Modulars;
using ShipDock.Notices;
using ShipDock.Tools;
using System.Collections.Generic;
using UnityEngine;

using AutoAdaptorGeneratesDic = System.Collections.Generic.Dictionary<string, System.Type>;

public class AppHotFixConfigBase : IHotFixConfig
{
    public virtual string SpaceName { get; set; } = "ShipDock.Applications";
    public System.Action<AppDomain> RegisterMethods { get; set; }
    public CrossBindingAdaptor[] Adapters { get; set; }
    public AutoAdaptorGeneratesDic AutoAdapterGenerates { get; set; }

    public AppHotFixConfigBase()
    {
        RegisterMethods = SetRegisterMethods;
        AutoAdapterGenerates = GetAutoAdapterGenerates();
        Adapters = GetAdapters().ToArray();
    }

    protected virtual List<CrossBindingAdaptor> GetAdapters()
    {
        return new List<CrossBindingAdaptor>
        {
            new IDisposeAdapter(),
            new UIModularHotFixerAdapter(),
            new HotFixerInteractorAdapter(),
            new IConfigAdapter(),
            new IPoolableAdapter(),
            new ApplicationModularAdapter(),
            new DataProxyAdapter(),
            new NoticeAdapter(),
            new INotificationSenderAdapter(),
            new IEnumerableAdapter(),
            new IEnumeratorAdapter(),
            new IEnumerable_1_ObjectAdapter(),
            new IEnumerable_1_StringAdapter(),
            new ValueItemAdapter(),
            new IDataExtracterAdapter(),
        };
    }

    protected virtual AutoAdaptorGeneratesDic GetAutoAdapterGenerates()
    {
        return new AutoAdaptorGeneratesDic
        {
            ["IDisposeAdapter"] = typeof(IDispose),
            ["UIModularHotFixerAdapter"] = typeof(UIModularHotFixer),
            ["HotFixerInteractorAdapter"] = typeof(HotFixerInteractor),
            ["IConfigAdapter"] = typeof(IConfig),
            ["ApplicationModularAdapter"] = typeof(ApplicationModular),
            ["ParamNoticeStringAdapter"] = typeof(ParamNotice<string>),
            ["ParamStringsNoticeAdapter"] = typeof(ParamNotice<string[]>),
            ["DataProxyAdapter"] = typeof(DataProxy),
            ["NoticeAdapter"] = typeof(Notice),
            ["INotificationSenderAdapter"] = typeof(INotificationSender),
            ["IEnumerableAdapter"] = typeof(System.Collections.IEnumerable),
            ["IEnumeratorAdapter"] = typeof(System.Collections.IEnumerator),
            ["IEnumerable_1_ObjectAdapter"] = typeof(IEnumerable<object>),
            ["IEnumerable_1_StringAdapter"] = typeof(IEnumerable<string>),
            ["ValueItemAdapter"] = typeof(ValueItem),
            ["IDataExtracterAdapter"] = typeof(IDataExtracter),
        };
    }

    protected virtual void SetRegisterMethods(AppDomain appdomain)
    {
        appdomain.DelegateManager.RegisterMethodDelegate<int>();
        appdomain.DelegateManager.RegisterMethodDelegate<INoticeBase<int>>();
        appdomain.DelegateManager.RegisterMethodDelegate<INotice>();        appdomain.DelegateManager.RegisterMethodDelegate<IParamNotice<System.Int32>>();
        appdomain.DelegateManager.RegisterMethodDelegate<InputData>();
        appdomain.DelegateManager.RegisterMethodDelegate<GameObject>();        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Vector3>();        appdomain.DelegateManager.RegisterMethodDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance>();        appdomain.DelegateManager.RegisterMethodDelegate<ShipDock.Tools.TimeGapper>();
        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.EventSystems.BaseEventData>();        appdomain.DelegateManager.RegisterMethodDelegate<System.Boolean, ShipDock.Loader.AssetsLoader>();        appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, ShipDock.Notices.INoticeBase<System.Int32>>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Boolean, System.Single>();        appdomain.DelegateManager.RegisterMethodDelegate<ShipDock.UI.IUIStack, System.Boolean>();        appdomain.DelegateManager.RegisterMethodDelegate<ShipDock.Datas.IDataProxy, System.Int32>();        appdomain.DelegateManager.RegisterMethodDelegate<System.Boolean, UnityEngine.EventSystems.EventTriggerType>();        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode>();        appdomain.DelegateManager.RegisterFunctionDelegate<System.Object>();        appdomain.DelegateManager.RegisterFunctionDelegate<INoticeBase<int>>();        appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Vector3>();        appdomain.DelegateManager.RegisterFunctionDelegate<ShipDock.Config.IConfigHolder>();        appdomain.DelegateManager.RegisterFunctionDelegate<ILRuntime.Runtime.Intepreter.ILTypeInstance>();
        appdomain.DelegateManager.RegisterFunctionDelegate<ShipDock.Applications.HotFixerInteractor>();        appdomain.DelegateManager.RegisterFunctionDelegate<System.Boolean>();        appdomain.DelegateManager.RegisterFunctionDelegate<System.Single>();        appdomain.DelegateManager.RegisterFunctionDelegate<System.Int32, ShipDock.Notices.INoticeBase<System.Int32>>();
        appdomain.DelegateManager.RegisterFunctionDelegate<ShipDock.Applications.IDisposeAdapter.Adapter, ShipDock.Applications.IDisposeAdapter.Adapter, System.Int32>();        //appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOGetter<System.Single>>((act) =>
        //{
        //    return new DG.Tweening.Core.DOGetter<System.Single>(() =>
        //    {
        //        return ((System.Func<System.Single>)act)();
        //    });
        //});        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode>>((act) =>
        {
            return new UnityEngine.Events.UnityAction<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode>((arg0, arg1) =>
            {
                ((System.Action<UnityEngine.SceneManagement.Scene, UnityEngine.SceneManagement.LoadSceneMode>)act)(arg0, arg1);
            });
        });        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.Boolean, UnityEngine.EventSystems.EventTriggerType>>((act) =>
        {
            return new UnityEngine.Events.UnityAction<System.Boolean, UnityEngine.EventSystems.EventTriggerType>((arg0, arg1) =>
            {
                ((System.Action<System.Boolean, UnityEngine.EventSystems.EventTriggerType>)act)(arg0, arg1);
            });
        });        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<UnityEngine.EventSystems.BaseEventData>>((act) =>
        {
            return new UnityEngine.Events.UnityAction<UnityEngine.EventSystems.BaseEventData>((arg0) =>
            {
                ((System.Action<UnityEngine.EventSystems.BaseEventData>)act)(arg0);
            });
        });        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction>((act) =>
        {
            return new UnityEngine.Events.UnityAction(() =>
            {
                ((System.Action)act)();
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<InputData>>((act) =>
        {
            return new UnityEngine.Events.UnityAction<InputData>((arg0) =>
            {
                ((System.Action<InputData>)act)(arg0);
            });
        });        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<GameObject>>((act) =>
        {
            return new UnityEngine.Events.UnityAction<GameObject>((arg0) =>
            {
                ((System.Action<GameObject>)act)(arg0);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.TweenCallback>((act) =>
        {
            return new DG.Tweening.TweenCallback(() =>
            {
                ((System.Action)act)();
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<UnityEngine.Events.UnityAction<System.Boolean, ShipDock.Loader.AssetsLoader>>((act) =>
        {
            return new UnityEngine.Events.UnityAction<System.Boolean, ShipDock.Loader.AssetsLoader>((arg0, arg1) =>
            {
                ((System.Action<System.Boolean, ShipDock.Loader.AssetsLoader>)act)(arg0, arg1);
            });
        });
        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOSetter<UnityEngine.Vector3>>((act) =>
        {
            return new DG.Tweening.Core.DOSetter<UnityEngine.Vector3>((pNewValue) =>
            {
                ((System.Action<UnityEngine.Vector3>)act)(pNewValue);
            });
        });        appdomain.DelegateManager.RegisterDelegateConvertor<DG.Tweening.Core.DOGetter<UnityEngine.Vector3>>((act) =>
        {
            return new DG.Tweening.Core.DOGetter<UnityEngine.Vector3>(() =>
            {
                return ((System.Func<UnityEngine.Vector3>)act)();
            });
        });        appdomain.DelegateManager.RegisterDelegateConvertor<System.Comparison<ShipDock.Applications.IDisposeAdapter.Adapter>>((act) =>
        {
            return new System.Comparison<ShipDock.Applications.IDisposeAdapter.Adapter>((x, y) =>
            {
                return ((System.Func<ShipDock.Applications.IDisposeAdapter.Adapter, ShipDock.Applications.IDisposeAdapter.Adapter, System.Int32>)act)(x, y);
            });
        });
    }
}
