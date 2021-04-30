using ShipDock.Notices;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace ShipDock.SDK
{
    /// <summary>
    /// 
    /// SDK消息中心
    /// 
    /// Author: Tian
    /// Modifie：Minghua.ji
    /// 
    /// </summary>
    public class SDKMessages : MonoBehaviour
    {
        public const int N_GET_SDK_MESSAGES_REF = -9000;

        public static int SDKChannel { get; set; }

        private class MessageBody
        {
            public string Tag;
            public bool RemovedAfterInvoked;
            public UnityAction<string> MethodBody;
        }

        private List<MessageBody> mMessageList;

        public string TargetName
        {
            get { return gameObject.name; }
        }

        public string MethodName
        {
            get { return nameof(MessageCallback); }
        }

        public void Init()
        {
            mMessageList = new List<MessageBody>();
            N_GET_SDK_MESSAGES_REF.Add(OnGetSDKMessagesHandler);
#if UNITY_IPHONE
            Register("earphone_interrupt", InterrupCallback, false);
#endif 
        }

#if UNITY_IPHONE
        private void InterrupCallback(string arg0)
        {
            JSONObject js = JSONObject.Create(arg0);
            if (js.GetBoolValue("is_interrupt"))
            {
                NoticeManager.Instance.SendNotice(Consts.NOTICE_SYSTEM_INTERRUPT);
            }
        }
#endif

        private void OnGetSDKMessagesHandler(INoticeBase<int> param)
        {
            ParamNotice<SDKMessages> value = param as ParamNotice<SDKMessages>;
            value.ParamValue = this;
        }

        private void MessageCallback(string msg)
        {
            int max = mMessageList.Count;
            if (max > 0)
            {
                List<MessageBody> willDeletes = new List<MessageBody>();
                MessageBody item;
                for (int i = 0; i < max; i++)
                {
                    item = mMessageList[i];
                    if (msg.IndexOf(item.Tag, System.StringComparison.Ordinal) >= 0)
                    {
                        UnityAction<string> cb = item.MethodBody;
                        if (item.RemovedAfterInvoked)
                        {
                            item.MethodBody = null;
                            willDeletes.Add(item);
                        }
                        else { }

                        cb?.Invoke(msg);

                        break;
                    }
                    else { }
                }
                max = willDeletes.Count;
                for (int i = 0; i < max; i++)
                {
                    mMessageList.Remove(willDeletes[i]);
                }
                willDeletes.Clear();
            }
            else { }
        }

        public void Register(string tag, UnityAction<string> msgCallback, bool removedAfterInvoked = true)
        {
            if (msgCallback == default || mMessageList.Find(m => m.Tag == tag) != default)
            {
                return;
            }
            else { }

            MessageBody message = new MessageBody() { Tag = tag, RemovedAfterInvoked = removedAfterInvoked, MethodBody = msgCallback };
            mMessageList.Add(message);
        }

        public void Unregister(string tag)
        {
            MessageBody body = mMessageList.Find(m => m.Tag == tag);
            if (body != null)
            {
                body.MethodBody = null;
                mMessageList.Remove(body);
            }
            else { }
        }

        private void OnDestroy()
        {
            for (int i = 0; i < mMessageList.Count; i++)
            {
                mMessageList[i].MethodBody = null;
            }
            mMessageList.Clear();
            mMessageList = null;
            
            N_GET_SDK_MESSAGES_REF.Remove(OnGetSDKMessagesHandler);
        }
    }
}
