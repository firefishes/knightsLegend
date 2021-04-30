using System;

#pragma warning disable
namespace ShipDock.SDK
{
    /// <summary>
    /// 
    /// <-请描述该类的功能->
    /// 
    /// Created by 田亚宗 on 2020/01/13.
    ///
    /// </summary>
    public interface IHuaweiSDK
	{
        /// <summary>
        /// 请求其他方法之前必须先调用init完成初始化,返回code值为0时,初始化成功
        /// </summary>
        /// <param name="initResult">初始化结果回调</param>
        void Init(Action<IHuaweiSDKResult> initResult);
        /// <summary>
        /// 检测应用是否有更新,code为0时没有更新,1时有新版本,-1为验证错误需联系华为联运技术人员
        /// </summary>
        /// <param name="result">结果返回回调</param>
        void CheckUpdate(Action<IHuaweiSDKResult> result);
        /// <summary>
        /// 请求授权登录华为账号
        /// </summary>
        /// <param name="result">登录结果回调哦</param>
        /// <param name="foreLogin">是否强制登录,保持默认</param>
        void SignIn(Action<IHuaweiSDKResult> result, bool foreLogin = true);
        /// <summary>
        /// 登出华为账号
        /// </summary>
        /// <param name="result">登出结果回调</param>
        void SignOut(Action<IHuaweiSDKResult> result);
        /// <summary>
        /// 请求华为支付
        /// </summary>
        /// <param name="payParam">支付参数</param>
        /// <param name="result">支付结果回调</param>
        void Pay(IHuaweiPayParam payParam, Action<IHuaweiPayResult> result);
	}

    public interface IHuaweiSDKResult
    {
        /// <summary>
        /// 请求结果返回码
        /// </summary>
        int Code { get; }
        /// <summary>
        /// 是否登出成功
        /// </summary>
        bool IsSignOutSuccces { get; }
        /// <summary>
        /// 登出状态返回码
        /// </summary>
        int StatusCode { get; }
        /// <summary>
        /// 华为用户信息
        /// </summary>
        IHuaweiUserInfo UserInfo { get; }
    }

    public interface IHuaweiUserInfo
    {
        string OpenId { get; }
        string NickName { get; }
        string PhotoUrl { get; }
        string AccessToken { get; }
        string UnionId { get; }
    }

    public interface IHuaweiPayParam
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        string ProductNo { get; }
        /// <summary>
        /// 商户ID
        /// </summary>
        string MerchantId { get; }
        /// <summary>
        /// 支付订单号
        /// </summary>
        string RequestId { get; }
        /// <summary>
        /// 商户名称
        /// </summary>
        string MerchantName { get; }
        /// <summary>
        /// 商户保留信息，选填不参与签名
        /// </summary>
        string ReservationInfo { get; }
    }

    public interface IHuaweiPayResult
    {
        int Code { get; }
        int RetCode { get; }
        bool PaySuccess { get; }
        bool SignOk { get; }
        bool CheckOrder { get; }
    }
}

