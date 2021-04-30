#pragma warning disable
using UnityEngine.Events;

namespace ShipDock.SDK
{
    /// <summary>
    /// 
    /// 
    /// 
    /// Created by 田亚宗 on 2019/05/31.
    ///
    /// </summary>
    public interface ITencentCloud
	{
        /// <summary>
        /// 确保语音服务权限均开启
        /// </summary>
        /// <param name="callback">权限回调.</param>
        void EnsurePermission(UnityAction<string> callback);
        /// <summary>
        /// 开始录音(开始之前请务必先调用EnsurePermission()方法,以确保权限)
        /// </summary>
        void StartRecord();
        /// <summary>
        /// 停止录音
        /// </summary>
        void StopRecord();
        /// <summary>
        /// 取消录音
        /// </summary>
        void CancelRecord();
	}	
}

