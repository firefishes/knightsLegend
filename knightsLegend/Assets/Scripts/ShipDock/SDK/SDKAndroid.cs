#define G_LOG
#define APPLY_TKD
#define APPLY_TKD_EVENT
#define _SHIP_DOCK_SDK

using ShipDock.Notices;
using ShipDock.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Events;

namespace ShipDock.SDK
{
    /// <summary>
    /// 
    /// Android SDK 插件交互类
    /// 
    /// Created by 田亚宗 on 2020/01/14.
    /// Modfire by Minghua.ji on 2021/03/29.
    /// 
    /// </summary>
    public class SDKAndroid : SDKAndroidBase, IWechatSDK, INativeSDK, IAlipaySDK, IOralEvaluation, ITalkingDataSDK
    {
        private AndroidJavaObject mWechat;
        private AndroidJavaObject mAlipay;
        private AndroidJavaObject mNative;
        private AndroidJavaObject mSmartOralEva;
        private AndroidJavaObject mMainActivity;
        private AndroidJavaObject mTalkingData;
        private SDKMessages mMessageBody;

        void Init()
        {
            ParamNotice<SDKMessages> notice = Pooling<ParamNotice<SDKMessages>>.From();
            SDKMessages.N_GET_SDK_MESSAGES_REF.Broadcast(notice);
            mMessageBody = notice.ParamValue;
            Pooling<ParamNotice<SDKMessages>>.To(notice);

            using (AndroidJavaClass mClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
            {
                mMainActivity = mClass.GetStatic<AndroidJavaObject>("currentActivity");
            }
            mMainActivity.Call("setUnityCallbackBody", mMessageBody.TargetName, mMessageBody.MethodName);
            mWechat = mMainActivity.Call<AndroidJavaObject>("getWXSDK");
            mAlipay = mMainActivity.Call<AndroidJavaObject>("getAlipaySDK");
            mNative = mMainActivity.Call<AndroidJavaObject>("getNativeUtil");
            mSmartOralEva = mMainActivity.Call<AndroidJavaObject>("getSmartOral");
            mTalkingData = mMainActivity.Call<AndroidJavaObject>("getTalkingData");
            mTalkingData.Call("init", SDKMessages.SDKChannel);
        }

        /// <summary>
        /// 检查用户之前是否拒绝过某个权限
        /// </summary>
        /// <returns><c>true</c>, if if user has refuse permission before was checked, <c>false</c> otherwise.</returns>
        /// <param name="code">Code.</param>
        public bool CheckIfUserHasRefusePermissionBefore(PermissionRequestCode code)
        {
            return mNative.Call<bool>("checkIfUserRefuseBefore", (int)code);
        }

        /// <summary>
        /// 检查微信token是否有效
        /// </summary>
        /// <param name="callback">Callback.</param>
        public void CheckAccessToken(string acessToken, string openid, UnityAction<string> callback)
        {
            CallMethod(mWechat, "isAcessTokenValide", acessToken, openid);
            Register("check_token", callback);
        }

        /// <summary>
        /// 刷新微信端token
        /// </summary>
        /// <param name="refreshToken">Refresh token.</param>
        /// <param name="callback">Callback.</param>
        public void RefreshToken(string refreshToken, UnityAction<string> callback)
        {
            Register("refresh_token", callback);
            CallMethod(mWechat, "refreshToken", refreshToken);
        }

        //-----------------------------微信接口-----------------------------------
        bool IWechatSDK.IsAppInstalled()
        {
            return CallMethod<AndroidJavaObject, bool>(mWechat, "isWXInstalled");
        }

        void IWechatSDK.PullWechatAuth(UnityAction<string> callback)
        {
            Register("get_token", callback);
            CallMethod(mWechat, "pullWXAuthorize");
        }

        void IWechatSDK.SendPayRequest(string json, UnityAction<string> callback)
        {
            Register("pay_result", callback);
            CallMethod(mWechat, "sendPayReq", json);
        }

        Sprite IWechatSDK.GetQRCodeData()
        {
            Texture2D texture = new Texture2D(470, 470, TextureFormat.ARGB32, false);
            texture.LoadImage(CallMethod<AndroidJavaObject, byte[]>(mWechat, "getQRCode"));
            return Sprite.Create(texture, new Rect(Vector2.zero, Vector2.one * 470), Vector2.zero);
        }

        void IWechatSDK.SendQrcodeRequest(UnityAction<string> loadQrcodeCallbak, UnityAction<string> scannedCallback, UnityAction<string> authFinishCallback)
        {
            Register("get_qrcode", loadQrcodeCallbak);
            Register("scanned_qrcode", scannedCallback);
            Register("auth_qrcode", authFinishCallback);
            CallMethod(mWechat, "sendQRCodeRequest");
        }

        void IWechatSDK.ShareImage(string description, byte[] imageData, ShareType where, UnityAction<string> callback)
        {
            Register("share_result", callback);
            CallMethod(mWechat, "shareImageToWX", description, imageData, (int)where);
        }

        void IWechatSDK.ShareWebpage(string url, string title, string content, ShareType where, UnityAction<string> callback = null, byte[] thumbData = null)
        {
            Register("share_result", callback);
            CallMethod(mWechat, "shareWebPageToWX", url, title, content, (int)where, thumbData);
        }

        //-----------------------------------------------------------------------

        //------------------------------安卓本地接口-------------------------------
        float INativeSDK.GetDeviceAudioValue(AudioType type)
        {
            return CallMethod<AndroidJavaObject, int>(mNative, "getAudioVolum", (int)type) / 100.0f;
        }

        void INativeSDK.ShowAppStoreScore() { }

        void INativeSDK.SetDeviceAudioValue(AudioType type, int value)
        {
            CallMethod(mNative, "setAudioVolum", (int)type, value);
        }

        bool INativeSDK.HasPermisison(PermissionRequestCode permission)
        {
            return CallMethod<AndroidJavaObject, bool>(mNative, "checkPermission", (int)permission);
        }

        void INativeSDK.RequestPermission(UnityAction<string> callback, params PermissionRequestCode[] permission)
        {
            mMessageBody.Register("permission_result", callback);
            int[] pers = permission.Cast<int>().ToArray();
            CallMethod(mNative, "requestPermission", pers);
        }

        void INativeSDK.CopyContentToClipboard(string content)
        {
            CallMethod(mNative, "copyTextToClipboard", content);
        }

        void INativeSDK.ShowAlert(IAlertParam param)
        {
            if (param == null)
            {
                return;
            }
            Register("alert_box", param.CallBack);
            CallMethod(mNative, "alertDialog", param.Title, param.Content, param.LeftButtonContent, param.RightButtonContent);
        }
#if UNITY_IOS
        void INativeSDK.GotoAppStore()
        {
        }

        string INativeSDK.GetIDFA()
        {
            return "";
        }
#endif
        //-----------------------------------------------------------------------

        //---------------------------支付宝接口------------------------------------
        bool IAlipaySDK.IsAppInstalled()
        {
            return CallMethod<AndroidJavaObject, bool>(mAlipay, "isAppInstalled");
        }

        void IAlipaySDK.SendPayRequest(string payOrder, UnityAction<string> callback)
        {
            Register("alipay_result", callback);
            CallMethod(mAlipay, "sendPayRequest", payOrder);
        }

        //void INativeSDK.OnShowAppStoreReviewId(string appid)
        //{

        //}

        //-----------------------------智聆评测-----------------------------------

        void IOralEvaluation.StartRecord(IOralParam param, UnityAction<IOralServerResult> callback, UnityAction<string> volumCallback)
        {
            Register("evaluation_result", s => callback.Invoke(new OralServerResult().ParseServerResult(s)));
            Register("eveluation_volum", volumCallback, false);
            CallMethod(mSmartOralEva, "startRecord", param.ToJson(), param.refText);
        }

        void IOralEvaluation.StopRecord(UnityAction<string> callback)
        {
            "log: =========^^^^^^^^^^^^^^^^^^===============  StopRecord =============^^^^^^^^^^^^^^^^^===========..".Log();

            Register("evaluation_stop", callback);
            UnRegister("eveluation_volum");
            CallMethod(mSmartOralEva, "stopRecord");
        }

        void IOralEvaluation.EnsurePermission(UnityAction<string> callback)
        {
            mMessageBody.Register("permission_result", callback);
            AddPermission(PermissionRequestCode.WRITE_EXTERNAL_STORAGE);
            AddPermission(PermissionRequestCode.RECORD_AUDIO);
            AddPermission(PermissionRequestCode.READ_PHONE_STATE);
            if (mPermissionList.Count != 0)
            {
                INativeSDK native = this as INativeSDK;
                native.RequestPermission(callback, mPermissionList.ToArray());
            }
            else
            {
                callback?.Invoke(null);
            }
        }

        private List<PermissionRequestCode> mPermissionList = new List<PermissionRequestCode>();
        private void AddPermission(PermissionRequestCode code)
        {
            INativeSDK native = this as INativeSDK;
            if (!native.HasPermisison(code))
            {
                mPermissionList.Add(code);
            }
        }

        //----------------------TalkingData--------------------------------------
        void ITalkingDataSDK.SetAccount(string account)
        {
#if APPLY_TKD
            if (!string.IsNullOrEmpty(account))
            {
                CallMethod(mTalkingData, "setAccount", account);
            }
#endif
        }

        void ITalkingDataSDK.SetAccountType(AcountType type)
        {
#if APPLY_TKD
            CallMethod(mTalkingData, "setAccountType", (int)type);
#endif
        }

        void ITalkingDataSDK.SetAccountName(string acountName)
        {
#if APPLY_TKD
            if (string.IsNullOrEmpty(acountName))
            {
                return;
            }
            CallMethod(mTalkingData, "setAccountName", acountName);
#endif
        }

        void ITalkingDataSDK.SetLevel(int level)
        {
#if APPLY_TKD
            if (level < 0)
            {
                return;
            }
            CallMethod(mTalkingData, "setLevel", level);
#endif
        }

        void ITalkingDataSDK.SetGender(int gender)
        {
#if APPLY_TKD
            CallMethod(mTalkingData, "setGender", gender);
#endif
        }

        void ITalkingDataSDK.SetAge(int age)
        {
#if APPLY_TKD
            if (age < 0 || age > 120)
            {
                return;
            }
            CallMethod(mTalkingData, "setAge", age);
#endif
        }

        void ITalkingDataSDK.SetGameServer(string server)
        {
#if APPLY_TKD
            if (string.IsNullOrEmpty(server))
            {
                return;
            }
            CallMethod(mTalkingData, "setGameServer", server);
#endif
        }

        void ITalkingDataSDK.OnChargeRequest(string orderId, string iapId, double currencyAmount, CurrencyType currencyType, double virtualCurrencyAmount, PaymentType payType)
        {
#if APPLY_TKD
            if (string.IsNullOrEmpty(orderId))
            {
                return;
            }
            CallMethod(mTalkingData, "onChargeRequest", orderId, iapId, currencyAmount, currencyType.ToString(), virtualCurrencyAmount, payType.ToString());
#endif
        }

        void ITalkingDataSDK.OnChargeSuccess(string orderId)
        {
#if APPLY_TKD
            if (string.IsNullOrEmpty(orderId))
            {
                return;
            }
            CallMethod(mTalkingData, "onChargeSuccess", orderId);
#endif
        }

        void ITalkingDataSDK.OnReward(double virtualCurrencyAmount, string reason)
        {
#if APPLY_TKD
            CallMethod(mTalkingData, "onReward", virtualCurrencyAmount, reason);
#endif
        }

        void ITalkingDataSDK.OnPurchase(string item, int number, double price)
        {
#if APPLY_TKD
            if (string.IsNullOrEmpty(item))
            {
                return;
            }
            CallMethod(mTalkingData, "onPurchase", item, number, price);
#endif
        }

        void ITalkingDataSDK.OnUse(string item, int itemNumber)
        {
#if APPLY_TKD
            if (string.IsNullOrEmpty(item))
            {
                return;
            }
            CallMethod(mTalkingData, "onUse", item, itemNumber);
#endif
        }

        void ITalkingDataSDK.OnBegin(string missionId)
        {
#if APPLY_TKD
            if (string.IsNullOrEmpty(missionId))
            {
                return;
            }
            CallMethod(mTalkingData, "onBegin", missionId);
#endif
        }

        void ITalkingDataSDK.OnCompleted(string missionId)
        {
#if APPLY_TKD
            if (string.IsNullOrEmpty(missionId))
            {
                return;
            }
            CallMethod(mTalkingData, "onCompleted", missionId);
#endif
        }

        void ITalkingDataSDK.OnFailed(string missionId, string failedCause)
        {
#if APPLY_TKD
            if (string.IsNullOrEmpty(missionId))
            {
                return;
            }
            CallMethod(mTalkingData, "onFailed", missionId, failedCause);
#endif
        }

        void ITalkingDataSDK.UpdateLocation(float latitude, float longitude)
        {

        }

        void ITalkingDataSDK.OnEvent(string eventId, Dictionary<string, object> eventData)
        {
            if (string.IsNullOrEmpty(eventId))
            {
                return;
            }

            if (eventData != null && eventData.Count > 0)
            {
                AndroidJavaObject map = new AndroidJavaObject("java.util.HashMap", eventData.Count);
                IntPtr method_put = AndroidJNIHelper.GetMethodID(map.GetRawClass(), "put", "(Ljava/lang/Object;Ljava/lang/Object;)Ljava/lang/Object;");
                object[] args = new object[2];
                foreach (var kv in eventData)
                {
                    args[0] = new AndroidJavaObject("java.lang.String", kv.Key);
                    if (typeof(String).IsInstanceOfType(kv.Value))
                    {
                        args[1] = new AndroidJavaObject("java.lang.String", kv.Value);
                    }
                    else
                    {
                        args[1] = new AndroidJavaObject("java.lang.Double",""+ kv.Value);
                    }

                    AndroidJNI.CallObjectMethod(map.GetRawClass(), method_put, AndroidJNIHelper.CreateJNIArgArray(args));
                }
                CallMethod(mTalkingData, "onEvent", eventId, map);
            }
            else
            {
                CallMethod(mTalkingData, "onEvent", eventId, null);
            }
        }

        void ITalkingDataSDK.SetLogDisabled()
        {
#if APPLY_TKD
            CallMethod(mTalkingData, "setLogDisable");
#endif
        }
        //-----------------------------------------------------------------------
    }

    public sealed class OralServerResult : IOralServerResult
    {
        [Serializable]
        /// <summary>
        /// 单词辅助类
        /// </summary>
        private class Word : IWord
        {
            public int BeginTime { get; set; }

            public int EndTime { get; set; }

            public int MatchTag { get; set; }

            public float PronAccuracy { get; set; }

            public float PronFluency { get; set; }

            public IPhoneInfo[] PhoneInfos { get; set; }

            public string Content { get; set; }
        }
        [Serializable]
        /// <summary>
        /// 音素辅助类
        /// </summary>
        private class PhoneInfo : IPhoneInfo
        {
            public int BeginTime { get; set; }

            public int EndTime { get; set; }

            public bool DetectedStress { get; set; }

            public string Phone { get; set; }

            public float PronAccuracy { get; set; }

            public bool Stress { get; set; }
        }
        /// <summary>
        /// 错误描述辅助类
        /// </summary>
        private class ErrorDsc : IError
        {
            public int Code { get; set; }

            public string Desc { get; set; }

            public string RequestId { get; set; }
        }

        public string AudioUrl { get; private set; }

        public string SessionId { get; private set; }

        public float PronAccuracy { get; private set; }

        public float PronCompletion { get; private set; }

        public float PronFluency { get; private set; }

        public float SuggestedScore { get; private set; }

        public IWord[] Words { get; private set; }

        public int Seq { get; private set; }

        public int End { get; private set; }

        public IError Error { get; private set; }

        public IOralServerResult ParseServerResult(string json)
        {
            try
            {
                if (string.IsNullOrEmpty(json))
                {
                    "error: ParseServerResult : json null".Log();
                    return null;
                }
                else { }

                "log".Log(json);

#if SHIP_DOCK_SDK
                //UnityEngine.Debug.Log("1");
                JSONObject tmp = JSONObject.Create(json);
                Seq = tmp.GetIntValue("seq");
                //UnityEngine.Debug.Log("2");
                End = tmp.GetIntValue("end");
                //UnityEngine.Debug.Log("3");
                JSONObject errJs = tmp.GetField("error");
                //UnityEngine.Debug.Log("4");
                ErrorDsc error = new ErrorDsc();
                error.Code = errJs.GetIntValue("code");
                //UnityEngine.Debug.Log("5");
                error.Desc = errJs.GetStringValue("desc");
                //UnityEngine.Debug.Log("6");
                error.RequestId = errJs.GetStringValue("requestId");
                //UnityEngine.Debug.Log("7");
                Error = error;
                JSONObject infos = tmp.GetField("ret");
                //UnityEngine.Debug.Log("8");
                AudioUrl = infos.GetStringValue("audioUrl");
                //UnityEngine.Debug.Log("9");
                PronAccuracy = infos.GetFloatValue("pronAccuracy");
                //UnityEngine.Debug.Log("10");
                PronFluency = infos.GetFloatValue("pronFluency");
                //UnityEngine.Debug.Log("11");
                PronCompletion = infos.GetFloatValue("pronCompletion");
                //UnityEngine.Debug.Log("12");
                SessionId = infos.GetStringValue("sessionId");
                //UnityEngine.Debug.Log("13");
                SuggestedScore = infos.GetFloatValue("suggestedScore");
                //UnityEngine.Debug.Log("14");
                JSONObject wordsjs = infos.GetField("words");
                //UnityEngine.Debug.Log("15");
                if (wordsjs.list.Count > 0)
                {
                    //UnityEngine.Debug.Log(string.Format("count:{0}",wordsjs.list.Count));
                    Word[] words = new Word[wordsjs.list.Count];
                    for (int i = 0; i < words.Length; i++)
                    {
                        words[i] = new Word();
                        JSONObject tempList = wordsjs.list[i];
                        // words[i].BeginTime = wordsjs.list[i].GetIntValue("beginTime");
                        words[i].BeginTime = tempList.GetIntValue("beginTime");
                        //UnityEngine.Debug.Log("16");
                        //words[i].EndTime = wordsjs.list[i].GetIntValue("endTime");
                        words[i].BeginTime = tempList.GetIntValue("beginTime");
                        //UnityEngine.Debug.Log("17");
                        // words[i].MatchTag = wordsjs.list[i].GetIntValue("matchTag");
                        words[i].MatchTag = tempList.GetIntValue("matchTag");
                        //UnityEngine.Debug.Log("18");
                        //words[i].PronFluency = wordsjs.list[i].GetFloatValue("pronFluency");
                        words[i].PronFluency = tempList.GetFloatValue("pronFluency");
                        //UnityEngine.Debug.Log("19");
                        // words[i].PronAccuracy = wordsjs.list[i].GetFloatValue("pronAccuracy");
                        words[i].PronAccuracy = tempList.GetFloatValue("pronAccuracy");
                        //UnityEngine.Debug.Log("20");
                        // words[i].Content = wordsjs.list[i].GetStringValue("word");
                        words[i].Content = tempList.GetStringValue("word");
                        //UnityEngine.Debug.Log("21");
                        // JSONObject phonesObj = wordsjs.list[i].GetField("phoneInfos");
                        JSONObject phonesObj = tempList.GetField("phoneInfos");

                        //  UnityEngine.Debug.Log("服务器返回结果，单词的音素长度：".Append());
                        if (phonesObj.list.Count > 0)
                        {
                            //UnityEngine.Debug.Log(string.Format("phonesObj.list.Count:{0}",phonesObj.list.Count));
                            PhoneInfo[] phoneInfos = new PhoneInfo[phonesObj.list.Count];
                            words[i].PhoneInfos = phoneInfos;
                            for (int j = 0; j < phonesObj.list.Count; j++)
                            {
                                phoneInfos[j] = new PhoneInfo();
                                JSONObject tempPhone = phonesObj.list[j];
                                // phoneInfos[j].BeginTime = phonesObj.list[j].GetIntValue("beginTime");
                                phoneInfos[j].BeginTime = tempPhone.GetIntValue("beginTime");
                                //UnityEngine.Debug.Log("23");
                                //phoneInfos[j].EndTime = phonesObj.list[j].GetIntValue("endTime");
                                phoneInfos[j].EndTime = tempPhone.GetIntValue("endTime");
                                //UnityEngine.Debug.Log("24");
                                //phoneInfos[j].DetectedStress = phonesObj.list[j].GetBoolValue("detectedStress");
                                phoneInfos[j].DetectedStress = tempPhone.GetBoolValue("detectedStress");
                                //UnityEngine.Debug.Log("25");
                                //  phoneInfos[j].Phone = phonesObj.list[j].GetStringValue("phone");
                                phoneInfos[j].Phone = tempPhone.GetStringValue("phone");
                                //UnityEngine.Debug.Log("26");
                                // phoneInfos[j].PronAccuracy = phonesObj.list[j].GetFloatValue("pronAccuracy");
                                phoneInfos[j].PronAccuracy = tempPhone.GetFloatValue("pronAccuracy");
                                //UnityEngine.Debug.Log("27");
                                //  phoneInfos[j].Stress = phonesObj.list[j].GetBoolValue("stress");
                                phoneInfos[j].Stress = tempPhone.GetBoolValue("stress");
                                //UnityEngine.Debug.Log("28");
                            }
                        }
                        else { }
                    }
                    Words = words;
                }
                else
                {
                    "log:Words null".Log();
                    Words = null;
                }
#endif
                return this;
            }
            catch (Exception ex)
            {
                "error: SDK ParseServerResult method expection，the return value IOralServerResult is null, details - {0}".Log(ex.ToString());
                return null;
            }
        }
    }

    public class OralEvaParam : IOralParam
    {
#if SHIP_DOCK_SDK
        private static JSONObject jSON = JSONObject.Create();
#endif
        private string mRefText = "";

        public string soeAppId { get; set; } = "";
        public string token { get; set; } = "";
        public EvalMode evalMode { get; set; } = EvalMode.Word;
        public WorkMode workMode { get; set; } = WorkMode.Once;
        public StorageMode storageMode { get; set; } = StorageMode.Disable;
        public ServerType serverType { get; set; } = ServerType.English;
        public TextMode textMode { get; set; } = TextMode.Phoneme;
        public float scoreCoeff { get; set; } = 1.0f;
        public float timeout { get; set; } = 5.0f;
        public int fragSize { get; set; } = 1;
        public bool fragEnable { get; set; } = false;
        public string refText
        {
            get { return mRefText; }
            set
            {
                if (textMode == TextMode.Phoneme)
                {
                    if (value.IndexOf("wordlist", StringComparison.Ordinal) >= 0)
                    {
                        mRefText = value;
                    }
                    else
                    {
                        mRefText = FormatText2Phoneme(value);
                    }
                }
                else
                {
                    mRefText = value;
                }
            }
        }

        private string FormatText2Phoneme(string text)
        {
            string[] elements = text.Trim().Split(new char[] { ' ', ',', '.' }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb = new StringBuilder();
            sb.Append("{\"wordlist\":[");
            for (int i = 0; i < elements.Length; i++)
            {
                sb.Append("{");
                sb.AppendFormat("\"word\":\"{0}\"", elements[i]);
                sb.Append("},");
            }
            sb.Remove(sb.Length - 1, 1);
            sb.Append("]}");
            return sb.ToString();
        }

        public string ToJson()
        {
#if SHIP_DOCK_SDK
            jSON.AddField(nameof(soeAppId), soeAppId);
            jSON.AddField(nameof(token), token);
            jSON.AddField(nameof(evalMode), (int)evalMode);
            jSON.AddField(nameof(workMode), (int)workMode);
            jSON.AddField(nameof(storageMode), (int)storageMode);
            jSON.AddField(nameof(serverType), (int)serverType);
            jSON.AddField(nameof(textMode), (int)textMode);
            jSON.AddField(nameof(scoreCoeff), scoreCoeff);
            jSON.AddField(nameof(fragSize), fragSize);
            jSON.AddField(nameof(fragEnable), fragEnable);
            jSON.AddField(nameof(timeout), timeout);
            string str = jSON.ToString();
            jSON.Clear();
            return str;
#else
            return string.Empty;
#endif
        }
    }
}
