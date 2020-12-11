using ShipDock.Interfaces;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ShipDock.Applications
{
    public class PerspectiveInputer : IDispose
    {
        #region 回调函数
        /// <summary> The on drag. </summary>
        public UnityAction<InputData> onDrag = default;
        /// <summary> The on drag end. </summary>
        public UnityAction<InputData> onDragEnd = default;
        /// <summary> The on drag start. </summary>
        public UnityAction<InputData> onDragStart = default;
        /// <summary> The on click object up. </summary>
        public UnityAction<InputData> onClickObjUp = default;
        /// <summary> The on click object down. </summary>
        public UnityAction<InputData> onClickObjDown = default;
        /// <summary> The on click object stay. </summary>
        public UnityAction<GameObject> onClickObjStay = default;
        /// <summary> The on click object. </summary>
        public UnityAction<GameObject> onClickObj = default;

        /// <summary> The on enter object. </summary>
        public UnityAction<GameObject> onEnter = default;
        /// <summary> The on exit object. </summary>
        public UnityAction<GameObject> onExit = default;
        /// <summary> The on stay object. </summary>
        public UnityAction<GameObject> onStay = default;
        /// <summary> The on click none down. </summary>
        public UnityAction<InputData> onClickNoneDown = default;
        /// <summary> The on click none up. </summary>
        public UnityAction<InputData> onClickNoneUp = default;
        /// <summary> The on click none stay. </summary>
        public UnityAction<InputData> onClickNoneStay = default;
        #endregion

        private bool mIsEnter = false;
        private bool mIsNullStart;
        private bool mIsDrag, mIsDragStart;
        private float mMouseDis;
        private Ray mInputRay;
        private RaycastHit mInputHit;
        private Vector3 mMousePos;
        private Vector3 mMouseDownPos;
        private Vector3 mScreenSpace, mOffset;
        private Camera mCamera;
        private Transform mTargetTF;
        private Transform curDragObj;
        private GameObject mHitTarget;
        private GameObject mOldTarget;
        private InputData mInputData = default;

        private LayerMask LayerMask { get; set; }

        public bool IsNullStart
        {
            get { return mIsNullStart; }
            set { mIsNullStart = value; }
        }

        public bool HasInput
        {
            get
            {
#if UNITY_EDITOR
                return EventSystem.current.IsPointerOverGameObject();
#else
                return EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId);
#endif
            }
        }

        /// <summary>是否开启输入</summary>
        public bool IsOpenInput { get; set; }
        public float ClickRange { get; set; }

        public PerspectiveInputer()
        {
            mInputData = new InputData();
        }

        /// <summary>打开 Input</summary>
        public void ActvieInputer()
        {
            if (mCamera == null)
            {
                mCamera = Camera.main;
            }
            IsOpenInput = true;

            Input.multiTouchEnabled = false;
        }

        /// <summary>
        /// 关闭 Input
        /// </summary>
        public void DeactiveInput()
        {
            IsOpenInput = false;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="isOpen">是否开启</param>
        /// <param name="clickRange">点击范围</param>
        /// <param name="layerMask">屏蔽层</param>
        public void Init(bool isOpen, float clickRange, LayerMask layerMask)
        {
            Init(isOpen, clickRange);
            LayerMask = layerMask;
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="_isOpen">是否开启</param>
        /// <param name="_clickRange">点击范围</param>
        public void Init(bool _isOpen, float _clickRange)
        {
            Dispose();

            if (_isOpen)
            {
                ActvieInputer();
            }
            ClickRange = _clickRange;
        }

        public void CheckInteracting()
        {
            if (!IsOpenInput)
            {
                return;
            }

            if (mCamera == null)
            {
                mCamera = Camera.main;
            }
            if (mInputData == null)
            {
                mInputData = new InputData();
            }

            mMousePos = Input.mousePosition;
            mInputRay = mCamera.ScreenPointToRay(mMousePos);

            if (Physics.Raycast(mInputRay, out mInputHit, float.MaxValue, LayerMask))
            {
                mHitTarget = mInputHit.transform.gameObject;
                if (Input.GetMouseButtonDown(0))
                {
                    mIsDragStart = false;

                    mMouseDownPos = Input.mousePosition;
                    mTargetTF = mInputHit.transform;
                    mScreenSpace = mCamera.WorldToScreenPoint(mTargetTF.position);

                    Vector3 screenPos = new Vector3(mMousePos.x, mMousePos.y, mScreenSpace.z);
                    mOffset = mTargetTF.position - mCamera.ScreenToWorldPoint(screenPos);

                    mInputData.screenPosition = mMousePos;
                    mInputData.target = mTargetTF;
                    onClickObjDown?.Invoke(mInputData);
                }
                if (!mIsEnter)
                {
                    onEnter?.Invoke(mHitTarget);
                    mIsEnter = true;
                }
                onStay?.Invoke(mHitTarget);
                if ((mOldTarget != mHitTarget) && (mOldTarget != null))
                {
                    SetExit();
                }
                mOldTarget = mHitTarget;
            }
            else
            {
                SetExit();
                mHitTarget = null;

                mInputData.screenPosition = mMousePos;
                mInputData.target = null;
                if (Input.GetMouseButtonDown(0))
                {
                    mMouseDownPos = Input.mousePosition;
                    onClickNoneDown?.Invoke(mInputData);
                }
                if (Input.GetMouseButton(0))
                {
                    mMouseDis = Vector3.Distance(mMouseDownPos, mMousePos);
                    if (mIsDrag == false && mMouseDis < ClickRange)
                    {
                        onClickNoneStay?.Invoke(mInputData);
                    }
                    else
                    {
                        if (!mIsDragStart)
                        {
                            //Debug.Log("onDragStart 1");
                            onDragStart?.Invoke(mInputData);
                            mIsDragStart = true;
                        }
                        mIsDrag = true;
                        curDragObj = null;
                        onDrag?.Invoke(mInputData);
                    }
                }
                if (Input.GetMouseButtonUp(0))
                {
                    mMouseDis = Vector3.Distance(mMouseDownPos, mMousePos);
                    if (mMouseDis < ClickRange && !mIsDrag)
                    {
                        onClickNoneUp?.Invoke(mInputData);
                    }
                    else
                    {
                        //Debug.Log("onDragEnd 1" + curDragObj);
                        mInputData.target = curDragObj;
                        onDragEnd?.Invoke(mInputData);
                    }
                    mTargetTF = null;
                    mIsDrag = false;
                    curDragObj = null;
                }
            }

            if (mTargetTF == null && !mIsNullStart)
            {
                return;
            }

            if (Input.GetMouseButton(0))
            {
                if (Physics.Raycast(mInputRay, out mInputHit, float.MaxValue, LayerMask))
                {
                    onClickObj?.Invoke(mInputHit.transform.gameObject);
                }
                mMouseDis = Vector3.Distance(mMouseDownPos, mMousePos);
                if (mIsDrag == false && mMouseDis < ClickRange)
                {
                    onClickObjStay?.Invoke(mTargetTF.gameObject);
                }
                else
                {
                    if (!mIsDragStart)
                    {
                        //Debug.Log("onDragStart 2");
                        onDragStart?.Invoke(mInputData);
                        mIsDragStart = true;
                    }
                    mIsDrag = true;
                    Vector3 curScreenSpace = new Vector3(mMousePos.x, mMousePos.y, mScreenSpace.z);
                    Vector3 CurPosition = mCamera.ScreenToWorldPoint(curScreenSpace) + mOffset;

                    mInputData.target = mTargetTF;
                    mInputData.position = CurPosition;
                    mInputData.screenPosition = curScreenSpace;
                    curDragObj = mTargetTF;
                    onDrag?.Invoke(mInputData);
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                mMouseDis = Vector3.Distance(mMouseDownPos, mMousePos);
                if (mInputData != null)
                {
                    mInputData.target = mTargetTF;
                }
                if (mMouseDis < ClickRange && !mIsDrag)
                {
                    onClickObjUp?.Invoke(mInputData);
                }
                else
                {
                    //Debug.Log("onDragEnd 2" + curDragObj);
                    onDragEnd?.Invoke(mInputData);
                }
                mTargetTF = null;
                mIsDrag = false;
                curDragObj = null;
            }
        }

        private void SetExit()
        {
            if (mIsEnter)
            {
                onExit?.Invoke(mOldTarget);
                mIsEnter = false;
                mOldTarget = null;
            }
        }

        public void Dispose()
        {

            DeactiveInput();

            mCamera = default;
            mInputData = default;
            mHitTarget = default;
            mOldTarget = default;
            curDragObj = default;

            mMouseDis = 5;
            mIsNullStart = false;

            onDrag = null;
            onDragEnd = null;
            onDragStart = null;
            onClickObjUp = null;
            onClickObjDown = null;
            onClickObjStay = null;
            onEnter = null;
            onExit = null;
            onStay = null;
            onClickObj = null;
            onClickNoneDown = null;
            onClickNoneUp = null;
            onClickNoneStay = null;
        }
    }

    public class InputData
    {
        /// <summary>作用物体</summary>
        public Transform target = default;
        /// <summary>世界坐标的输入数据</summary>
        public Vector3 position = Vector3.zero;
        /// <summary>屏幕坐标的输入数据</summary>
        public Vector2 screenPosition = Vector2.zero;
    }
}