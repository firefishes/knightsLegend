using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace ShipDock.UI
{
    [Serializable]
    public class UIPointerAreaInvkedEvent : UnityEvent<bool, EventTriggerType> { }
    [Serializable]
    public class UIPointerAreaEvent : UnityEvent<PointerEventData> { }

    public class UIPointerArea : MonoBehaviour, IUIPointerArea
    {
        [SerializeField]
        private bool m_ApplyAreaCheck = true;
        [SerializeField]
        private bool m_IsPointerClick = true;
        [SerializeField]
        private bool m_IsPointerDown;
        [SerializeField]
        private bool m_IsPointerUp;
        [SerializeField]
        private bool m_IsPointerEnter;
        [SerializeField]
        private bool m_IsPointerExit;
        [SerializeField]
        private UIPointerAreaEvent m_PointerAreaEvent = new UIPointerAreaEvent();
        [SerializeField]
        private UIPointerAreaInvkedEvent m_PointerAreaInvokedEvent = new UIPointerAreaInvkedEvent();

        private Touch mTouch;
        private Touch[] mTouches;
        private bool mIsInArea;
        private EventTriggerType mTriggerType;
        private Camera mUICamera;
        private Vector2 mPointerPosition;
        private RectTransform mRectTransform;

        private void Awake()
        {
            mIsInArea = true;
            mRectTransform = transform as RectTransform;
            UIManager UIs = Framework.Instance.GetUnit<UIManager>(Framework.UNIT_UI);
            mUICamera = UIs.UIRoot.UICamera;
        }

        private void OnDestroy()
        {
            m_PointerAreaEvent.RemoveAllListeners();
            m_PointerAreaInvokedEvent.RemoveAllListeners();

            mUICamera = default;
            m_PointerAreaEvent = default;
            m_PointerAreaInvokedEvent = default;
        }

        private void Update()
        {
            if (m_ApplyAreaCheck && (mRectTransform != default) && (mUICamera != default))
            {
//#if UNITY_EDITOR
//                if (Input.touchCount > 0)
//                {
//                    mTouches = Input.touches;
//                    mTouch = mTouches[0];
//                    mPointerPosition = mTouch.position;
//                    mIsInArea = IsPositionSelf(mPointerPosition);

//                    switch (mTouch.phase)
//                    {
//                        case TouchPhase.Began:
//                            mTriggerType = EventTriggerType.PointerDown;
//                            m_PointerAreaInvokedEvent.Invoke(mIsInArea, mTriggerType);
//                            break;
//                        case TouchPhase.Moved:
//                            mTriggerType = EventTriggerType.Move;
//                            break;
//                        case TouchPhase.Stationary:
//                            if (mTriggerType == EventTriggerType.PointerDown)
//                            {
//                                mTriggerType = EventTriggerType.
//                            }
//                            break;
//                        case TouchPhase.Ended:
//                            if (mTriggerType == EventTriggerType.PointerDown)
//                            {
//                                mTriggerType = EventTriggerType.PointerUp;
//                            }
//                            mTriggerType = EventTriggerType.PointerUp;
//                            break;
//                    }
//                }
//#endif
            }
        }

        private bool IsPositionSelf(Vector2 pos)
        {
            return RectTransformUtility.RectangleContainsScreenPoint(mRectTransform, pos, mUICamera);
        }

        private void CheckPointer(bool flag, ref PointerEventData eventData, EventTriggerType triggerType)
        {
            mPointerPosition = eventData.position;
            if (flag)
            {
                mTriggerType = EventTriggerType.PointerClick;
                m_PointerAreaEvent.Invoke(eventData);

                if (m_ApplyAreaCheck)
                {
                    mIsInArea = IsPositionSelf(mPointerPosition);
                    m_PointerAreaInvokedEvent.Invoke(mIsInArea, mTriggerType);
                }
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            CheckPointer(m_IsPointerClick, ref eventData, EventTriggerType.PointerClick);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            CheckPointer(m_IsPointerDown, ref eventData, EventTriggerType.PointerDown);
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            CheckPointer(m_IsPointerEnter, ref eventData, EventTriggerType.PointerEnter);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            CheckPointer(m_IsPointerExit, ref eventData, EventTriggerType.PointerExit);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            CheckPointer(m_IsPointerUp, ref eventData, EventTriggerType.PointerUp);
        }

        public UIPointerAreaEvent PointerAreaEvent
        {
            get
            {
                return m_PointerAreaEvent;
            }
        }

        public UIPointerAreaInvkedEvent PointerAreaInvokedEvent
        {
            get
            {
                return m_PointerAreaInvokedEvent;
            }
        }
    }

    public interface IUIPointerArea :
        IPointerClickHandler, IPointerDownHandler, IPointerEnterHandler, IPointerExitHandler, IPointerUpHandler
    {
        UIPointerAreaEvent PointerAreaEvent { get; }
        UIPointerAreaInvkedEvent PointerAreaInvokedEvent { get; }
    }
}
