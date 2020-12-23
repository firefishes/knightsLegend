#define HOT_FIX_1

using UnityEngine;
using ShipDock.Applications;

namespace Game
{
    public class HotfixLogic : HotFixBase
    {
        private GameObject mOwner;
        private Transform mCube;

        public override void FixedUpdate()
        {
            Debug.Log("Fixed update");
        }

        public override void LateUpdate()
        {
            Debug.Log("Late Update");
        }

        public override void Update()
        {
            Debug.Log("Update");
            if (mCube == default)
            {
                return;
            }

#if HOT_FIX_1
            mCube.transform.localPosition += new Vector3(0f, 0.1f, 0f);
#else
            mCube.transform.localEulerAngles += new Vector3(0f, 1f, 2f);
#endif
        }

        public override void ShellInited(Object target)
        {
            Debug.Log(target.name);
            HotFixTest test = target as HotFixTest;
            mCube = test.transform;
        }

        private void OnUpdate(int time)
        {
            Debug.Log("自定义的其他游戏逻辑");
        }
    }
}
