using UnityEngine;
using UnityEngine.Events;

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
	public interface IWechatSDK
	{
        /// <summary>
        /// 是否安装了微信
        /// </summary>
        bool IsAppInstalled();

        /// <summary>
        /// 本地拉起微信进行登录授权
        /// </summary>
        /// <param name="callback">回调.</param>
        void PullWechatAuth(UnityAction<string> callback);

        /// <summary>
        /// 发送支付请求
        /// </summary>
        /// <param name="json">支付json数据.</param>
        /// <param name="callback">回调.</param>
        void SendPayRequest(string json, UnityAction<string> callback);

        /// <summary>
        /// 获取微信扫码登录的二维码
        /// </summary>
        /// <returns>二维码图片字节数组.</returns>
        Sprite GetQRCodeData();

        /// <summary>
        /// 请求登录二维码
        /// </summary>
        /// <param name="loadQrcodeCallbak">服务端返回二维码后的回调.</param>
        /// <param name="scannedCallback">二维码被扫描后的回调.</param>
        /// <param name="authFinishCallback">扫完码后的微信回调.</param>
        void SendQrcodeRequest(UnityAction<string> loadQrcodeCallbak, UnityAction<string> scannedCallback, UnityAction<string> authFinishCallback);

        /// <summary>
        /// 分享图片到微信
        /// </summary>
        /// <param name="description">图片描述.</param>
        /// <param name="imageData">图片数据.</param>
        /// <param name="where">分享到哪.</param>
        /// <param name="callback">分享后的回调.</param>
        void ShareImage(string description, byte[] imageData, ShareType where, UnityAction<string> callback = null);

        /// <summary>
        /// 分享网页到微信
        /// </summary>
        /// <param name="url">分享的网址.</param>
        /// <param name="title">标题.</param>
        /// <param name="content">分享内容说明.</param>
        /// <param name="where">分享到哪.</param>
        /// <param name="callback">分享后的回调.</param>
        void ShareWebpage(string url, string title, string content, ShareType where, UnityAction<string> callback = null, byte[] thumbData = null);
	}

    public enum ShareType
    {
        /// <summary>微信会话内</summary>
        SceneSession = 0,
        /// <summary>微信朋友圈</summary>
        SceneTimeline = 1,
        /// <summary>微信收藏</summary>
        SceneFavourate = 2
    }
}

