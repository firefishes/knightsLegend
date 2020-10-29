using ShipDock.Datas;
using ShipDock.Loader;
using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.UI;
using UnityEngine;

namespace ShipDock.Applications
{
    /// <summary>
    /// UI模块
    /// </summary>
    public abstract class UIModular<T> : UIStack, IUIModular, IDataExtracter where T : MonoBehaviour, INotificationSender
    {
        protected T mUI;

        protected T UI
        {
            get
            {
                return mUI;
            }
        }

        public override bool IsStackable
        {
            get
            {
                return UILayer == UILayerType.WINDOW;
            }
        }

        protected IAssetBundles ABs { get; private set; }
        protected UIManager UIs { get; private set; }
        protected DataWarehouse Datas { get; private set; }

        /// <summary>UI模块的资源包名</summary>
        public virtual string ABName { get; }
        /// <summary>需要关联的数据代理</summary>
        public abstract int[] DataProxyLinks { get; }
        /// <summary>UI层级</summary>
        public virtual int UILayer { get; protected set; }

        public override void Init()
        {
            base.Init();

            ShipDockApp app = ShipDockApp.Instance;
            Datas = app.Datas;
            ABs = app.ABs;
            UIs = app.UIs;

            GameObject prefab = ABs.Get(ABName, UIAssetName);
            GameObject ui = Object.Instantiate(prefab, UIs.UIRoot.MainCanvas.transform);

            ParamNotice<MonoBehaviour> notice = Pooling<ParamNotice<MonoBehaviour>>.From();

            int id = ui.GetInstanceID();
            id.Broadcast(notice);

            mUI = (T)notice.ParamValue;
            notice.ToPool();

            UILayer layer = ui.GetComponent<UILayer>();
            if (layer != default)
            {
                UILayer = layer.UILayerValue;
            }
            GetUIParent(out Transform parent);
            mUI.transform.SetParent(parent);
        }

        private void GetUIParent(out Transform parent)
        {
            parent = default;
            IUIRoot root = UIs.UIRoot;
            switch (UILayer)
            {
                case UILayerType.WINDOW:
                    parent = root.Windows;
                    break;
                case UILayerType.POPUPS:
                    parent = root.Popups;
                    break;
                case UILayerType.WIDGET:
                    parent = root.Widgets;
                    break;
            }
        }

        public override void Enter()
        {
            base.Enter();

            CheckDisplay();
        }

        public override void Renew()
        {
            base.Renew();

            CheckDisplay();
        }

        private void CheckDisplay()
        {
            ShipDockApp.Instance.DataProxyLink(this, DataProxyLinks);

            if (mUI != default)
            {
                ShowUI();
            }
        }

        /// <summary>
        /// 覆盖此方法，重载界面显示的逻辑
        /// </summary>
        protected virtual void ShowUI()
        {
            mUI.Add(UIModularHandler);
            if (UILayer == UILayerType.POPUPS)
            {
                mUI.transform.SetAsLastSibling();
            }
            mUI.transform.localScale = Vector3.one;
        }

        /// <summary>
        /// 覆盖此方法，重载界面隐藏的逻辑
        /// </summary>
        protected virtual void HideUI()
        {
            mUI.transform.localScale = Vector3.zero;
        }

        /// <summary>
        /// UI模块注册在UI资源中的消息器处理函数，用于模块与UI资源体的通信
        /// </summary>
        protected abstract void UIModularHandler(INoticeBase<int> param);

        /// <summary>
        /// UI模块注册在数据代理中的消息处理器函数，用于模块与数据的通信
        /// </summary>
        public abstract void OnDataProxyNotify(IDataProxy data, int keyName);

        public override void Exit(bool isDestroy)
        {
            base.Exit(isDestroy);

            ShipDockApp.Instance.DataProxyDelink(this, DataProxyLinks);

            if (mUI != default)
            {
                mUI.Remove(UIModularHandler);

                if (isDestroy)
                {
                    Object.Destroy(mUI);
                }
                else
                {
                    HideUI();
                }
            }

        }

        public virtual void Dispose()
        {
            UIs = default;
            ABs = default;
            mUI = default;
            Datas = default;
        }
    }
}