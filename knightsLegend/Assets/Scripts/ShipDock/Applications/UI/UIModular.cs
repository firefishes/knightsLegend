using System;
using ShipDock.Datas;
using ShipDock.Loader;
using ShipDock.Notices;
using ShipDock.Pooling;
using ShipDock.UI;
using UnityEngine;

namespace ShipDock.Applications
{
    public class UIModular<T> : UIStack, IUIModular where T : MonoBehaviour, INotificationSender
    {
        protected T mUI;

        public override void Init()
        {
            base.Init();

            Datas = ShipDockApp.Instance.Datas;
            ABs = ShipDockApp.Instance.ABs;
            UIs = ShipDockApp.Instance.UIs;

            GameObject prefab = ABs.Get(ABName, UIName);
            GameObject ui = UnityEngine.Object.Instantiate(prefab, UIs.UIRoot.MainCanvas.transform);

            ParamNotice<MonoBehaviour> notice = Pooling<ParamNotice<MonoBehaviour>>.From();
            int id = ui.GetInstanceID();
            id.Broadcast(notice);

            mUI = (T)notice.ParamValue;
            Pooling<ParamNotice<MonoBehaviour>>.To(notice);
        }

        public override void Enter()
        {
            base.Enter();

            if (mUI != default)
            {
                mUI.transform.localScale = Vector3.one;
                mUI.Add(UIChangeHandler);
            }
        }

        protected virtual void UIChangeHandler(INoticeBase<int> param)
        {

        }

        public override void Exit(bool isDestroy)
        {
            base.Exit(isDestroy);

            if (mUI != default)
            {
                mUI.Remove(UIChangeHandler);

                if (isDestroy)
                {
                    UnityEngine.Object.Destroy(mUI);
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

        protected virtual string ABName { get; }
        protected DataWarehouse Datas { get; private set; }
        protected IAssetBundles ABs { get; private set; }
        protected UIManager UIs { get; private set; }
    }

}