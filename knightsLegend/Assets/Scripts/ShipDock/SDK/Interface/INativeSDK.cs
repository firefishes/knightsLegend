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
    public interface INativeSDK
    {
        /// <summary>
        /// 获取系统音量
        /// </summary>
        /// <returns>音量值(0-1之间).</returns>
        /// <param name="type">音量类型.</param>
        float GetDeviceAudioValue(AudioType type);
        /// <summary>
        /// 设置设备音量
        /// </summary>
        /// <param name="type">音量类型.</param>
        /// <param name="value">要设置的音量值.</param>
        void SetDeviceAudioValue(AudioType type, int value);
        /// <summary>
        /// 拷贝内容至系统剪切板
        /// </summary>
        /// <param name="content">要拷贝的内容.</param>
        void CopyContentToClipboard(string content);
        /// <summary>
        /// 显示系统原生提示框
        /// </summary>
        /// <param name="title">标题.</param>
        /// <param name="msg">内容.</param>
        void ShowAlert(IAlertParam param);
        /// <summary>
        /// 检查应用是否拥有某项权限
        /// </summary>
        /// <param name="permission">Permission.</param>
        bool HasPermisison(PermissionRequestCode permission);
        /// <summary>
        /// 应用向系统请求权限
        /// </summary>
        /// <param name="code">权限码.</param>
        /// <param name="callback">用户授权回调.</param>
        void RequestPermission(UnityAction<string> callback, params PermissionRequestCode[] code);
        /// <summary>
		/// 拉取ios内应用评分
		/// </summary>
		void ShowAppStoreScore();
#if UNITY_IOS
        /// <summary>
        /// 跳转到App Store
        /// </summary>
        void GotoAppStore();
        /// <summary>
        /// 获取idfa标识
        /// </summary>
        /// <returns></returns>
        string GetIDFA();
#endif
    }

    public interface IAlertParam
    {
        /// <summary>
        /// 弹窗标题
        /// </summary>
        string Title { get; }
        /// <summary>
        /// 弹窗内容
        /// </summary>
        string Content { get; }
        /// <summary>
        /// 弹窗左侧按钮内容
        /// </summary>
        string LeftButtonContent { get; }
        /// <summary>
        /// 弹窗右侧按钮内容
        /// </summary>
        string RightButtonContent { get; }
        /// <summary>
        /// 左侧按钮样式
        /// </summary>
        AlerStyle LeftButtonStyle { get; }
        /// <summary>
        /// 右侧按钮样式
        /// </summary>
        AlerStyle RightButtonStyle { get; }
        /// <summary>
        /// 弹窗关闭后的回调
        /// </summary>
        UnityAction<string> CallBack { get; }
        /// <summary>
        /// 弹窗传出现后的回调
        /// </summary>
        UnityAction<string> OnShow { get; }

        void SetButtonText(string left,string right);
        void SetButtonStyle(AlerStyle left, AlerStyle right);
        void SetAlertAction(UnityAction<string> callback, UnityAction<string> onshow = null);
    }
    
    public enum AlerStyle
    {
        /// <summary>
        /// 默认的确认按钮
        /// </summary>
        UIAlertActionStyleDefault = 0,
        /// <summary>
        /// 默认的取消按钮
        /// </summary>
        UIAlertActionStyleCancel,
        /// <summary>
        /// 默认的红色按钮
        /// </summary>
        UIAlertActionStyleDestructive
    }

    public enum AudioType
    {
        /// <summary>
        /// 通话音量
        /// </summary>
        Voice = 0,
        /// <summary>
        /// 系统音量
        /// </summary>
        System = 1,
        /// <summary>
        /// 铃声
        /// </summary>
        Ring = 2,
        /// <summary>
        /// 音乐
        /// </summary>
        Music = 3,
        /// <summary>
        /// 提示音量
        /// </summary>
        Alarm = 4
    }

    public enum PermissionStatus
    {
        /// <summary>
        /// 用户状态未确定
        /// </summary>
        NotDetermined = 0,
        /// <summary>
        /// 用户拒绝授权
        /// </summary>
        Denied = 2,
        /// <summary>
        /// 用户授权
        /// </summary>
        Authorized = 3
    }

    public enum PermissionRequestCode
    {
        /// <summary>
        /// 读取日历
        /// </summary>
        READ_CALENDAR = 0,
        /// <summary>
        /// 写日历
        /// </summary>
        WRITE_CALENDAR = 1,
        /// <summary>
        /// 摄像机权限
        /// </summary>
        CAMERA = 2,
        /// <summary>
        /// 读通讯录权限
        /// </summary>
        READ_CONTACTS = 3,
        /// <summary>
        /// 写通讯录权限
        /// </summary>
        WRITE_CONTACTS = 4,
        /// <summary>
        /// 定位权限
        /// </summary>
        ACCESS_FINE_LOCATION = 5,
        /// <summary>
        /// 定位权限
        /// </summary>
        ACCESS_COARSE_LOCATION = 6,
        /// <summary>
        /// 录音权限
        /// </summary>
        RECORD_AUDIO = 7,
        /// <summary>
        /// 读取手机设备信息
        /// </summary>
        READ_PHONE_STATE = 8,
        /// <summary>
        /// 发送信息
        /// </summary>
        SEND_SMS = 9,
        /// <summary>
        /// 读取信息
        /// </summary>
        READ_SMS = 10,
        /// <summary>
        /// 读存储设备
        /// </summary>
        READ_EXTERNAL_STORAGE = 11,
        /// <summary>
        /// 写存储设备
        /// </summary>
        WRITE_EXTERNAL_STORAGE = 12
    }
}

