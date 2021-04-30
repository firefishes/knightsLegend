
#if ODIN_INSPECTOR
using Sirenix.OdinInspector;
#endif
using System;
using UnityEngine;

namespace ShipDock.Applications
{
    [Serializable]
    public class HotFixerLoadedNoticeInfo
    {
        [Header("热更对象就绪消息的配置")]
        [SerializeField, Tooltip("是否启用发送就绪消息（以 GameObject或Script id为消息名）至热更端的功能")]
#if ODIN_INSPECTOR
        [LabelText("启用")]
#endif
        private bool m_IsSendIDAsNotice;

        [SerializeField, Tooltip("是否在物体被渲染时发送对象就绪的消息"), Indent(1)]
#if ODIN_INSPECTOR
        [LabelText("在物体被渲染时发送消息"), ShowIf("m_IsSendIDAsNotice", true)]
#endif
        private bool m_IsSendInRenderObject;

        [SerializeField, Tooltip("在物体被渲染时发送对象就绪的消息是否仅发送一次")]
#if ODIN_INSPECTOR
        [LabelText("仅发送一次"), ShowIf("@this.m_IsSendIDAsNotice == true && this.m_IsSendInRenderObject == true"), Indent(2)]
#endif
        private bool m_IsSendOnceInRenderObject = true;

        [SerializeField, Tooltip("是否以 GameObject为消息名，不勾选则使用本组件的唯一 ID 标识作为消息名")]
#if ODIN_INSPECTOR
        [LabelText("以物体唯一 ID 标识作为消息名"), ShowIf("m_IsSendIDAsNotice", true), Indent(1)]
#endif
        private bool m_ApplyGameObjectID = true;

        [SerializeField, Tooltip("是否使用默认的消息类型发送")]
#if ODIN_INSPECTOR
        [LabelText("使用默认消息类（Notice）传递消息"), ShowIf("m_IsSendIDAsNotice", true), Indent(1)]
#endif
        private bool m_ApplyDefaultNoticeType = true;

        [SerializeField, Tooltip("能从热更端对象获取自定义的消息类型的函数名"), Indent(1)]
#if ODIN_INSPECTOR
        [LabelText("获取自定义消息类对象的方法名"), ShowIf("@this.m_IsSendIDAsNotice && !this.m_ApplyDefaultNoticeType"), OnValueChanged("GetIDAsCustomNoticeMethodChanged"), Indent(2)]
#endif
        private string m_GetIDAsCustomNoticeMethod;
#if ODIN_INSPECTOR
        private void GetIDAsCustomNoticeMethodChanged()
        {
            m_ApplyDefaultNoticeType = string.IsNullOrEmpty(m_GetIDAsCustomNoticeMethod);
        }
#endif

        [SerializeField]
#if ODIN_INSPECTOR
        [LabelText("推迟到下一帧发送消息"), ShowIf("m_IsSendIDAsNotice", true), Indent(1)]
#endif
        private bool m_ApplyCallLate;

        [SerializeField, Tooltip("是否已发送过对象就绪的消息")]
#if ODIN_INSPECTOR
        [LabelText("消息是否已发送"), ShowIf("m_IsSendIDAsNotice", true), ReadOnly()]
#endif
        private bool m_IsReadyNoticeSend;

        public void SetReadyNoticeSend(bool value)
        {
            m_IsReadyNoticeSend = value;
        }

        /// <summary>
        /// 禁用热更端对象的就绪消息发送
        /// </summary>
        public void DisableReadyNotice()
        {
            m_IsSendIDAsNotice = false;
        }

        public void EnableReadyNotice()
        {
            m_IsSendIDAsNotice = true;
        }

        public bool IsSendIDAsNotice
        {
            get
            {
                return m_IsSendIDAsNotice;
            }
        }

        public bool ApplyDefaultNoticeType
        {
            get
            {
                return m_ApplyDefaultNoticeType;
            }
        }

        public string GetIDAsCustomNoticeMethod
        {
            get
            {
                return m_GetIDAsCustomNoticeMethod;
            }
        }

        public bool ApplyCallLate
        {
            get
            {
                return m_ApplyCallLate;
            }
        }

        public bool ApplyGameObjectID
        {
            get
            {
                return m_ApplyGameObjectID;
            }
        }

        public bool IsSendInRenderObject
        {
            get
            {
                return m_IsSendInRenderObject;
            }
        }

        public bool IsSendOnceInRenderObject
        {
            get
            {
                return m_IsSendOnceInRenderObject;
            }
        }

        public bool IsReadyNoticeSend
        {
            get
            {
                return m_IsReadyNoticeSend;
            }
        }
    }
}