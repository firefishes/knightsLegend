using ShipDock.Applications;
using ShipDock.Loader;
using ShipDock.Notices;
using ShipDock.Pooling;
using UnityEngine;

namespace ShipDock.UI
{
    /// <summary>UI虚类</summary>
    public abstract class UI : MonoBehaviour, INotificationSender
    {
        private AssetsPooling mUIPooling;

        protected virtual void Awake()
        {
            Destroyed = false;
            ABs = ShipDockApp.Instance.ABs;
            UIs = ShipDockApp.Instance.UIs;

            int id = gameObject.GetInstanceID();
            id.Add(OnUIReady);
        }

        protected virtual void Start() { }

        protected virtual void Update()
        {
            if(UIChanged)
            {
                UIChanged = false;
                UpdateUI();
            }
        }

        protected void OnDestroy()
        {
            Purge();
        }

        protected virtual void Purge()
        {
            Destroyed = true;

            int id = gameObject.GetInstanceID();
            id.Remove(OnUIReady);

            mUIPooling = default;
            ABs = default;
            UIs = default;
        }

        private void OnUIReady(INoticeBase<int> param)
        {
            int id = gameObject.GetInstanceID();
            id.Remove(OnUIReady);

            IParamNotice<MonoBehaviour> notice = param as IParamNotice<MonoBehaviour>;
            notice.ParamValue = this;
        }

        protected AssetsPooling UIPooling
        {
            get
            {
                if(!Destroyed && (mUIPooling == default))
                {
                    mUIPooling = ShipDockApp.Instance.AssetsPooling;
                }
                return mUIPooling;
            }
        }

        public void AddChild(Transform tf)
        {
            int a = tf.GetInstanceID();
            int b = transform.GetInstanceID();
            if (a != b)
            {
                tf.SetParent(transform);
            }
        }

        public void AddChildAt(Transform tf, int index)
        {
            int a = tf.GetInstanceID();
            int b = transform.GetInstanceID();
            if (a != b)
            {
                tf.SetParent(transform);
                tf.SetSiblingIndex(index);
            }
        }

        public void RemoveChild(Transform tf)
        {
            int a = tf.GetInstanceID();
            int b = transform.GetInstanceID();
            if(a == b)
            {
                tf.SetParent(default);
            }
        }

        public void SetParent(Transform tf)
        {
            int a = tf.GetInstanceID();
            int b = transform.GetInstanceID();
            if (a != b)
            {
                transform.SetParent(tf);
            }
        }

        public abstract void UpdateUI();

        public bool UIChanged { get; set; }
        public bool Destroyed { get; private set; }
        protected IAssetBundles ABs { get; private set; }
        protected UIManager UIs { get; private set; }
    }
}

