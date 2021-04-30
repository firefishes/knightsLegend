using ShipDock.Notices;
using ShipDock.Pooling;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
#if UNITY_IPHONE
using UnityEngine.Purchasing;
#endif

#pragma warning disable
namespace ShipDock.SDK
{
    /// <summary>
    /// 
    /// 
    /// 
    /// Created by 田亚宗 on 2019/05/23.
    ///
    /// </summary>
    public sealed class SDKIos : IPlatformSDK, IWechatSDK, INativeSDK, IAlipaySDK, ITalkingDataSDK
#if UNITY_IPHONE
        , IApplePaySDK
#endif
    {
        [DllImport("__Internal")]
        private static extern void init();
        [DllImport("__Internal")]
        private static extern float getSystemVolum();
        [DllImport("__Internal")]
        private static extern bool isWXAppInstalled();
        [DllImport("__Internal")]
        private static extern bool isAlipayAppInstalled();
        [DllImport("__Internal")]
        private static extern void getQRCode();
        [DllImport("__Internal")]
        private static extern IntPtr getQRCodeData(out int length);
        [DllImport("__Internal")]
        private static extern void pullWechatAuth();
        [DllImport("__Internal")]
        private static extern void sendPayRequestToAlipayApp(string order);
        [DllImport("__Internal")]
        private static extern void sendPayRequestToWechat(string json);
        [DllImport("__Internal")]
        private static extern void shareImageToWX(string description, int where, byte[] imgData, int dataLength);
        [DllImport("__Internal")]
        private static extern void showAlert(string title, string msg, string lcontent, string rcontent, AlerStyle lstyle, AlerStyle rstyle);
        [DllImport("__Internal")]
        private static extern void showAppStoreScore(string appUrl);
        [DllImport("__Internal")]
        private static extern void goToAppStore(string appID);
        [DllImport("__Internal")]
        private static extern string getIosIDFA(); 
        [DllImport("__Internal")]
        private static extern bool hasPermission(int code);
        [DllImport("__Internal")]
        private static extern void requestPermisson(int permissionCode);
        [DllImport("__Internal")]
        private static extern void shareWebpageToWX(string title, string description, string url, int where, byte[] thumbData, int length);

        //-----------------------TalkingData-----------------------
        [DllImport("__Internal")]
        private static extern void onStart();
        [DllImport("__Internal")]
        private static extern void setLocation(double latitude, double longitude);
        [DllImport("__Internal")]
        private static extern void setAccount(string accountId);
        [DllImport("__Internal")]
        private static extern void setAccountName(string accountName);
        [DllImport("__Internal")]
        private static extern void setAccountType(int accountType);
        [DllImport("__Internal")]
        private static extern void setLevel(int level);
        [DllImport("__Internal")]
        private static extern void setGender(int gender);
        [DllImport("__Internal")]
        private static extern void setAge(int age);
        [DllImport("__Internal")]
        private static extern void setGameServer(string gameServer);
        [DllImport("__Internal")]
        private static extern void onBegin(string missionId);
        [DllImport("__Internal")]
        private static extern void onCompleted(string missionId);
        [DllImport("__Internal")]
        private static extern void onFailed(string missionId, string failedCause);
        [DllImport("__Internal")]
        private static extern void onChargeRequst(string orderId, string iapId, double currencyAmount, string currencyType, double virtualCurrencyAmount, string paymentType);
        [DllImport("__Internal")]
        private static extern void onChargeSuccess(string orderId);
        [DllImport("__Internal")]
        private static extern void onReward(double virtualCurrencyAmount, string reason);
        [DllImport("__Internal")]
        private static extern void onPurchase(string item, int itemNumber, double priceInVirtualCurrency);
        [DllImport("__Internal")]
        private static extern void onUse(string item, int itemNumber);
        [DllImport("__Internal")]
        private static extern void onEvent(string eventId, string[] keys, string[] stringValuse, double[] numberValuse, int count);
        [DllImport("__Internal")]
        private static extern void setLogDisabled();
        //---------------------------------------------------------

        private SDKMessages mMessageBody;
        private static bool hasTokenBeenObtained = false;

        void IPlatformSDK.Init()
        {
            init();
            onStart();

            ParamNotice<SDKMessages> notice = Pooling<ParamNotice<SDKMessages>>.From();
            SDKMessages.N_GET_SDK_MESSAGES_REF.Broadcast(notice);
            mMessageBody = notice.ParamValue;
            Pooling<ParamNotice<SDKMessages>>.To(notice);
        }

        private void Register(string tag, UnityAction<string> callback)
        {
            if (callback != null && !string.IsNullOrEmpty(tag))
            {
                mMessageBody.Register(tag, callback);
            }
            else { }
        }

        //------------------------------微信接口----------------------------------
        Sprite IWechatSDK.GetQRCodeData()
        {
            int length = -1;
            IntPtr byteData = getQRCodeData(out length);
            byte[] bytes = new byte[length];
            if (length > 0)
            {
                Marshal.Copy(byteData, bytes, 0, length);
                Marshal.FreeHGlobal(byteData);
            }
            else
            {
                throw new Exception("二维码图片数组为空或者没有数据....");
            }
            Texture2D texture = new Texture2D(470, 470, TextureFormat.ARGB32, false);
            texture.LoadImage(bytes);
            return Sprite.Create(texture, new Rect(Vector2.zero, Vector2.one * 470), Vector2.zero);
        }

        bool IWechatSDK.IsAppInstalled()
        {
            return isWXAppInstalled();
        }

        void IWechatSDK.PullWechatAuth(UnityAction<string> callback)
        {
            Register("get_token", callback);
            pullWechatAuth();
        }

        void IWechatSDK.SendPayRequest(string json, UnityAction<string> callback)
        {
            Register("pay_result", callback);
            sendPayRequestToWechat(json);
        }

        void IWechatSDK.SendQrcodeRequest(UnityAction<string> loadQrcodeCallbak, UnityAction<string> scannedCallback, UnityAction<string> authFinishCallback)
        {
            Register("get_qrcode", loadQrcodeCallbak);
            Register("scanned_qrcode", scannedCallback);
            Register("auth_qrcode", authFinishCallback);
            getQRCode();
        }

        void IWechatSDK.ShareImage(string description, byte[] imageData, ShareType where, UnityAction<string> callback)
        {
            if (imageData == null || imageData.Length <= 0)
            {
                return;
            }
            Register("share_result", callback);
            shareImageToWX(description, (int)where, imageData, imageData.Length);
        }

        void IWechatSDK.ShareWebpage(string url, string title, string content, ShareType where, UnityAction<string> callback, byte[] thumbData = null)
        {
            Register("share_result", callback);
            if (thumbData != null)
            {
                shareWebpageToWX(title, content, url, (int)where, thumbData, thumbData.Length);
            }
            else
            {
                shareWebpageToWX(title, content, url, (int)where, null, -1);
            }
        }
        //-----------------------------------------------------------------------

        //--------------------------------------Ios 本地接口-----------------------
        float INativeSDK.GetDeviceAudioValue(AudioType type)
        {
            return getSystemVolum();
        }

        void INativeSDK.ShowAppStoreScore()
        {
            //showAppStoreScore(Consts.IOS_APPID);
            showAppStoreScore("1474298553");
        }
#if UNITY_IOS
        void INativeSDK.GotoAppStore()
        {
            goToAppStore("1474298553");
        }

        string INativeSDK.GetIDFA()
        {
            return getIosIDFA();
        }
#endif
        void INativeSDK.SetDeviceAudioValue(AudioType type, int value)
        {

        }

        void INativeSDK.CopyContentToClipboard(string content)
        {
            GUIUtility.systemCopyBuffer = content;
        }

        void INativeSDK.ShowAlert(IAlertParam param)
        {
            if (param.CallBack == null)
            {
                return;
            }
            else { }

            Register("alert_box", param.CallBack);
            Register("alert_show", param.OnShow);
            showAlert(param.Title, param.Content, param.LeftButtonContent, param.RightButtonContent, param.LeftButtonStyle, param.RightButtonStyle);
        }

        bool INativeSDK.HasPermisison(PermissionRequestCode permission)
        {
            return hasPermission((int)permission);
        }

        void INativeSDK.RequestPermission(UnityAction<string> callback, params PermissionRequestCode[] code)
        {
#if SHIP_DOCK_SDK
            if (code.Length > 1)
            {
                List<int> codelist = code.Cast<int>().ToList();
                JSONObject result = JSONObject.Create();
                result.AddField("tag", "permission_result");
                result.AddField("permission_code", 10020);
                result.AddField("grant_result", JSONObject.Create());
                RequestPermissions(codelist, callback,result);
            }
            else
            {
                mMessageBody.Register("permission_result", callback);
                requestPermisson((int)code[0]);
            }
#endif
        }

#if SHIP_DOCK_SDK
        private void RequestPermissions(List<int> code, UnityAction<string>  callback,JSONObject result)
        {
            UnityAction<string> custom = s =>
            {
                JSONObject res = JSONObject.Create(s);
                bool isOk = res.GetIntValue("status_code") == 3;
                result["grant_result"].AddField("ios.permission."+ Enum.GetName(typeof(PermissionRequestCode),code[0]),isOk?0:-1);
                code.RemoveAt(0);
                if (code.Count > 0)
                {
                    RequestPermissions(code, callback, result);
                }
                else
                {
                    callback?.Invoke(result.ToString());
                }
            };
            mMessageBody.Register("permission_result", custom);
            requestPermisson(code[0]);
        }
#endif

        //-----------------------------------------------------------------------

        //--------------------------支付宝接口-------------------------------------
        bool IAlipaySDK.IsAppInstalled()
        {
            return isAlipayAppInstalled();
        }

        void IAlipaySDK.SendPayRequest(string payOrder, UnityAction<string> callback)
        {
            Register("alipay_result", callback);
            sendPayRequestToAlipayApp(payOrder);
        }
        //-----------------------------------------------------------------------

        //---------------------------数据统计-------------------------------------
        void ITalkingDataSDK.SetAccount(string account)
        {
#if PUBLIC
            setLogDisabled();
#endif
            if (!string.IsNullOrEmpty(account))
            {
                setAccount(account);
            }
            else { }

        }

        void ITalkingDataSDK.SetAccountType(AcountType type)
        {
            setAccountType((int)type);
        }

        void ITalkingDataSDK.SetAccountName(string accountName)
        {
            if (!string.IsNullOrEmpty(accountName))
            {
                setAccountName(accountName);
            }
            else { }

        }

        void ITalkingDataSDK.SetLevel(int level)
        {
            setLevel(level);
        }

        void ITalkingDataSDK.SetGender(int gender)
        {
            setGender(gender);
        }

        void ITalkingDataSDK.SetAge(int age)
        {
            if (age >= 0 && age <= 120)
            {
                setAge(age);
            }
            else { }

        }

        void ITalkingDataSDK.SetGameServer(string server)
        {
            if (!string.IsNullOrEmpty(server))
            {
                setGameServer(server);
            }
            else { }

        }

        void ITalkingDataSDK.OnChargeRequest(string orderId, string iapId, double currencyAmount, CurrencyType currencyType, double virtualCurrencyAmount, PaymentType payType)
        {
            if (!string.IsNullOrEmpty(orderId))
            {
                onChargeRequst(orderId, iapId, currencyAmount, currencyType.ToString(), virtualCurrencyAmount, payType.ToString());
            }
            else { }

        }

        void ITalkingDataSDK.OnChargeSuccess(string orderId)
        {
            if (!string.IsNullOrEmpty(orderId))
            {
                onChargeSuccess(orderId);
            }
            else { }

        }

        void ITalkingDataSDK.OnReward(double virtualCurrencyAmount, string reason)
        {
            onReward(virtualCurrencyAmount, reason);
        }

        void ITalkingDataSDK.OnPurchase(string item, int number, double price)
        {
            if (!string.IsNullOrEmpty(item))
            {
                onPurchase(item, number, price);
            }
            else { }

        }

        void ITalkingDataSDK.OnUse(string item, int itemNumber)
        {
            if (!string.IsNullOrEmpty(item))
            {
                onUse(item, itemNumber);
            }
            else { }

        }

        void ITalkingDataSDK.OnBegin(string missionId)
        {
            if (!string.IsNullOrEmpty(missionId))
            {
                onBegin(missionId);
            }
            else { }

        }

        void ITalkingDataSDK.OnCompleted(string missionId)
        {
            if (!string.IsNullOrEmpty(missionId))
            {
                onCompleted(missionId);
            }
            else { }

        }

        void ITalkingDataSDK.OnFailed(string missionId, string failedCause)
        {
            if (!string.IsNullOrEmpty(missionId))
            {
                onFailed(missionId, failedCause);
            }
            else { }

        }

        void ITalkingDataSDK.UpdateLocation(float latitude, float longitude)
        {
            setLocation(latitude, longitude);
        }

        void ITalkingDataSDK.OnEvent(string eventId, Dictionary<string, object> eventData)
        {
            if (string.IsNullOrEmpty(eventId))
            {
                return;
            }
            else { }

            if (eventData != null && eventData.Count > 0)
            {
                int count = eventData.Count;
                string[] keys = new string[count];
                string[] stringValues = new string[count];
                double[] numberValues = new double[count];
                int index = 0;
                foreach (KeyValuePair<string, object> kvp in eventData)
                {
                    if (kvp.Value is string)
                    {
                        keys[index] = kvp.Key;
                        stringValues[index] = (string)kvp.Value;
                    }
                    else
                    {
                        try
                        {
                            double tmp = System.Convert.ToDouble(kvp.Value);
                            numberValues[index] = tmp;
                            keys[index] = kvp.Key;
                        }
                        catch (System.Exception)
                        {
                            count--;
                            continue;
                        }
                    }
                    index++;

                }
                onEvent(eventId, keys, stringValues, numberValues, count);
            }
            else
            {
                onEvent(eventId, null, null, null, 0);
            }
        }

        void ITalkingDataSDK.SetLogDisabled()
        {
            setLogDisabled();
        }

        //-----------------------------------苹果内购--------------------------------------------

#if UNITY_IPHONE
        private IStoreController mController;
        private Action<Product> onDeferred;
        private Action<UnifiedReceipt> onBuySuccess;
        private Action<bool, string> onInitResult;
        private Action<Product, PurchaseFailureReason> onBuyFailed;
        private bool mIsInit = false;

        void IApplePaySDK.Init(int[] ids, Action<bool, string> onInitResult = null)
        {
            if (mIsInit)
            {
                return;
            }
            mIsInit = true;
            this.onInitResult = onInitResult;
            StandardPurchasingModule module = StandardPurchasingModule.Instance();
            ConfigurationBuilder builder = ConfigurationBuilder.Instance(module);
            for (int i = 0; i < ids.Length; i++)
            {
                builder.AddProduct(Application.identifier + "_" + ids[i].ToString(), ProductType.Consumable);
            }
            UnityPurchasing.Initialize(this, builder);
        }

        void IApplePaySDK.SendPayRequest(string productId, Action<UnifiedReceipt> onBuySuccess = null, Action<Product, PurchaseFailureReason> onBuyFailed = null)
        {
            if (mController != null)
            {
                if (string.IsNullOrEmpty(productId))
                {
                    UIUtils.ShowTip(this, "the product info is null or empty...");
                    return;
                }

                this.onBuySuccess = onBuySuccess;
                this.onBuyFailed = onBuyFailed;
                mController.InitiatePurchase(productId);
            }
            else
            {
                Debug.LogError("pay controller is null..");
            }
        }

        PurchaseProcessingResult IStoreListener.ProcessPurchase(PurchaseEventArgs e)
        {
            UnifiedReceipt receipt = JsonUtility.FromJson<UnifiedReceipt>(e.purchasedProduct.receipt);
            onBuySuccess?.Invoke(receipt);
            return PurchaseProcessingResult.Complete;
        }

        void IStoreListener.OnPurchaseFailed(Product i, PurchaseFailureReason p)
        {
            onBuyFailed?.Invoke(i, p);
        }

        void IStoreListener.OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            mController = controller;
            extensions.GetExtension<IAppleExtensions>().RegisterPurchaseDeferredListener(onDeferred);
            if (onInitResult != null)
            {
                onInitResult.Invoke(true, string.Empty);
                onInitResult = null;
            }
        }

        void IStoreListener.OnInitializeFailed(InitializationFailureReason error)
        {
            if (onInitResult != null)
            {
                onInitResult.Invoke(false, error.ToString());
                onInitResult = null;
            }
            Debug.LogError("payment init failed...." + error.ToString());
        }

        //--------------------------------------------------------------------------------------
#endif
    }
}

