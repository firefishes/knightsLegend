using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

#pragma warning disable
namespace ShipDock.SDK
{
    public interface ITalkingDataSDK
    {
        /// <summary>
        /// 设置帐户
        /// </summary>
        /// <param name="account">设定帐户唯一标识，用于区分一个玩家，最多64个字符。其他调用依赖于此 ID。如果无玩家账户或期望以设备为单位计算玩家，调用时传入uuid即可</param>
        void SetAccount(string account);
        /// <summary>
        /// 设置帐户类型
        /// </summary>
        /// <param name="type">账户类型</param>
        void SetAccountType(AcountType type);
        /// <summary>
        /// 设置帐户的显性名
        /// </summary>
        /// <param name="acountName">在帐户有显性名时，可用于设定帐户名，最多支持 64 个字符。</param>
        void SetAccountName(string acountName);
        /// <summary>
        /// 设置级别 
        /// </summary>
        /// <param name="level">设定玩家当前的级别，未设定过等级的玩家默认的初始等级为“1”</param>
        void SetLevel(int level=1);
        /// <summary>
        /// 设置性别 
        /// </summary>
        /// <param name="gender">0为未知,1为男,2为女</param>
        void SetGender(int gender = 0);
        /// <summary>
        /// 设置年龄
        /// </summary>
        void SetAge(int age);
        /// <summary>
        /// 设置区服
        /// </summary>
        /// <param name="server">服务器名</param>
        void SetGameServer(string server);
        /// <summary>
        /// 充值请求
        /// </summary>
        /// <param name="orderId">订单ID,最多64个字符</param>
        /// <param name="iapId">充值包ID,最多32个字符.唯一标识一类充值包。例如：VIP3 礼包、500 元 10000 宝石包</param>
        /// <param name="currencyAmount">现金金额或现金等价物的额度</param>
        /// <param name="currencyType">支付币种</param>
        /// <param name="virtualCurrencyAmount">虚拟币金额</param>
        /// <param name="payType">支付途径</param>
        void OnChargeRequest(string orderId,string iapId,double currencyAmount, CurrencyType currencyType,double virtualCurrencyAmount, PaymentType payType);
        /// <summary>
        /// 充值成功
        /// </summary>
        /// <param name="orderId">订单ID,最多64个字符</param>
        void OnChargeSuccess(string orderId);
        /// <summary>
        /// 赠予虚拟币
        /// </summary>
        /// <param name="virtualCurrencyAmount">虚拟币金额</param>
        /// <param name="reason">赠送虚拟币的原因</param>
        void OnReward(double virtualCurrencyAmount,string reason);
        /// <summary>
        /// 记录付费点
        /// </summary>
        /// <param name="item">某个消费点的编号，最多 32 个字符</param>
        /// <param name="number">消费数量</param>
        /// <param name="price">道具单价</param>
        void OnPurchase(string item, int number, double price);
        /// <summary>
        /// 消耗物品或服务
        /// </summary>
        /// <param name="item">某个消费点的编号，最多 32 个字符</param>
        /// <param name="itemNumber">消费数量</param>
        void OnUse(string item, int itemNumber);
        /// <summary>
        /// 开始任务
        /// </summary>
        /// <param name="missionId">任务、关卡或副本的编号，最多 32 个字符。此 处可填写 ID，别名可在报表编辑</param>
        void OnBegin(string missionId);
        /// <summary>
        /// 完成任务
        /// </summary>
        /// <param name="missionId">任务、关卡或副本的编号，最多 32 个字符。此 处可填写 ID，别名可在报表编辑</param>
        void OnCompleted(string missionId);
        /// <summary>
        /// 任务失败
        /// </summary>
        /// <param name="missionId">任务、关卡或副本的编号，最多 32 个字符。此 处可填写 ID，别名可在报表编辑</param>
        /// <param name="failedCause">失败原因,最多16字符,共支持100</param>
        void OnFailed(string missionId, string failedCause);
        /// <summary>
        /// 更新用户位置信息,最好不要设置
        /// </summary>
        /// <param name="latitude">纬度</param>
        /// <param name="longitude">经度</param>
        void UpdateLocation(float latitude,float longitude);
        /// <summary>
        /// 自定义事件用于统计任何您期望去追踪的数据，如：点击某功能按钮、填写某个输入框、触发了某个广告等
        /// </summary>
        /// <param name="eventId">自定义事件名称，最多支持 32 个字符。仅限使用中英文字符、数字和下划线，不要加空格或其他的转义字符</param>
        /// <param name="eventData">key为自定义事件参数名称,一次事件最多只支持50个参数。字典中的value值只能为int或者string类型,否则不统计.若为int类型Game Analytics 会统计每种 value的总和,若为string类型则统计每种 value 出现的次数</param>
        void OnEvent(string eventId, Dictionary<string, object> eventData);
        /// <summary>
        /// 设置不显示日志  如发布时不需显示日志，应当最先调用该方法
        /// </summary>
        void SetLogDisabled();
    }

    /// <summary>
    /// 账户类型
    /// </summary>
    public enum AcountType
    {
        /// <summary>
        /// 匿名,指游客
        /// </summary>
        Anonymous=0,
        /// <summary>
        /// 注册获得的账户
        /// </summary>
        Registered=1,
        /// <summary>
        /// 微信
        /// </summary>
        Wechat=6
    }

    /// <summary>
    /// 支付币种
    /// </summary>
    [System.Flags]
    public enum CurrencyType
    {
        /// <summary>
        /// 人民币
        /// </summary>
        CNY=0,
        /// <summary>
        /// 美元
        /// </summary>
        USD,
        /// <summary>
        /// 欧元
        /// </summary>
        EUR
    }

    /// <summary>
    /// 支付途径
    /// </summary>
    [System.Flags]
    public enum PaymentType
    {
        /// <summary>
        /// 苹果支付
        /// </summary>
        ApplePay=0,
        /// <summary>
        /// 微信支付
        /// </summary>
        Wechat,
        /// <summary>
        /// 支付宝支付
        /// </summary>
        Alipay
    }
}
