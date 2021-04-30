using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#pragma warning disable
namespace ShipDock.SDK
{
    /// <summary>
    /// 
    /// <-请描述该类的功能->
    /// 
    /// Created by 田亚宗 on 2019/11/27.
    ///
    /// </summary>
    public sealed class SDKMac : IPlatformSDK, IWechatSDK, INativeSDK, IAlipaySDK, IOralEvaluation, ITalkingDataSDK
    {
        public void CopyContentToClipboard(string content)
        {
        }

        public void EnsurePermission(UnityAction<string> callback)
        {
        }

        public float GetDeviceAudioValue(AudioType type)
        {
            return 0;
        }

        public Sprite GetQRCodeData()
        {
            return null;
        }

        public bool HasPermisison(PermissionRequestCode permission)
        {
            return false;
        }

        public bool IsAppInstalled()
        {
            return false;
        }

        public void OnBegin(string missionId)
        {
        }

        public void OnChargeRequest(string orderId, string iapId, double currencyAmount, CurrencyType currencyType, double virtualCurrencyAmount, PaymentType payType)
        {
        }

        public void OnChargeSuccess(string orderId)
        {
        }

        public void OnCompleted(string missionId)
        {
            //throw new System.NotImplementedException();
        }

        //public void OnEvent(string eventId, Dictionary<string, string> eventData)
        //{
        //}

        //public void OnEvent(string eventId, Dictionary<string, int> eventData)
        //{
        //}

        public void OnEvent(string eventId, Dictionary<string, object> eventData)
        {
        }

        public void OnFailed(string missionId, string failedCause)
        {
        }

        public void OnPurchase(string item, int number, double price)
        {
        }

        public void OnReward(double virtualCurrencyAmount, string reason)
        {
        }

        public void OnUse(string item, int itemNumber)
        {
        }

        public void PullWechatAuth(UnityAction<string> callback)
        {
        }

        public void RequestPermission(UnityAction<string> callback, params PermissionRequestCode[] code)
        {
        }

        public void SendPayRequest(string json, UnityAction<string> callback)
        {
        }

        public void SendQrcodeRequest(UnityAction<string> loadQrcodeCallbak, UnityAction<string> scannedCallback, UnityAction<string> authFinishCallback)
        {
        }

        public void SetAccount(string account)
        {
        }

        public void SetAccountName(string acountName)
        {
        }

        public void SetAccountType(AcountType type)
        {
        }

        public void SetAge(int age)
        {
        }

        public void SetDeviceAudioValue(AudioType type, int value)
        {
        }

        public void SetGameServer(string server)
        {
        }

        public void SetGender(int gender = 0)
        {
        }

        public void SetLevel(int level = 1)
        {
        }

        public void SetLogDisabled()
        {
        }

        public void ShareImage(string description, byte[] imageData, ShareType where, UnityAction<string> callback = null)
        {
        }

        public void ShareWebpage(string url, string title, string content, ShareType where, UnityAction<string> callback = null, byte[] thumbData = null)
        {
        }

        public void ShowAlert(IAlertParam param)
        {
        }

        public void ShowAppStoreScore()
        {
        }

        public void StartRecord(IOralParam param, UnityAction<IOralServerResult> callback, UnityAction<string> volumCallback = null)
        {
        }

        public void StopRecord(UnityAction<string> callback)
        {
        }

        public void UpdateLocation(float latitude, float longitude)
        {
        }

        void INativeSDK.CopyContentToClipboard(string content)
        {
        }

        float INativeSDK.GetDeviceAudioValue(AudioType type)
        {
            return 0;
        }

#if UNITY_IOS
        void INativeSDK.GotoAppStore()
        {
        }

        string INativeSDK.GetIDFA()
        {
        }
#endif
        bool INativeSDK.HasPermisison(PermissionRequestCode permission)
        {
            return true;
        }

        void IPlatformSDK.Init()
        {
            
        }

        void INativeSDK.RequestPermission(UnityAction<string> callback, params PermissionRequestCode[] code)
        {
        }

        void INativeSDK.SetDeviceAudioValue(AudioType type, int value)
        {
        }

        void INativeSDK.ShowAlert(IAlertParam param)
        {
        }

        void INativeSDK.ShowAppStoreScore()
        {
        }

    }
}

