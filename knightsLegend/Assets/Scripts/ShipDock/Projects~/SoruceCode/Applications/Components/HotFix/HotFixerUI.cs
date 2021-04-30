using ShipDock.Loader;
using ShipDock.Pooling;
using ShipDock.UI;
using UnityEngine;

namespace ShipDock.Applications
{
    /// <summary>
    /// 
    /// UI热更中间类
    /// 
    /// 为了同时获得框架的UI类功能、又兼顾ILRuntime中尽量不继承 MonoBehaviour 的原则而设计
    /// 
    /// </summary>
    public class HotFixerUI : UI.UI
    {
        public HotFixerInteractor HotFixerInteractor { get; private set; }
        public HotFixerUIAgent Agent { get; private set; }

        public AssetsPooling GetUIPooling()
        {
            return UIPooling;
        }

        public UIManager UIManager()
        {
            return UIs;
        }

        public IAssetBundles AssetBundles()
        {
            return ABs;
        }

        public override void UpdateUI()
        {
            HotFixerInteractor?.UpdateInteractor();
        }

        protected override void Purge()
        {
            HotFixerInteractor?.Release();
            HotFixerInteractor = default;

            Agent = default;
        }

        public void SetHotFixInteractor(HotFixerInteractor target)
        {
            HotFixerInteractor = target;
            ILRuntimeUtils.InvokeMethodILR(target, Agent.UIInteractorName, "InitInteractor", 2, this, Agent);
        }

        public void SetHotFixerAgent(HotFixerUIAgent agent)
        {
            Agent = agent;
        }

        protected override MonoBehaviour GetUIReadyParam()
        {
            return Agent;
        }
    }
}