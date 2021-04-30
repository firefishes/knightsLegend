using ShipDock.Notices;
using ShipDock.Pooling;
using UnityEngine;
using UnityEngine.Events;

#pragma warning disable
namespace ShipDock.SDK
{
    /// <summary>
    /// 
    /// Android 原生交互基类，用于实现与平台相关的交互
    /// 
    /// Created by 田亚宗 on 2020/01/14.
    /// Modfire by Minghua.ji on 2021/03/29.
    ///
    /// </summary>
    public abstract class SDKAndroidBase : IPlatformSDK
    {
        private static AndroidJavaObject mMainActivity;
        private static SDKMessages mMessageBody;

        void IPlatformSDK.Init()
        {
            if (mMessageBody == null)
            {
                ParamNotice<SDKMessages> notice = Pooling<ParamNotice<SDKMessages>>.From();
                SDKMessages.N_GET_SDK_MESSAGES_REF.Broadcast(notice);
                mMessageBody = notice.ParamValue;
                Pooling<ParamNotice<SDKMessages>>.To(notice);
            }
            else { }

            if (mMainActivity == null)
            {
                using (AndroidJavaClass mClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    mMainActivity = mClass.GetStatic<AndroidJavaObject>("currentActivity");
                    mMainActivity.Call("setUnityCallbackBody", mMessageBody.TargetName, mMessageBody.MethodName);
                }
            }
            else { }
        }

        protected AndroidJavaObject GetTarget(string methodName)
        {
            if (string.IsNullOrEmpty(methodName))
            {
                return null;
            }
            else { }
            return mMainActivity.Call<AndroidJavaObject>(methodName);
        }

        protected void CallMethod<T>(T target, string method, params object[] args) where T : AndroidJavaObject
        {
            if (args == null)
            {
                target.Call(method);
            }
            else
            {
                target.Call(method, args);
            }
        }

        protected TResult CallMethod<T, TResult>(T target, string method, params object[] args) where T : AndroidJavaObject
        {
            if (args == null)
            {
                return target.Call<TResult>(method);
            }
            else { }
            return target.Call<TResult>(method, args);
        }

        protected void Register(string tag, UnityAction<string> callback, bool removedAfterInvoke = true)
        {
            if (callback != null && !string.IsNullOrEmpty(tag))
            {
                mMessageBody.Register(tag, callback, removedAfterInvoke);
            }
            else { }
        }

        protected void UnRegister(string tag)
        {
            if (!string.IsNullOrEmpty(tag))
            {
                mMessageBody.Unregister(tag);
            }
            else { }
        }
    }
}

