using ILRuntime.Runtime.Enviorment;
using ShipDock.Applications;
using ShipDock.Interfaces;
using ShipDock.Modulars;
using ShipDock.Notices;
using System.Collections.Generic;
using UnityEngine;

using AutoAdaptorGeneratesDic = System.Collections.Generic.Dictionary<string, System.Type>;

public class AppHotFixConfigBase : IHotFixConfig
{
    public virtual string SpaceName { get; set; } = string.Empty;
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
        };
    }

    protected virtual void SetRegisterMethods(AppDomain appdomain)
    {
        appdomain.DelegateManager.RegisterMethodDelegate<int>();
        appdomain.DelegateManager.RegisterMethodDelegate<INoticeBase<int>>();
        appdomain.DelegateManager.RegisterMethodDelegate<INotice>();        appdomain.DelegateManager.RegisterMethodDelegate<IParamNotice<System.Int32>>();
        appdomain.DelegateManager.RegisterMethodDelegate<System.Boolean, ShipDock.Loader.AssetsLoader>();        appdomain.DelegateManager.RegisterMethodDelegate<System.Int32, ShipDock.Notices.INoticeBase<System.Int32>>();
        appdomain.DelegateManager.RegisterMethodDelegate<InputData>();
        appdomain.DelegateManager.RegisterMethodDelegate<GameObject>();        appdomain.DelegateManager.RegisterMethodDelegate<UnityEngine.Vector3>();        appdomain.DelegateManager.RegisterFunctionDelegate<System.Object>();        appdomain.DelegateManager.RegisterFunctionDelegate<INoticeBase<int>>();        appdomain.DelegateManager.RegisterFunctionDelegate<UnityEngine.Vector3>();        appdomain.DelegateManager.RegisterFunctionDelegate<System.Int32, ShipDock.Notices.INoticeBase<System.Int32>>();

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
        });
    }
}
