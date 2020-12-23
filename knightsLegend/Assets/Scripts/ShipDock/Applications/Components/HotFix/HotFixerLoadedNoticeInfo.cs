using System;
using UnityEngine;

namespace ShipDock.Applications
{
    [Serializable]
    public class HotFixerLoadedNoticeInfo
    {
        [Header("热更端对象就绪消息的配置")]
        [SerializeField]
        [Tooltip("是否启用发送就绪消息（以 GameObject或Script id为消息名）至热更端的功能")]
        private bool m_IsSendIDAsNotice;
        [SerializeField]
        [Tooltip("是否以 GameObject为消息名")]
        private bool m_ApplyGameObjectID = true;
        [SerializeField]
        [Tooltip("是否使用默认的消息类型发送")]
        private bool m_ApplyDefaultNoticeType = true;
        [SerializeField]
        [Tooltip("是否推迟到下一帧发送消息")]
        private bool m_ApplyCallLate;
        [SerializeField]
        [Tooltip("能从热更端对象获取自定义的消息类型的函数名")]
        private string m_GetIDAsCustomNoticeMethod;

        /// <summary>
        /// 禁用热更端对象的就绪消息发送
        /// </summary>
        public void DisableReadyNotice()
        {
            m_IsSendIDAsNotice = false;
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

    }
}