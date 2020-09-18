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
            Pooling<ParamNotice<MonoBehaviour>>.To(notice);
        }

        public override void Enter()
        {
            base.Enter();

            ShowUI();
        }

        public override void Renew()
        {
            base.Renew();

            ShowUI();
        }

        private void ShowUI()
        {
            ShipDockApp.Instance.DataProxyLink(this, DataProxyLinks);

            if (mUI != default)
            {
                mUI.Add(UIModularHandler);
                mUI.transform.localScale = Vector3.one;
            }
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
                    mUI.transform.localScale = Vector3.zero;
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

        protected T UI
        {
            get
            {
                return mUI;
            }
        }

        protected IAssetBundles ABs { get; private set; }
        protected UIManager UIs { get; private set; }
        protected DataWarehouse Datas { get; private set; }

        /// <summary>UI模块的资源包名</summary>
        public virtual string ABName { get; }
        /// <summary>需要关联的数据代理</summary>
        public abstract int[] DataProxyLinks { get; }
    }
}