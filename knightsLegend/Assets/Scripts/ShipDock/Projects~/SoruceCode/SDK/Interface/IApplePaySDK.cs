using System;
#if UNITY_IPHONE
using UnityEngine;
using UnityEngine.Purchasing;
#endif

#pragma warning disable
namespace ShipDock.SDK
{
    /// <summary>
    /// 
    /// 苹果支付接口
    /// 
    /// Created by 田亚宗 on 2019/08/13.
    ///
    /// </summary>
#if UNITY_IPHONE
	public interface IApplePaySDK:IStoreListener
#else
    public interface IApplePaySDK
#endif
    {
        /// <summary>
        /// 初始化苹果支付组件
        /// </summary>
        void Init(int[] ids,Action<bool,string> onInitResult=null);
#if UNITY_IPHONE
        /// <summary>
        /// 发起苹果支付请求
        /// </summary>
        void SendPayRequest(string productId, Action<UnifiedReceipt> onBuySuccess = null, Action<Product, PurchaseFailureReason> onBuyFailed = null);
#endif
    }
}

