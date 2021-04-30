using System;
using UnityEngine;

#pragma warning disable
namespace ShipDock.SDK
{
    /// <summary>
    /// 
    /// <-请描述该类的功能->
    /// 
    /// Created by 田亚宗 on 2020/01/14.
    ///
    /// </summary>
    public sealed class SDKHuawei : SDKAndroidBase, IHuaweiSDK
    {
        private AndroidJavaObject mActivity;

        public SDKHuawei()
        {
        }

        void IHuaweiSDK.CheckUpdate(Action<IHuaweiSDKResult> result)
        {
            Register("hms_checkUpdate", s => { result.Invoke(JsonUtility.FromJson<HuaweiSDKResult>(s)); });
            CallMethod(mActivity, "checkUpdate");
        }

        void IHuaweiSDK.Init(Action<IHuaweiSDKResult> initResult)
        {
            mActivity = GetTarget("getHuaweiSDK");
            Register("hms_connect", s => { initResult.Invoke(JsonUtility.FromJson<HuaweiSDKResult>(s)); });
            CallMethod(mActivity, "initSdk");
        }

        void IHuaweiSDK.Pay(IHuaweiPayParam payParam, Action<IHuaweiPayResult> result)
        {
            throw new NotImplementedException();
        }

        void IHuaweiSDK.SignIn(Action<IHuaweiSDKResult> result, bool foreLogin = true)
        {
            Register("hms_login", s => { result.Invoke(JsonUtility.FromJson<HuaweiSDKResult>(s)); });
            CallMethod(mActivity, "signIn", foreLogin);
        }

        void IHuaweiSDK.SignOut(Action<IHuaweiSDKResult> result)
        {
            Register("hms_signout", s => { result.Invoke(JsonUtility.FromJson<HuaweiSDKResult>(s)); });
            CallMethod(mActivity, "signOut");
        }

    }

    [Serializable]
    public class HuaweiSDKResult : IHuaweiSDKResult
    {
        public int code;
        public int status_code;
        public bool is_success;
        public HuaweiUserInfo user_info;

        int IHuaweiSDKResult.Code => code;

        bool IHuaweiSDKResult.IsSignOutSuccces => is_success;

        int IHuaweiSDKResult.StatusCode => status_code;

        IHuaweiUserInfo IHuaweiSDKResult.UserInfo => user_info;
    }

    [Serializable]
    public class HuaweiUserInfo : IHuaweiUserInfo
    {
        public string openId;
        public string disPlayName;
        public string photoUrl;
        public string accessToken;
        public string unionId;

        string IHuaweiUserInfo.OpenId => openId;

        string IHuaweiUserInfo.NickName => disPlayName;

        string IHuaweiUserInfo.PhotoUrl => photoUrl;

        string IHuaweiUserInfo.AccessToken => accessToken;

        string IHuaweiUserInfo.UnionId => unionId;
    }
}

