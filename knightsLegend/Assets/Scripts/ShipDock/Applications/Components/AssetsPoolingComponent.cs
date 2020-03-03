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

        private ComponentBridge mCompBridge;

        private void Awake()
        {
            name = "AssetsPool";
            transform.position = GameObjectReadyPos;

            mCompBridge = new ComponentBridge(OnInited);
            mCompBridge.Start();
        }

        private void OnInited()
        {
            mCompBridge.Dispose();
            ShipDockApp.Instance.AssetsPooling.SetAssetsPoolComp(this);
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
            if (target.transform.parent != transform)
            {
                target.transform.SetParent(transform);
            }
            target.transform.localPosition = Vector3.zero;
            target.SetActive(visible);
        }
    }

}