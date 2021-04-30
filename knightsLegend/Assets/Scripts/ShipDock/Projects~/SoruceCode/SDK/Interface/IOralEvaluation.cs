using System;
using UnityEngine.Events;

#pragma warning disable
namespace ShipDock.SDK
{
    /// <summary>
    /// 
    /// 
    /// 
    /// Created by 田亚宗 on 2019/06/04.
    ///
    /// </summary>
    public interface IOralEvaluation
    {
        /// <summary>
        /// 确人权限
        /// </summary>
        /// <param name="callback">Callback.</param>
        void EnsurePermission(UnityAction<string> callback);
        /// <summary>
        /// 开始评测(请务必确认权限)
        /// </summary>
        /// <param name="param">评测参数.</param>
        /// <param name="callback">评测结果回调.</param>
        void StartRecord(IOralParam param, UnityAction<IOralServerResult> callback,UnityAction<string> volumCallback=null);
        /// <summary>
        /// 终止评测
        /// </summary>
        /// <param name="callback">终止后的回调.</param>
        void StopRecord(UnityAction<string> callback);
    }

    public interface IOralParam
    {
        /// <summary>
        /// 地域域名
        /// </summary>
        string soeAppId { get; set; }
        /// <summary>
        /// token
        /// </summary>
        string token { get; set; }
        /// <summary>
        /// 评测模式
        /// </summary
        EvalMode evalMode { get; set; }
        /// <summary>
        /// 工作模式
        /// </summary>
        WorkMode workMode { get; set; }
        /// <summary>
        /// 存储模式
        /// </summary>
        StorageMode storageMode { get; set; }
        /// <summary>
        /// 服务器类型
        /// </summary>
        ServerType serverType { get; set; }
        /// <summary>
        /// 文本模式
        /// </summary>
        TextMode textMode { get; set; }
        /// <summary>
        /// 苛刻指数[1.0-4.0]
        /// </summary>
        float scoreCoeff { get; set; }
        /// <summary>
        /// 评测内容
        /// </summary>
        string refText { get; set; }
        /// <summary>
        /// 分片大小[单位K]
        /// </summary>
        int fragSize { get; set; }
        /// <summary>
        /// 是否启用分片
        /// </summary>
        bool fragEnable { get; set; }
        /// <summary>
        /// 录音超时时间
        /// </summary>
        float timeout { get; set; }
        /// <summary>
        /// 参数转json文本
        /// </summary>
        string ToJson();
    }

    /// <summary>
    /// 智聆评测服务器返回值接口
    /// </summary>
    public interface IOralServerResult
    {
        /// <summary>
        /// 保存语音音频文件的下载地址
        /// </summary>
        string AudioUrl { get; }
        /// <summary>
        /// 一次批改唯一标识
        /// </summary>
        string SessionId { get; }
        /// <summary>
        /// 发音精准度，取值范围[-1, 100]，当取-1时指完全不匹配
        /// </summary>
        float PronAccuracy { get; }
        /// <summary>
        /// 发音完整度，取值范围[0, 1]，当为词模式时，取值无意义
        /// </summary>
        float PronCompletion { get; }
        /// <summary>
        /// 发音流利度，取值范围[0, 1]，当为词模式时，取值无意义
        /// </summary>
        float PronFluency { get; }
        /// <summary>
        /// 建议整体评分
        /// </summary>
        float SuggestedScore { get; }
        /// <summary>
        /// 分片序列号
        /// </summary>
        int Seq { get; }
        /// <summary>
        /// 是否最后一个分片
        /// </summary>
        int End { get; }
        /// <summary>
        /// 详细发音评估结果
        /// </summary>
        IWord[] Words { get; }
        /// <summary>
        /// 错误值
        /// </summary>
        /// <value>The error.</value>
        IError Error { get; }
        /// <summary>
        /// 外部不能调用
        /// </summary>
        IOralServerResult ParseServerResult(string json);
    }

    public interface IError
    {
        /// <summary>
        /// 错误码
        /// </summary>
        int Code { get; }
        /// <summary>
        /// 错误描述
        /// </summary>
        string Desc { get; }
        /// <summary>
        /// 请求 ID，定位错误信息
        /// </summary>
        string RequestId { get; }
    }

    public interface IWord
    {
        /// <summary>
        /// 当前单词语音起始时间点，单位为ms
        /// </summary>
        int BeginTime { get; }
        /// <summary>
        /// 当前单词语音终止时间点，单位为ms
        /// </summary>
        int EndTime { get; }
        /// <summary>
        /// 当前词与输入语句的匹配情况，0：匹配单词、1：新增单词、2：缺少单词
        /// </summary>
        int MatchTag { get; }
        /// <summary>
        /// 单词发音准确度，取值范围[-1, 100]，当取-1时指完全不匹配
        /// </summary>
        float PronAccuracy { get; }
        /// <summary>
        /// 单词发音流利度，取值范围[0, 1]
        /// </summary>
        float PronFluency { get; }
        /// <summary>
        /// 当前词
        /// </summary>
        string Content { get; }
        /// <summary>
        /// 智聆音素
        /// </summary>
        IPhoneInfo[] PhoneInfos { get; }
    }

    public interface IPhoneInfo
    {
        /// <summary>
        /// 当前音素开始时间,单位ms
        /// </summary>
        int BeginTime { get; }
        /// <summary>
        /// 当前音素结束时间,单位ms
        /// </summary>
        int EndTime { get; }
        bool DetectedStress { get; }
        /// <summary>
        /// 音素
        /// </summary>
        string Phone { get; }
        /// <summary>
        /// 当前音素发音准确度，取值范围[-1, 100]，当取-1时指完全不匹配
        /// </summary>
        float PronAccuracy {get;} 
        bool Stress { get; }
    }

    [Flags]
    public enum ErrCode
    {
        /// <summary>
        /// 成功.
        /// </summary>
        Success = 0,
        /// <summary>
        /// 参数错误
        /// </summary>
        ParamErr = 1,
        /// <summary>
        /// json错误
        /// </summary>
        JsonErr = 2,
        /// <summary>
        /// http 错误
        /// </summary>
        HttpsErr = 3,
        /// <summary>
        /// server 错误
        /// </summary>
        ServerErr = 4
    }

    public enum TextMode
    {
        /// <summary>
        /// 普通文本
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 音素文本
        /// </summary>
        Phoneme = 1
    }

    public enum ServerType
    {
        /// <summary>
        /// 英语类型
        /// </summary>
        English = 0,
        /// <summary>
        /// 中文类型
        /// </summary>
        Chinese = 1
    }

    public enum StorageMode
    {
        /// <summary>
        /// 启用
        /// </summary>
        Disable = 0,
        /// <summary>
        /// 禁止
        /// </summary>
        Enable = 1
    }

    public enum WorkMode
    {
        /// <summary>
        /// 流式
        /// </summary>
        Stream = 0,
        /// <summary>
        /// 一次性传输
        /// </summary>
        Once = 1
    }

    public enum EvalMode
    {
        /// <summary>
        /// 单词模式
        /// </summary>
        Word = 0,
        /// <summary>
        /// 句子模式
        /// </summary>
        Sentence = 1,
        /// <summary>
        /// 段落模式
        /// </summary>
        Paragraph = 2,
        /// <summary>
        /// 自由说模式
        /// </summary>
        Free = 3
    }
}

