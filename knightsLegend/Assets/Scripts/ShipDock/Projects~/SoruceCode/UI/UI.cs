using ShipDock.Applications;
using ShipDock.Loader;
using ShipDock.Notices;
using ShipDock.Pooling;
using UnityEngine;

namespace ShipDock.UI
{
    /// <summary>
    /// UI虚类
    /// </summary>
    public abstract class UI : MonoBehaviour, INotificationSender
    {
        /// <summary>游戏物体id</summary>
        private int mInstanceID;
        /// <summary>资源对象池引用</summary>
        private AssetsPooling mUIPooling;

        /// <summary>标识UI是否有修改</summary>
        public bool UIChanged { get; set; }
        /// <summary>销毁</summary>
        public bool Destroyed { get; private set; }
        /// <summary>ab资源管理器</summary>
        protected IAssetBundles ABs { get; private set; }
        /// <summary>UI管理器</summary>
        protected UIManager UIs { get; private set; }

        protected void OnDestroy()
        {
            if (Destroyed)
            {
                return;
            }
            else { }

            Destroyed = true;
            UIChanged = false;

            mInstanceID.Remove(OnUIReady);

            Purge();

            mUIPooling = default;
            ABs = default;
            UIs = default;
        }

        protected virtual void Awake()
        {
            Destroyed = false;
            ABs = ShipDockApp.Instance.ABs;
            UIs = ShipDockApp.Instance.UIs;

            mInstanceID = gameObject.GetInstanceID();
            mInstanceID.Add(OnUIReady);//以游戏物体id为消息名注册处理器
        }

        protected virtual void Start() { }

        protected virtual void Update()
        {
            if (UIChanged)
            {
                UIChanged = false;
                UpdateUI();
            }
            else { }
        }

        protected abstract void Purge();

        /// <summary>
        /// UI 就绪的消息处理函数
        /// </summary>
        /// <param name="param"></param>
        private void OnUIReady(INoticeBase<int> param)
        {
            mInstanceID.Remove(OnUIReady);

            IParamNotice<MonoBehaviour> notice = param as IParamNotice<MonoBehaviour>;
            if (notice != default)
            {
                notice.ParamValue = GetUIReadyParam();
            }
            else { }
        }

        protected virtual MonoBehaviour GetUIReadyParam()
        {
            return this;
        }

        protected AssetsPooling UIPooling
        {
            get
            {
                if (!Destroyed && (mUIPooling == default))
                {
                    mUIPooling = ShipDockApp.Instance.AssetsPooling;
                }
                else { }

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
            else { }
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
            else { }
        }

        public void RemoveChild(Transform tf)
        {
            int a = tf.GetInstanceID();
            int b = transform.GetInstanceID();

            if (a == b)
            {
                tf.SetParent(default);
            }
            else { }
        }

        public void SetParent(Transform tf)
        {
            int a = tf.GetInstanceID();
            int b = transform.GetInstanceID();

            if (a != b)
            {
                transform.SetParent(tf);
            }
            else { }
        }

        /// <summary>如果Ui被标识为已修改，则调用此更新函数</summary>
        public abstract void UpdateUI();
    }
}

