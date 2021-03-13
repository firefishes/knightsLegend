using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ShipDock.Applications
{
    public class PerspectiveInputer
    {
        #region 回调函数
        /// <summary> The on drag. </summary>
        public UnityAction<InputData> onDrag = default;
        /// <summary> The on drag end. </summary>
        public UnityAction<InputData> onDragEnd = default;
        /// <summary> The on drag start. </summary>
        public UnityAction<InputData> onDragStart = default;
        /// <summary> The on click object up. </summary>
        public UnityAction<InputData> onClickUp = default;
        /// <summary> The on click object down. </summary>
        public UnityAction<InputData> onClickDown = default;
        /// <summary> The on click object stay. </summary>
        public UnityAction<GameObject> onClickStay = default;
        /// <summary> The on click object. </summary>
        public UnityAction<GameObject> onClick = default;

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
        private bool mIsDrag, mIsDragStart;
        private float mMouseDis;
        private Ray mInputRay;
        private RaycastHit mInputHit;
        private Vector3 mMousePos;
        private Vector3 mMouseDownPos;
        private Vector3 mScreenSpace, mOffset;
        private Camera mCamera;
        private Transform mTargetTrans;
        private Transform mCurDraging;
        private GameObject mHitTarget;
        private GameObject mPrevTarget;
        private InputData mInputData = default;


        public bool IsNullStart { get; set; }
        /// <summary>是否开启输入</summary>
        public bool IsOpenInput { get; set; }
        public float ClickRange { get; set; }

        private LayerMask LayerMask { get; set; }

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

            if (mCamera == default)
            {
                mCamera = Camera.main;
            }
            if (mInputData == default)
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
                    mTargetTrans = mInputHit.transform;
                    mScreenSpace = mCamera.WorldToScreenPoint(mTargetTrans.position);

                    Vector3 screenPos = new Vector3(mMousePos.x, mMousePos.y, mScreenSpace.z);
                    mOffset = mTargetTrans.position - mCamera.ScreenToWorldPoint(screenPos);

                    mInputData.target = mTargetTrans;
                    mInputData.screenPosition = mMousePos;
                    onClickDown?.Invoke(mInputData);
                }
                if (!mIsEnter)
                {
                    mIsEnter = true;
                    onEnter?.Invoke(mHitTarget);
                }
                onStay?.Invoke(mHitTarget);
                if ((mPrevTarget != default) && (mPrevTarget != mHitTarget))
                {
                    InteractExit();
                }
                mPrevTarget = mHitTarget;
            }
            else
            {
                InteractExit();
                mHitTarget = default;

                mInputData.screenPosition = mMousePos;
                mInputData.target = default;
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
                            onDragStart?.Invoke(mInputData);
                            mIsDragStart = true;
                        }
                        mIsDrag = true;
                        mCurDraging = default;
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
                        mInputData.target = mCurDraging;
                        onDragEnd?.Invoke(mInputData);
                    }
                    mIsDrag = false;
                    mTargetTrans = default;
                    mCurDraging = default;
                }
            }

            if (mTargetTrans == default && !IsNullStart)
            {
                return;
            }

            if (Input.GetMouseButton(0))
            {
                if (Physics.Raycast(mInputRay, out mInputHit, float.MaxValue, LayerMask))
                {
                    onClick?.Invoke(mInputHit.transform.gameObject);
                }
                mMouseDis = Vector3.Distance(mMouseDownPos, mMousePos);
                if (mIsDrag == false && mMouseDis < ClickRange)
                {
                    onClickStay?.Invoke(mTargetTrans.gameObject);
                }
                else
                {
                    if (!mIsDragStart)
                    {
                        onDragStart?.Invoke(mInputData);
                        mIsDragStart = true;
                    }
                    mIsDrag = true;
                    Vector3 curScreenSpace = new Vector3(mMousePos.x, mMousePos.y, mScreenSpace.z);
                    Vector3 curPosition = mCamera.ScreenToWorldPoint(curScreenSpace) + mOffset;

                    mInputData.target = mTargetTrans;
                    mInputData.position = curPosition;
                    mInputData.screenPosition = curScreenSpace;
                    mCurDraging = mTargetTrans;
                    onDrag?.Invoke(mInputData);
                }
            }
            if (Input.GetMouseButtonUp(0))
            {
                mMouseDis = Vector3.Distance(mMouseDownPos, mMousePos);
                if (mInputData != default)
                {
                    mInputData.target = mTargetTrans;
                }
                if (mMouseDis < ClickRange && !mIsDrag)
                {
                    onClickUp?.Invoke(mInputData);
                }
                else
                {
                    onDragEnd?.Invoke(mInputData);
                }
                mIsDrag = false;
                mTargetTrans = default;
                mCurDraging = default;
            }
        }

        private void InteractExit()
        {
            if (mIsEnter)
            {
                mIsEnter = false;
                onExit?.Invoke(mPrevTarget);
                mPrevTarget = default;
                mInputHit = default;
            }
        }

        public void Dispose()
        {

            DeactiveInput();

            mCamera = default;
            mInputData = default;
            mHitTarget = default;
            mPrevTarget = default;
            mCurDraging = default;

            mMouseDis = 5;
            IsNullStart = false;

            onDrag = default;
            onDragEnd = default;
            onDragStart = default;
            onClickUp = default;
            onClickDown = default;
            onClickStay = default;
            onEnter = default;
            onExit = default;
            onStay = default;
            onClick = default;
            onClickNoneDown = default;
            onClickNoneUp = default;
            onClickNoneStay = default;
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