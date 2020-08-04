using System;
using ShipDock.Notices;
using UnityEngine;

namespace ShipDock.Applications
{
    /// <summary>
    /// 
    /// 游戏对象池组件（单例组件）
    /// 
    /// add by Minghua.ji
    /// 
    /// </summary>
    public class AssetsPoolingComponent : MonoBehaviour
    {

        public static Vector3 GameObjectReadyPos = new Vector3(100000, 100000, 100000);

        private bool mIsDestroyed;
        private ComponentBridge mCompBridge;

        private void Awake()
        {
            name = "AssetsPool";
            mIsDestroyed = false;
            transform.position = GameObjectReadyPos;

            mCompBridge = new ComponentBridge(OnInited);
            mCompBridge.Start();
        }

        private void OnDestroy()
        {
            mIsDestroyed = true;
        }

        private void OnInited()
        {
            mCompBridge.Dispose();

            ShipDockConsts.NOTICE_APPLICATION_CLOSE.Add(OnAppClose);

            ShipDockApp.Instance.AssetsPooling.SetAssetsPoolComp(this);
        }

        private void OnAppClose(INoticeBase<int> obj)
        {
            if (mIsDestroyed)
            {
                return;
            }
            GameObject item;
            int count = transform.childCount;
            for (int i = 0; i < count; i++)
            {
                item = transform.GetChild(i).gameObject;
                Destroy(item);
            }
        }

        public void Get(GameObject target)
        {
            if (target.transform.parent == transform)
            {
                target.transform.SetParent(null);
            }
        }

        public void Collect(GameObject target, bool visible = false)
        {
            target.transform.position = GameObjectReadyPos;
#if UNITY_EDITOR
            if (target.transform.parent != transform)
            {
                target.transform.SetParent(transform);
            }
#endif
        }
    }

}